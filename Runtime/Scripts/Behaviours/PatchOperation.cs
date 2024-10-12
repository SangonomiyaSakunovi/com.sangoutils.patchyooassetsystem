using System;
using System.Collections.Generic;
using YooAsset;

namespace SangoUtils.Patchs_YooAsset
{
    internal class PatchOperation : GameAsyncOperation
    {
        private readonly Dictionary<PatchOperationEventCode, PatchOperationOP_Base> _patchOperationOPs = new();
        private void AddPatchOperationOP<T>() where T : PatchOperationOP_Base, new()
        {
            var patchOperationOP = Activator.CreateInstance<T>();
            _patchOperationOPs.Add(patchOperationOP.PatchOperationEventCode, patchOperationOP);
        }

        internal PatchOperation()
        {
            EventBus_Patchs.AddPatchUserEvent(OnPatchUserEvent);
            EventBus_Patchs.AddPatchOperationEvent(OnPatchOperationEvent);
            AddPatchOperationOPs();
        }

        protected override void OnStart()
        {
            EventBus_Patchs.CallPatchOperationEvent(this, new PatchOperationEventArgs(PatchOperationEventCode.InitializePackage));
        }

        protected override void OnUpdate() { }

        protected override void OnAbort() { }

        private void AddPatchOperationOPs()
        {
            AddPatchOperationOP<PatchOperationOP_InitializePackage>();
            AddPatchOperationOP<PatchOperationOP_UpdatePackageVersion>();
            AddPatchOperationOP<PatchOperationOP_UpdatePackageManifest>();
            AddPatchOperationOP<PatchOperationOP_CreatePackageDownloader>();
            AddPatchOperationOP<PatchOperationOP_DownloadPackageFiles>();
            AddPatchOperationOP<PatchOperationOP_DownloadPackageOver>();
            AddPatchOperationOP<PatchOperationOP_ClearPackageCache>();
            AddPatchOperationOP<PatchOperationOP_LoadDll>();
            AddPatchOperationOP<PatchOperationOP_UpdaterDone>();
        }

        private void OnPatchUserEvent(object sender, PatchUserEventArgs eventArgs)
        {
            switch (eventArgs.PatchUserEventCode)
            {
                case PatchUserEventCode.UserTryInitialize:
                    EventBus_Patchs.CallPatchOperationEvent(this, new PatchOperationEventArgs(PatchOperationEventCode.InitializePackage));
                    break;
                case PatchUserEventCode.UserBeginDownloadWebFiles:
                    EventBus_Patchs.CallPatchOperationEvent(this, new PatchOperationEventArgs(PatchOperationEventCode.DownloadPackageFiles));
                    break;
                case PatchUserEventCode.UserTryUpdatePackageVersion:
                    EventBus_Patchs.CallPatchOperationEvent(this, new PatchOperationEventArgs(PatchOperationEventCode.UpdatePackageVersion));
                    break;
                case PatchUserEventCode.UserTryUpdatePatchManifest:
                    EventBus_Patchs.CallPatchOperationEvent(this, new PatchOperationEventArgs(PatchOperationEventCode.UpdatePackageManifest));
                    break;
                case PatchUserEventCode.UserTryDownloadWebFiles:
                    EventBus_Patchs.CallPatchOperationEvent(this, new PatchOperationEventArgs(PatchOperationEventCode.CreatePackageDownloader));
                    break;
            }
        }

        private void OnPatchOperationEvent(object sender, PatchOperationEventArgs eventArgs)
        {
            if (_patchOperationOPs.TryGetValue(eventArgs.PatchOperationEventCode, out var patchOperationOP))
            {
                patchOperationOP.OnEvent();
            }
            if (eventArgs.PatchOperationEventCode == PatchOperationEventCode.UpdaterDone)
            {
                EventBus_Patchs.RemovePatchOperationEvent(OnPatchOperationEvent);
                EventBus_Patchs.RemovePatchUserEvent(OnPatchUserEvent);
                Status = EOperationStatus.Succeed;
            }
        }
    }
}