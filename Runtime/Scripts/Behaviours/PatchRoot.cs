using SangoUtils.Patchs_YooAsset.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YooAsset;

namespace SangoUtils.Patchs_YooAsset
{
    [RequireComponent(typeof(PatchEvent))]
    public class PatchRoot : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour _patchWnd;
        [SerializeField] private PatchConfigObj _patchConfig;

        private IPatchService _IPatchWnd;

        private List<DownloadFailedFileInfo> _downloadFailedFileInfos;

        private void Awake()
        {
            new EventBus_Patchs();

            var cfg = new PatchConfig();
            SetConfig(ref cfg);
            EventBus_Patchs.PatchConfig = cfg;
            EventBus_Patchs.SetCustomPatchEvent(GetComponent<PatchEvent>());

            EventBus_Patchs.AddPatchSystemEvent(OnPatchSystemEvent);
            EventBus_Patchs.AddPatchSystem_DownloadProgressUpdateEvent(OnPatchSystemDownloadProgressUpdateEvent);

            if (_patchWnd.GetType().GetInterfaces().Any(iface => iface == typeof(IPatchService)))
            {
                _IPatchWnd = (IPatchService)_patchWnd;
                _IPatchWnd.OnInit(this);
            }
            else
            {
                Debug.LogError("No Patch Wnd Found! You need set it!");
            }
        }

        private void Start()
        {
            StartOperationASync().Start();
            _IPatchWnd.OnStart();
        }

        /// <summary>
        /// TODO: Only for test.
        /// </summary>
        public void ReStartDefaultHotFix()
        {
            StartOperationASync().Start();
            _IPatchWnd.OnStart();
        }

        private void SetConfig(ref PatchConfig cfg)
        {
            //For we do not know if the ScriptableObject will influenced the RuntimeConfig, we`d better store it by others.
            cfg.CDNPathAndroidEditor = _patchConfig.CDNPathAndroidEditor;
            cfg.CDNPathIOSEditor = _patchConfig.CDNPathIOSEditor;
            cfg.CDNPathWebGLEditor = _patchConfig.CDNPathWebGLEditor;
            cfg.CDNPathOthersEditor = _patchConfig.CDNPathOthersEditor;

            cfg.CDNPathAndroidRuntime = _patchConfig.CDNPathAndroidRuntime;
            cfg.CDNPathIOSRuntime = _patchConfig.CDNPathIOSRuntime;
            cfg.CDNPathWebGLRuntime = _patchConfig.CDNPathWebGLRuntime;
            cfg.CDNPathOthersRuntime = _patchConfig.CDNPathOthersRuntime;

            cfg.PackageName = _patchConfig.PackageName;
            cfg.DefaultTag = _patchConfig.DefaultTag;

            cfg.DefaultDownloadingMaxNumber = _patchConfig.DefaultDownloadingMaxNumber;
            cfg.DefaultFailedTryAgainCount = _patchConfig.DefaultFailedTryAgainCount;
            cfg.DefaultTimeout = _patchConfig.DefaultTimeout;

            cfg.PlayMode = _patchConfig.PlayMode;
            cfg.BuildPipeline = _patchConfig.BuildPipeline;

            cfg.AOTAssemblyFileNames = _patchConfig.AOTAssemblyFileNames;
            cfg.HotFixAssemblyFileNames = _patchConfig.HotFixAssemblyFileNames;
            cfg.HotFixRootPrefabPath = _patchConfig.HotFixRootPrefabPath;
        }

        private IEnumerator StartOperationASync()
        {
            YooAssets.Initialize();

            PatchOperation hotFixOperation = new();
            YooAssets.StartOperation(hotFixOperation);
            yield return hotFixOperation;

            ResourcePackage assetPackage = YooAssets.GetPackage(EventBus_Patchs.PatchConfig.PackageName);
            YooAssets.SetDefaultPackage(assetPackage);

            EventBus_Patchs.CallPatchSystemEvent(this, new PatchSystemEventArgs(PatchSystemEventCode.OnPatchEnd));
        }

