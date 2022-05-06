using System;


namespace ET
{
    public static class LoginHelper
    {
        public static async ETTask<int> Login(Scene zoneScene, string address, string account, string password)
        {
            A2C_LoginAccount a2C_LoginAccount = null;
            Session accountSession = null;

            try
            {
                accountSession = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(address));
                password = MD5Helper.StringMD5(password);
                a2C_LoginAccount = (A2C_LoginAccount)await accountSession.Call(new C2A_LoginAccount { AccountName = account, Password = password });
            }
            catch (System.Exception e)
            {
                accountSession?.Dispose();
                Log.Error(e.ToString());
                return ErrorCode.ERR_NetWorkError;
            }

            if (a2C_LoginAccount.Error != ErrorCode.ERR_Success)
            {
                accountSession?.Dispose();
                return a2C_LoginAccount.Error;
            }
            zoneScene.AddComponent<SessionComponent>().Session = accountSession;
            // 添加心跳检测组件
            zoneScene.GetComponent<SessionComponent>().Session.AddComponent<PingComponent>();

            // 保存 Token 和 AccountId
            var accountInfoComponent = zoneScene.GetComponent<AccountInfoComponent>();
            accountInfoComponent.Token = a2C_LoginAccount.token;
            accountInfoComponent.AccountId = a2C_LoginAccount.AccountId;
            return ErrorCode.ERR_Success;
        }

        public static async ETTask<int> GetServerInfosList(Scene zoneScene)
        {
            try
            {
                var accountInfo = zoneScene.GetComponent<AccountInfoComponent>();
                var a2C_GetServerInfos = (A2C_GetServerInfos)await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_GetServerInfos { Token = accountInfo.Token, AccountId = accountInfo.AccountId });

                if (a2C_GetServerInfos.Error != ErrorCode.ERR_Success)
                {
                    return a2C_GetServerInfos.Error;
                }

                var serverInfoComponent = zoneScene.GetComponent<ServerInfoComponent>();
                a2C_GetServerInfos?.ServerInfosList.ForEach(s =>
                {
                    var serverInfo = serverInfoComponent.AddChild<ServerInfo>();
                    serverInfo.FromMessage(s);
                    serverInfoComponent.Add(serverInfo);
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return ErrorCode.ERR_NetWorkError;
            }
            await ETTask.CompletedTask;
            return ErrorCode.ERR_Success;
        }

        public static async ETTask<int> CreateRole(Scene zoneScene, string name)
        {
            A2C_CreateRole a2C_CreateRole = null;
            try
            {
                var accountInfo = zoneScene.GetComponent<AccountInfoComponent>();
                a2C_CreateRole = (A2C_CreateRole)await zoneScene.GetComponent<SessionComponent>().Session.Call(new C2A_CreateRole
                {
                    Name = name,
                    AccountId = accountInfo.AccountId,
                    Token = accountInfo.Token,
                    ServerId = 1
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return ErrorCode.ERR_NetWorkError;
            }

            if (a2C_CreateRole.Error != ErrorCode.ERR_Success)
            {
                Log.Error(a2C_CreateRole.Error.ToString());
                return a2C_CreateRole.Error;
            }

            await ETTask.CompletedTask;
            return ErrorCode.ERR_Success;
        }
    }
}