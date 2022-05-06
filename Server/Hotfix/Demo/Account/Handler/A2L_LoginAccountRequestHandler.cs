using System;

namespace ET
{
    [ActorMessageHandler]
    public class A2L_LoginAccountRequestHandler : AMActorRpcHandler<Scene, A2L_LoginAccountRequest, L2A_LoginAccountResponse>
    {
        protected override async ETTask Run(Scene scene, A2L_LoginAccountRequest request, L2A_LoginAccountResponse response, Action reply)
        {
            var accountId = request.AccountId;
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginCenterLock, accountId.GetHashCode()))
            {
                if (!scene.GetComponent<LoginInfoRecordComponent>().IsExist(accountId))
                {
                    reply();
                    return;
                }
                // 发送下线消息给网关服务器
                var zone = scene.GetComponent<LoginInfoRecordComponent>().Get(accountId);
                StartSceneConfig startSceneConfig = RealmGateAddressHelper.GetGate(zone, accountId);
                G2L_DisconnectionGateUnit g2L_DisconnectionGateUnit = (G2L_DisconnectionGateUnit)await MessageHelper.CallActor(startSceneConfig.InstanceId, new L2G_DisconnectionGateUnit { AccountId = accountId });
                response.Error = g2L_DisconnectionGateUnit.Error;
                reply();
            }

            await ETTask.CompletedTask;
        }
    }
}
