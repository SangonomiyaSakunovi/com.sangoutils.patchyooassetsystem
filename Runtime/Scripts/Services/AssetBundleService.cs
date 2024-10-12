using System;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace SangoUtils.Patchs_YooAsset
{
    public class AssetBundleService : MonoBehaviour
    {
        private static AssetBundleService _instance;
        private ResourcePackage _package;

        public static AssetBundleService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(AssetBundleService)) as AssetBundleService;
                    if (_instance == null)
                    {
                        GameObject gameObject = new GameObject("[" + typeof(AssetBundleService).FullName + "]");
                        _instance = gameObject.AddComponent<AssetBundleService>();
                        gameObject.hideFlags = HideFlags.HideInHierarchy;
                        DontDestroyOnLoad(gameObject);
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (null != _instance && _instance != this)
            {
                Destroy(gameObject);
            }
        }

        private List<AssetHandle> _cacheAssetHandles = new();

        public void DownloadAssetBundlesAsync(string tag, int downloadingMaxNumber, int failedTryAgain, int timeout = 60,
            Action<int, int, long, long> downloadingProgressed = null)
        {
            var package = YooAssets.GetPackage(EventBus_Patchs.PatchConfig.PackageName);
            var downloader = package.CreateResourceDownloader(tag, downloadingMaxNumber, failedTryAgain, timeout);

            if (downloader.TotalDownloadCount > 0)
            {
                //TODO
            }
        }

        public void LoadAssetAsync<T>(string assetPath, Action<T> onAssetLoaded, bool isCache = true)
            where T : UnityEngine.Object
        {
            var package = YooAssets.GetPackage(EventBus_Patchs.PatchConfig.PackageName);
            if (package != null)
            {
                var handle = package.LoadAssetAsync<T>(assetPath);
                handle.Completed += (handle) =>
                {
                    T res = handle.AssetObject as T;
                    if (res != null)
                    {
                        if (!isCache)
                            GCAssetHandleTODO(handle);
                        onAssetLoaded?.Invoke(res);
                    }
                };
            }

            void GCAssetHandleTODO(AssetHandle assetHandle)
            {
                _cacheAssetHandles.Add(assetHandle);
            }
        }

        public void ReleaseAssetHandles()
        {
            for (int i = 0; i < _cacheAssetHandles.Count; i++)
            {
                _cacheAssetHandles[i].Release();
            }
            _cacheAssetHandles.Clear();
        }
    }
}