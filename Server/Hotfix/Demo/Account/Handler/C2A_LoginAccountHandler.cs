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

            // 移除验证组件,表示验证通过
            session.RemoveComponent<SessionAcceptTimeoutComponent>();

            if (string.IsNullOrEmpty(request.AccountName) || string.IsNullOrEmpty(request.Password))
            {
                response.Error = ErrorCode.Err_LoginInfoError;
                reply();
                session.Dispose();
                return;
            }

            if (!Regex.IsMatch(request.AccountName, @"^[a-zA-Z][a-zA-Z0-9_]{6,15}$"))
            {
                response.Error = ErrorCode.Err_AccountNameError;
                reply();
                session.Dispose();
                return;
            }
            if (!Regex.IsMatch(request.Password, @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,16}$"))
            {
                response.Error = ErrorCode.Err_PasswordError;
                reply();
                session.Dispose();
                return;
            }

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
                    response.Error = ErrorCode.Err_PasswordUnPassError;
                    reply();
                    session.Dispose();
                    return;
                }
                if (account.AccountType == (int)AccountType.BlackList)
                {
                    response.Error = ErrorCode.Err_AccountUnEnableError;
                    reply();
                    session.Dispose();
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
        }
    }
}
