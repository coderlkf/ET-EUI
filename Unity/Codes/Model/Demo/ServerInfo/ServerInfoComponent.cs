using System.Collections.Generic;

namespace ET
{
    public class ServerInfoComponent : Entity, IAwake, IDestroy
    {
        public List<ServerInfo> serverInfos = new List<ServerInfo>();
    }
}
