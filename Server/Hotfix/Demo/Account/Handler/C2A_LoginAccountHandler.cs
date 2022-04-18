using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ET
{
    [ActorMessageHandler]
    public class C2A_LoginAccountHandler : AMRpcHandler<C2A_LoginAccount, A2C_LoginAccount>
    {
        protected override async ETTask Run(Session session, C2A_LoginAccount request, A2C_LoginAccount response, Action reply)
        {
            var sceneType = session.DomainScene().SceneType;
            // 判断 sceneType 是否为 Account
            if (sceneType != SceneType.Account)
            {
                Log.Error($"请求的 scene 错误，当前请求的 scene 为{sceneType}");
                session.Dispose();
                return;
            }

            if (session.GetComponent<SessionLockingComponent>() != null)
            {
                response.Error = ErrorCode.Err_RequestRepeatedly;
                reply();
                // 延迟释放 session 保证正常回复客户端消息
                session.DisconnectAsync().Coroutine();
                return;
            }

            // 移除验证组件,表示验证通过
            session.RemoveComponent<SessionAcceptTimeoutComponent>();

            if (string.IsNullOrEmpty(request.AccountName) || string.IsNullOrEmpty(request.Password))
            {
                response.Error = ErrorCode.Err_LoginInfoIsNull;
                reply();
                // 延迟释放 session 保证正常回复客户端消息
                session.DisconnectAsync().Coroutine();
                return;
            }

            if (!Regex.IsMatch(request.AccountName, @"^[a-zA-Z][a-zA-Z0-9_]{6,15}$"))
            {
                response.Error = ErrorCode.Err_AccountNameFormError;
                reply();
                session.DisconnectAsync().Coroutine();
                return;
            }
            if (!Regex.IsMatch(request.Password, @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,16}$"))
            {
                response.Error = ErrorCode.Err_PasswordFormError;
                reply();
                session.DisconnectAsync().Coroutine();
                return;
            }

            // 防止同玩家同会话重复请求
            using (session.AddComponent<SessionLockingComponent>())
            // 使用携程锁防止不同玩家的相同请求
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.AccountLogin, request.AccountName.Trim().GetHashCode(), 1000))
            {
                var accountList = await DBManagerComponent.Instance.GetZoneDB(session.DomainZone()).Query<Account>(a => a.AccountName.Equals(request.AccountName.Trim()));
                Account account = null;
                if (accountList.Count == 0)
                {
                    // 自动注册
                    account = session.AddChild<Account>();
                    account.AccountName = request.AccountName.Trim();
                    account.Password = request.Password;
                    account.CreateTime = TimeHelper.ServerNow();
                    account.AccountType = (int)AccountType.General;
                    await DBManagerComponent.Instance.GetZoneDB(session.DomainZone()).Save(account);
                }
                else
                {
                    account = accountList.FirstOrDefault(a => a.Password == request.Password);
                    if (account == null)
                    {
                        response.Error = ErrorCode.Err_LoginPasswordError;
                        reply();
                        session.DisconnectAsync().Coroutine();
                        account?.Dispose();
                        return;
                    }
                    if (account.AccountType == (int)AccountType.BlackList)
                    {
                        response.Error = ErrorCode.Err_AccountInBlackListError;
                        reply();
                        session.DisconnectAsync().Coroutine();
                        account?.Dispose();
                        return;
                    }
                }
                var token = $"{TimeHelper.ServerNow()}|{Guid.NewGuid()}";

                var tokenComponent = session.DomainScene().GetComponent<TokenComponent>();
                tokenComponent.Remove(account.Id);
                tokenComponent.Add(account.Id, token);

                response.AccountId = account.Id;
                response.token = token;

                reply();
                account?.Dispose();
            }
        }
    }
}
