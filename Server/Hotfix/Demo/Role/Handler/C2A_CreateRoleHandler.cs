using System;

namespace ET
{
    public class C2A_CreateRoleHandler : AMRpcHandler<C2A_CreateRole, A2C_CreateRole>
    {
        protected override async ETTask Run(Session session, C2A_CreateRole request, A2C_CreateRole response, Action reply)
        {
            var sceneType = session.DomainScene().SceneType;
            // 判断 sceneType 是否为 Account
            if (sceneType != SceneType.Account)
            {
                Log.Error($"请求的 scene 错误，当前请求的 scene 为{sceneType}");
                session.Dispose();
                return;
            }

            // 校验 token
            var token = session.DomainScene().GetComponent<TokenComponent>().Get(request.AccountId);
            if (token == null || token != request.Token)
            {
                response.Error = ErrorCode.Err_TokenError;
                reply();
                session.DisconnectAsync().Coroutine();
                return;
            }

            if (string.IsNullOrEmpty(request.Name))
            {
                response.Error = ErrorCode.ERR_RoleNameEmptyError;
                reply();
                session.DisconnectAsync().Coroutine();
                return;
            }
            // TODO 名称格式校验，敏感词过滤

            if (session.GetComponent<SessionLockingComponent>() != null)
            {
                response.Error = ErrorCode.Err_RequestRepeatedly;
                reply();
                // 延迟释放 session 保证正常回复客户端消息
                session.DisconnectAsync().Coroutine();
                return;
            }
            // 防止同玩家同会话重复请求
            using (session.AddComponent<SessionLockingComponent>())
            // 使用携程锁防止不同玩家的相同请求
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.CreateRole, request.AccountId))
            {
                var roleInfos = await DBManagerComponent.Instance.GetZoneDB(session.DomainZone()).Query<RoleInfo>(r => r.ServerId == request.ServerId && r.Name == request.Name);
                if (roleInfos != null && roleInfos.Count > 0)
                {
                    response.Error = ErrorCode.ERR_RoleNameExist;
                    reply();
                    session.DisconnectAsync().Coroutine();
                    return;
                }

                var roleInfo = session.AddChildWithId<RoleInfo>(IdGenerater.Instance.GenerateUnitId(session.DomainZone()));
                roleInfo.Name= request.Name;
                roleInfo.AccountId= request.AccountId;
                roleInfo.ServerId= request.ServerId;
                roleInfo.State = (int)RoleState.Normal;
                roleInfo.CreateTime = TimeHelper.ServerNow();
                roleInfo.LastLoginTime = 0;

                await DBManagerComponent.Instance.GetZoneDB(session.DomainZone()).Save(roleInfo);
                response.RoleInfo = roleInfo.ToMessage();
                response.Error = ErrorCode.ERR_Success;
                reply();
            }
        }
    }
}
