using SangoUtils.Patchs_YooAsset.Utils;
using System.Collections;
using UnityEngine;
using YooAsset;

namespace SangoUtils.Patchs_YooAsset
{
    internal class PatchOperationOP_CreatePackageDownloader : PatchOperationOP_Base
    {
        internal override PatchOperationEventCode PatchOperationEventCode => PatchOperationEventCode.CreatePackageDownloader;

        internal override void OnEvent()
        {
            EventBus_Patchs.CallPatchSystemEvent(this, new PatchSystemEventArgs(PatchSystemEventCode.PatchStatesChange, "创建补丁下载器！"));
            CreateDownloaderASync().Start();
        }

        private IEnumerator CreateDownloaderASync()
        {
            yield return new WaitForSecondsRealtime(0.5f);

            var packageName = EventBus_Patchs.PatchConfig.PackageName;
            var package = YooAssets.GetPackage(packageName);
            var tag = EventBus_Patchs.PatchConfig.DefaultTag;
            int downloadingMaxNumber = EventBus_Patchs.PatchConfig.DefaultDownloadingMaxNumber;
            int failedTryAgain = EventBus_Patchs.PatchConfig.DefaultFailedTryAgainCount;
            int timeout = EventBus_Patchs.PatchConfig.DefaultTimeout;
            var downloader = package.CreateResourceDownloader(tag, downloadingMaxNumber, failedTryAgain, timeout);

            EventBus_Patchs.PatchConfig.ResourceDownloaderOperation = downloader;

            if (downloader.TotalDownloadCount == 0)
            {
                Debug.Log("Not found any download files !");
                EventBus_Patchs.CallPatchOperationEvent(this, new PatchOperationEventArgs(PatchOperationEventCode.LoadDll));
            }
            else
            {
                // 发现新更新文件后，挂起流程系统
                // 注意：开发者需要在下载前检测磁盘空间不足
                int totalDownloadCount = downloader.TotalDownloadCount;
                long totalDownloadBytes = downloader.TotalDownloadBytes;
                EventBus_Patchs.CallPatchSystemEvent(this, new PatchSystemEventArgs(PatchSystemEventCode.FoundUpdateFiles, totalDownloadCount, totalDownloadBytes));
            }
        }
    }
}