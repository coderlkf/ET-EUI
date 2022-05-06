namespace ET
{
    [Timer(TimerType.AccountSessionCheckOutTime)]
    public class AccountSessionCheckOutTimeTimer : ATimer<AccountCheckTimeOutComponent>
    {
        public override void Run(AccountCheckTimeOutComponent self)
        {
            try
            {
                self.DeleteSession();
            }
            catch (System.Exception e)
            {

                Log.Error(e.ToString());
            }
        }
    }

    public class AccountCheckTimeOutComponentAwakeSystem : AwakeSystem<AccountCheckTimeOutComponent, long>
    {
        public override void Awake(AccountCheckTimeOutComponent self, long accountId)
        {
            self.AccountId = accountId;
            // 启动定时器定时检查 session 是否存活，存活则释放
            TimerComponent.Instance.Remove(ref self.Timer);
            self.Timer = TimerComponent.Instance.NewOnceTimer(TimeHelper.ServerNow() + 600000, TimerType.AccountSessionCheckOutTime, self);
        }
    }

    public class AccountCheckTimeOutComponentDestroySystem : DestroySystem<AccountCheckTimeOutComponent>
    {
        public override void Destroy(AccountCheckTimeOutComponent self)
        {
            self.AccountId = 0;
            TimerComponent.Instance.Remove(ref self.Timer);
        }
    }

    public static class AccountCheckTimeOutComponentSystem
    {
        public static void DeleteSession(this AccountCheckTimeOutComponent self)
        {
            Session session = self.GetParent<Session>();

            var accountSessionInstanceId = session.DomainScene().GetComponent<AccountSessionsComponent>().Get(self.AccountId);
            if (accountSessionInstanceId == session.InstanceId)
            {
                session.DomainScene().GetComponent<AccountSessionsComponent>().Remove(self.AccountId);
            }
            session?.Send(new A2C_Disconnect { Error = 1 });
            session?.DisconnectAsync().Coroutine();
        }
    }
}
