namespace ET
{
    public static class TokenComponentSystem
    {
        public static void Add(this TokenComponent self, long accountId, string token)
        {
            self.TokenDictonary.Add(accountId, token);
            self.TimeOutTokenRemove(accountId, token).Coroutine();
        }

        public static string Get(this TokenComponent self, long accountId)
        {
            self.TokenDictonary.TryGetValue(accountId, out string result);
            return result;
        }

        public static void Remove(this TokenComponent self, long accountId)
        {
            if (self.TokenDictonary.ContainsKey(accountId))
                self.TokenDictonary.Remove(accountId);
        }

        public static async ETTask TimeOutTokenRemove(this TokenComponent self, long accountId, string token)
        {
            await TimerComponent.Instance.WaitAsync(600000);

            var onlineToken = self.Get(accountId);
            if (string.IsNullOrEmpty(onlineToken) && onlineToken.Equals(token))
            {
                self.Remove(accountId);
            }
        }
    }
}
