using YooAsset;

namespace SangoUtils.Patchs_YooAsset
{
    internal class PatchOperationOP_ClearPackageCache : PatchOperationOP_Base
    {
        internal override PatchOperationEventCode PatchOperationEventCode => PatchOperationEventCode.ClearPackageCache;

        internal override void OnEvent()
        {
            EventBus_Patchs.CallPatchSystemEvent(this, new PatchSystemEventArgs(PatchSystemEventCode.PatchStatesChange, "����δʹ�õĻ����ļ���"));
            var packageName = EventBus_Patchs.PatchConfig.PackageName;
            var package = YooAssets.GetPackage(packageName);
            var operation = package.ClearUnusedBundleFilesAsync();
            operation.Completed += Operation_Completed;
        }

        private void Operation_Completed(AsyncOperationBase obj)
        {
            EventBus_Patchs.CallPatchOperationEvent(this, new PatchOperationEventArgs(PatchOperationEventCode.LoadDll));
        }
    }
}