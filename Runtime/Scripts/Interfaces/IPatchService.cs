using System;

namespace SangoUtils.Patchs_YooAsset
{
    public interface IPatchService
    {
        void OnInit(PatchRoot root);
        void OnStart();
        void OnMessageBoxEvent(PatchMessageBoxEventType messageType, Action onMessageBoxBtnTryAgainClickedCB, params string[] extensionDatas);
        void OnUpdateDownloadingProgress(int currentDownloadCount, int totalDownloadCount, long currentDownloadSizeBytes, long totalDownloadSizeBytes);
        void OnEnd();
    }

    public enum PatchMessageBoxEventType
    {
        InitFailed,
        PackageVersionUpdateFailed,
        FilesNeedUpdateFound,
        ManifestUpdateFailed,
        PartFilesDownloadFailed
    }
}
