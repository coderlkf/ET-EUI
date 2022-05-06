namespace ET
{
    public class ServerInfoManagerComponentAwakeSystem : AwakeSystem<ServerInfoManagerComponent>
    {
        public override void Awake(ServerInfoManagerComponent self)
        {
            self.Awake().Coroutine();
        }
    }

    public class ServerInfoManagerComponentDestroySystem : DestroySystem<ServerInfoManagerComponent>
    {
        public override void Destroy(ServerInfoManagerComponent self)
        {
            self.serverInfos?.ForEach(s =>
            {
                s?.Dispose();
            });
            self.serverInfos.Clear();
        }
    }

    public class ServerInfoManagerComponentLoadSystem : LoadSystem<ServerInfoManagerComponent>
    {
        public override void Load(ServerInfoManagerComponent self)
        {
            self.Awake().Coroutine();
        }
    }

    public static class ServerInfoManagerComponentSystem
    {
        public static async ETTask Awake(this ServerInfoManagerComponent self)
        {
            var serverInfos = await DBManagerComponent.Instance.GetZoneDB(self.DomainZone()).Query<ServerInfo>(s => true);
            if (serverInfos == null || serverInfos.Count <= 0)
            {
                Log.Error("serverinfo count is zero");

                var serverInfosConfig = ServerInfoConfigCategory.Instance.GetAll();
                serverInfos = new System.Collections.Generic.List<ServerInfo>();
                foreach (var config in serverInfosConfig.Values)
                {
                    var newServerInfo = self.AddChildWithId<ServerInfo>(config.Id);
                    newServerInfo.ServerName = config.ServerName;
                    newServerInfo.Status = (int)ServerStatus.Normal;
                    serverInfos.Add(newServerInfo);
                }
                await DBManagerComponent.Instance.GetZoneDB(self.DomainZone()).InsertBatch(serverInfos);
                return;
            }
            self.serverInfos.Clear();
            serverInfos.ForEach(s =>
            {
                self.AddChild(s);
                self.serverInfos.Add(s);
            });

            await ETTask.CompletedTask;
        }
    }
}
