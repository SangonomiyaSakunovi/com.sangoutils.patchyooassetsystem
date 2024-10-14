using SangoUtils.Patchs_YooAsset.Utils;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using YooAsset;

namespace SangoUtils.Patchs_YooAsset
{
    [RequireComponent(typeof(SangoPatchEvent))]
    public class SangoPatchRoot : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour _patchWnd;
        [SerializeField] private SangoPatchConfig _patchConfig;

        private ISangoPatchWnd _sangoPatchWnd;

        private void Awake()
        {
            new EventBus_Patchs();

            var cfg = new PatchConfig();
            SetConfig(ref cfg);
            EventBus_Patchs.PatchConfig = cfg;
            EventBus_Patchs.SetCustomPatchEvent(GetComponent<SangoPatchEvent>());

            EventBus_Patchs.AddPatchSystemEvent(OnPatchSystemEvent);
            EventBus_Patchs.AddPatchSystem_DownloadProgressUpdateEvent(OnPatchSystemDownloadProgressUpdateEvent);

            if (_patchWnd.GetType().GetInterfaces().Any(iface => iface == typeof(ISangoPatchWnd)))
            {
                _sangoPatchWnd = (ISangoPatchWnd)_patchWnd;
                _sangoPatchWnd.OnInit(this);
            }
            else
            {
                Debug.LogError("No Patch Wnd Found! You need set it!");
            }
        }

        private void Start()
        {
            StartOperationASync().Start();
            _sangoPatchWnd.OnStart();
        }

        /// <summary>
        /// TODO: Only for test.
        /// </summary>
        public void ReStartDefaultHotFix()
        {
            StartOperationASync().Start();
            _sangoPatchWnd.OnStart();
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
                    _sangoPatchWnd.OnShowMessageBox($"Failed to initialize package !", callback);
                    break;
                case PatchSystemEventCode.PatchStatesChange:
                    string tips = eventArgs.ExtensionData[0].ToString();
                    _sangoPatchWnd.OnUpdateTips(tips);
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
                    _sangoPatchWnd.OnShowMessageBox($"Found update patch files, Total count {totalCount} Total szie {totalSizeMB}MB", callback1);
                    break;
                case PatchSystemEventCode.PackageVersionUpdateFailed:
                    Action callback2 = delegate
                    {
                        EventBus_Patchs.CallPatchUserEvent(this, new PatchUserEventArgs(PatchUserEventCode.UserTryUpdatePackageVersion));
                    };
                    _sangoPatchWnd.OnShowMessageBox($"Failed to update static version, please check the network status.", callback2);
                    break;
                case PatchSystemEventCode.PatchManifestUpdateFailed:
                    Action callback3 = delegate
                    {
                        EventBus_Patchs.CallPatchUserEvent(this, new PatchUserEventArgs(PatchUserEventCode.UserTryUpdatePatchManifest));
                    };
                    _sangoPatchWnd.OnShowMessageBox($"Failed to update patch manifest, please check the network status.", callback3);
                    break;
                case PatchSystemEventCode.WebFileDownloadFailed:
                    string fileName = eventArgs.ExtensionData[0].ToString();
                    string Error = eventArgs.ExtensionData[1].ToString();
                    Action callback4 = delegate
                    {
                        EventBus_Patchs.CallPatchUserEvent(this, new PatchUserEventArgs(PatchUserEventCode.UserTryDownloadWebFiles));
                    };
                    _sangoPatchWnd.OnShowMessageBox($"Failed to download file : {fileName}", callback4);
                    break;
                case PatchSystemEventCode.OnPatchEnd:
                    _sangoPatchWnd.OnEnd();
                    break;
            }
        }

        private void OnPatchSystemDownloadProgressUpdateEvent(object sender, PatchSystem_DownloadProgressUpdateEventArgs eventArgs)
        {
            int totalDownloadCount = eventArgs.TotalDownloadCount;
            int currentDownloadCount = eventArgs.CurrentDownloadCount;
            long totalDownloadSizeBytes = eventArgs.TotalDownloadSizeBytes;
            long currentDownloadSizeBytes = eventArgs.CurrentDownloadSizeBytes;
            float sliderValue = (float)currentDownloadCount / totalDownloadCount;
            _sangoPatchWnd.OnUpdateSliderValue(sliderValue);
            string currentSizeMB = (currentDownloadSizeBytes / 1048576f).ToString("f1");
            string totalSizeMB = (totalDownloadSizeBytes / 1048576f).ToString("f1");
            string tips = $"{currentDownloadCount}/{totalDownloadCount} {currentSizeMB}MB/{totalSizeMB}MB";
            _sangoPatchWnd.OnUpdateTips(tips);
        }
    }
}