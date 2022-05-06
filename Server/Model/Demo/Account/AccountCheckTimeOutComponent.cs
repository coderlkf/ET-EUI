namespace ET
{
    public class AccountCheckTimeOutComponent : Entity, IAwake<long>, IDestroy
    {
        public long Timer = 0;
        public long AccountId = 0;
    }
}
