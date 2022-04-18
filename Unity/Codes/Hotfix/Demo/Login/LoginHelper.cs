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

            // ±£´æ Token ºÍ AccountId
            var accountInfoComponent = zoneScene.GetComponent<AccountInfoComponent>();
            accountInfoComponent.Token = a2C_LoginAccount.token;
            accountInfoComponent.AccountId = a2C_LoginAccount.AccountId;
            return ErrorCode.ERR_Success;
        }
    }
}