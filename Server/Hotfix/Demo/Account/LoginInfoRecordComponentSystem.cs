namespace ET
{
    public class LoginInfoRecordComponentDestroySystem : DestroySystem<LoginInfoRecordComponent>
    {
        public override void Destroy(LoginInfoRecordComponent self)
        {
            self.LoginInfoRecordDict.Clear();
        }
    }

    public static class LoginInfoRecordComponentSystem
    {
        public static void Add(this LoginInfoRecordComponent self, long accountId, int gate)
        {
            if (self.LoginInfoRecordDict.TryAdd(accountId, gate))
            {
                return;
            }
            self.LoginInfoRecordDict[accountId] = gate;
        }

        public static void Remove(this LoginInfoRecordComponent self, long accountId)
        {
            if (self.LoginInfoRecordDict.ContainsKey(accountId))
            {
                self.LoginInfoRecordDict.Remove(accountId);
            }
        }

        public static int Get(this LoginInfoRecordComponent self, long accountId)
        {
            if (!self.LoginInfoRecordDict.TryGetValue(accountId, out int result))
            {
                return -1;
            }
            return result;
        }

        public static bool IsExist(this LoginInfoRecordComponent self, long accountId)
        {
            return self.LoginInfoRecordDict.ContainsKey((accountId));
        }
    }
}
