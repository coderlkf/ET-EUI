namespace ET
{
    public class ServerInfoComponentDestroySystem : DestroySystem<ServerInfoComponent>
    {
        public override void Destroy(ServerInfoComponent self)
        {
            self.serverInfos.ForEach(s => s?.Dispose());
            self.serverInfos.Clear();
        }
    }
    public static class ServerInfoComponentSystem
    {
        public static void Add(this ServerInfoComponent self,ServerInfo serverInfo)
        {
            self.serverInfos.Add(serverInfo);
        }
    }
}
