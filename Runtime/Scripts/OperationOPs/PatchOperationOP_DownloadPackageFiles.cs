using SangoUtils.Patchs_YooAsset.Utils;
using System.Collections;
using YooAsset;

namespace SangoUtils.Patchs_YooAsset
{
    internal class PatchOperationOP_DownloadPackageFiles : PatchOperationOP_Base
    {
        internal override PatchOperationEventCode PatchOperationEventCode => PatchOperationEventCode.DownloadPackageFiles;

        internal override void OnEvent()
        {
            EventBus_Patchs.CallPatchSystemEvent(this, new PatchSystemEventArgs(PatchSystemEventCode.PatchStatesChange, "开始下载补丁文件！"));
            BeginDownloadASync().Start();
        }

        private IEnumerator BeginDownloadASync()
        {
            var downloaderOperation = EventBus_Patchs.PatchConfig.ResourceDownloaderOperation;
            downloaderOperation.OnDownloadProgressCallback = delegate (int totalDownloadCount, int currentDownloadCount, long totalDownloadSizeBytes, long currentDownloadSizeBytes)
            {
                EventBus_Patchs.CallPatchSystem_DownloadProgressUpdateEvent(this, new PatchSystem_DownloadProgressUpdateEventArgs
                    (totalDownloadCount, currentDownloadCount, totalDownloadSizeBytes, currentDownloadSizeBytes));
            };
            downloaderOperation.OnDownloadErrorCallback = delegate (string fileName, string error)
            {
                EventBus_Patchs.CallPatchSystemEvent(this, new PatchSystemEventArgs(PatchSystemEventCode.WebFileDownloadFailed, fileName, error));
            };
            downloaderOperation.BeginDownload();
            yield return downloaderOperation;

            if (downloaderOperation.Status != EOperationStatus.Succeed)
            {
                EventBus_Patchs.CallCustomPatchEvent(CustomPatchEventCode.PatchFailed);
                yield break;
            }

            EventBus_Patchs.CallPatchOperationEvent(this, new PatchOperationEventArgs(PatchOperationEventCode.DownloadPackageOver));
        }
    }
}