        private void OnPatchSystemEvent(object sender, PatchSystemEventArgs eventArgs)
        {
            switch (eventArgs.PatchSystemEventCode)
            {
                case PatchSystemEventCode.InitializeFailed:
                    Action callback = delegate
                    {
                        EventBus_Patchs.CallPatchUserEvent(this, new PatchUserEventArgs(PatchUserEventCode.UserTryInitialize));
                    };
                    _IPatchWnd.OnMessageBoxEvent(PatchMessageBoxEventType.InitFailed, callback);
                    break;
                case PatchSystemEventCode.PatchStatesChange:

                    break;
                case PatchSystemEventCode.FoundUpdateFiles:
                    int totalCount = int.Parse(eventArgs.ExtensionData[0].ToString());
                    long totalSizeBytes = long.Parse(eventArgs.ExtensionData[1].ToString());
                    Action callback1 = delegate
                    {
                        EventBus_Patchs.CallPatchUserEvent(this, new PatchUserEventArgs(PatchUserEventCode.UserBeginDownloadWebFiles));
                    };
                    float sizeMB = totalSizeBytes / 1048576f;
                    sizeMB = Mathf.Clamp(sizeMB, 0.1f, float.MaxValue);
                    string totalSizeMB = sizeMB.ToString("f1");
                    _IPatchWnd.OnMessageBoxEvent(PatchMessageBoxEventType.FilesNeedUpdateFound, callback1, new string[] { totalSizeMB });
                    break;
                case PatchSystemEventCode.PackageVersionUpdateFailed:
                    Action callback2 = delegate
                    {
                        EventBus_Patchs.CallPatchUserEvent(this, new PatchUserEventArgs(PatchUserEventCode.UserTryUpdatePackageVersion));
                    };
                    _IPatchWnd.OnMessageBoxEvent(PatchMessageBoxEventType.PackageVersionUpdateFailed, callback2);
                    break;
                case PatchSystemEventCode.PatchManifestUpdateFailed:
                    Action callback3 = delegate
                    {
                        EventBus_Patchs.CallPatchUserEvent(this, new PatchUserEventArgs(PatchUserEventCode.UserTryUpdatePatchManifest));
                    };
                    _IPatchWnd.OnMessageBoxEvent(PatchMessageBoxEventType.ManifestUpdateFailed, callback3);
                    break;
                case PatchSystemEventCode.PartWebFileDownloadFailed:
                    string fileName = eventArgs.ExtensionData[0].ToString();
                    string Error = eventArgs.ExtensionData[1].ToString();

                    if (_downloadFailedFileInfos == null)
                        _downloadFailedFileInfos = new List<DownloadFailedFileInfo>();

                    _downloadFailedFileInfos.Add(new DownloadFailedFileInfo() { FileName = fileName, Error = Error });
                    break;
                case PatchSystemEventCode.OnAllDownloadFailedFilesFound:
                    Action callback4 = delegate
                    {
                        EventBus_Patchs.CallPatchUserEvent(this, new PatchUserEventArgs(PatchUserEventCode.UserTryDownloadWebFiles));
                    };
                    _IPatchWnd.OnMessageBoxEvent(PatchMessageBoxEventType.PartFilesDownloadFailed, callback4);
                    break;
                case PatchSystemEventCode.OnPatchEnd:
                    _IPatchWnd.OnEnd();
                    break;
            }
        }

        private void OnPatchSystemDownloadProgressUpdateEvent(object sender, PatchSystem_DownloadProgressUpdateEventArgs eventArgs)
        {
            int currentDownloadCount = eventArgs.CurrentDownloadCount;
            int totalDownloadCount = eventArgs.TotalDownloadCount;
            long currentDownloadSizeBytes = eventArgs.CurrentDownloadSizeBytes;
            long totalDownloadSizeBytes = eventArgs.TotalDownloadSizeBytes;

           
            _IPatchWnd.OnUpdateDownloadingProgress(currentDownloadCount, totalDownloadCount, currentDownloadSizeBytes, totalDownloadSizeBytes);
        }

        private struct DownloadFailedFileInfo
        {
            internal string FileName;
            internal string Error;
        }
    }
}