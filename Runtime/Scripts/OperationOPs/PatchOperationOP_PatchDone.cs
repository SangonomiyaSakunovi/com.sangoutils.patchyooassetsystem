namespace SangoUtils.Patchs_YooAsset
{
    internal class PatchOperationOP_PatchDone : PatchOperationOP_Base
    {
        internal override PatchOperationEventCode PatchOperationEventCode => PatchOperationEventCode.PatchDone;

        internal override void OnEvent()
        {
            EventBus_Patchs.CallCustomPatchEvent(CustomPatchEventCode.PatchDone);
        }
    }
}