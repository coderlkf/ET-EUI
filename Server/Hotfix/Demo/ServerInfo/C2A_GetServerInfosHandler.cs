using System;
using System.Linq;

namespace ET
{
    [MessageHandler]
    public class C2A_GetServerInfosHandler : AMRpcHandler<C2A_GetServerInfos, A2C_GetServerInfos>
    {
        protected override async ETTask Run(Session session, C2A_GetServerInfos request, A2C_GetServerInfos response, Action reply)
        {
            var sceneType = session.DomainScene().SceneType;
            // 判断 sceneType 是否为 Account
            if (sceneType != SceneType.Account)
            {
                Log.Error($"请求的 scene 错误，当前请求的 scene 为{sceneType}");
                session.Dispose();
                return;
            }

            var token = session.DomainScene().GetComponent<TokenComponent>().Get(request.AccountId);
            if (token == null || token != request.Token)
            {
                response.Error = ErrorCode.Err_TokenError;
                reply();
                session.DisconnectAsync().Coroutine();
                return;
            }

            var serverInfos = session.DomainScene().GetComponent<ServerInfoManagerComponent>().serverInfos;
            var serverInfoProtos = serverInfos?.Select(s => s.ToMessage());
            if (serverInfoProtos != null)
            {
                response.ServerInfosList.AddRange(serverInfoProtos);
            }
            reply();
            await ETTask.CompletedTask;
        }
    }
}
