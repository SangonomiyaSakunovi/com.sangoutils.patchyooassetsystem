namespace SangoUtils.Patchs_YooAsset
{
    internal class PatchOperationOP_UpdaterDone : PatchOperationOP_Base
    {
        internal override PatchOperationEventCode PatchOperationEventCode => PatchOperationEventCode.UpdaterDone;

        internal override void OnEvent()
        {
            EventBus_Patchs.PatchEvent.OnUpdaterDone?.Invoke();
        }
    }
}