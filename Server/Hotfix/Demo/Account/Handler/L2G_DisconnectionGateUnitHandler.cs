using System;

namespace ET
{
    [ActorMessageHandler]
    public class L2G_DisconnectionGateUnitHandler : AMActorRpcHandler<Scene, L2G_DisconnectionGateUnit, G2L_DisconnectionGateUnit>
    {
        protected override async ETTask Run(Scene scene, L2G_DisconnectionGateUnit request, G2L_DisconnectionGateUnit response, Action reply)
        {
            var accountId = request.AccountId;
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.GateLoginLock, accountId.GetHashCode()))
            {
                PlayerComponent playerComponent = scene.GetComponent<PlayerComponent>();
                Player gateUnit = playerComponent.Get(accountId);
                if (gateUnit == null)
                {
                    reply();
                    return;
                }
                playerComponent.Remove(accountId);
                gateUnit.Dispose();
            }
            reply();
            await ETTask.CompletedTask;
        }
    }
}
