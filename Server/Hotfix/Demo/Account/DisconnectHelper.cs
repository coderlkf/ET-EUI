namespace ET
{
    public static class DisconnectHelper
    {
        public static async ETTask DisconnectAsync(this Session self)
        {
            if (self == null || self.IsDisposed)
            {
                return;
            }

            var instanceId = self.InstanceId;
            await TimerComponent.Instance.WaitAsync(1000);
            if (instanceId == self?.InstanceId)
            {
                self.Dispose();
            }
        }
    }
}
