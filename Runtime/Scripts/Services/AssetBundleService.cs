using SangoUtils.Patchs_YooAsset.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace SangoUtils.Patchs_YooAsset
{
    public class AssetBundleService : MonoBehaviour
    {
        private static AssetBundleService _instance;
        private ResourcePackage _package;
        private Dictionary<string, AssetHandle> _assetHandleCacheDict = new();

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

        /// <summary>
        /// Start a async downloader manually.
        /// </summary>
        /// <param name="message">The param of this downloader.</param>
        public void DownloadAssetBundlesAsync(AssetBundleMessage_DownloadASync message)
        {
            var package = YooAssets.GetPackage(EventBus_Patchs.PatchConfig.PackageName);
            var downloader = package.CreateResourceDownloader(message.Tag, message.DownloadingMaxNumber, message.FailedTryAgainCount, message.Timeout);

            if (downloader.TotalDownloadCount > 0)
            {
                BeginDownloadAsync(message).Start();
            }
        }

        /// <summary>
        /// Load asset async, the asset will be cache inner this API.
        /// Be care, the assetPath will be the only key to clean the cache.
        /// </summary>
        /// <typeparam name="T">Should be a unity object.</typeparam>
        /// <param name="assetPath">Must be unique, should be full path same as config in YooAsset Editor Window.</param>
        /// <param name="onAssetLoaded">Received the result.</param>
        /// <param name="isCache">If true, must manual clean cache by call API: ReleaseAssetHandle</param>
        /// <param name="priority">The priority of this load, will use as if need dependence.</param>
        public void LoadAssetAsync<T>(string assetPath, Action<T> onAssetLoaded, bool isCache = true, uint priority = 0)
            where T : UnityEngine.Object
        {
            GetAssetHandleAsync<T>(assetPath, onAssetHandleFound, priority);

            void onAssetHandleFound(AssetHandle handle)
            {
                T res = handle.AssetObject as T;
                if (res != null)
                {
                    if (isCache)
                        _assetHandleCacheDict.Add(assetPath, handle);
                    else
                        handle.Release();

                    onAssetLoaded?.Invoke(res);
                }
            }
        }

        /// <summary>
        /// Get asset handle direct.
        /// </summary>
        /// <typeparam name="T">Should be a unity object.</typeparam>
        /// <param name="assetPath">Must be unique, should be full path same as config in YooAsset Editor Window.</param>
        /// <param name="onHandleLoaded">Receive the result</param>
        /// <param name="priority">The priority of this load, will use as if need dependence.</param>
        public void GetAssetHandleAsync<T>(string assetPath, Action<AssetHandle> onHandleLoaded, uint priority = 0)
             where T : UnityEngine.Object
        {
            AssetHandle handle;

            if (_assetHandleCacheDict.TryGetValue(assetPath, out handle))
            {
                onHandleLoaded?.Invoke(handle);
                return;
            }

            var package = YooAssets.GetPackage(EventBus_Patchs.PatchConfig.PackageName);
            if (package != null)
            {
                handle = package.LoadAssetAsync<T>(assetPath, priority);
                handle.Completed += onHandleLoaded;
            }
        }

        /// <summary>
        /// Release the target asset handle by assetPath.
        /// </summary>
        /// <param name="assetPath">The key to find target.</param>
        public void ReleaseAssetHandle(string assetPath)
        {
            if (_assetHandleCacheDict.TryGetValue(assetPath, out var handle))
            {
                handle.Release();
                _assetHandleCacheDict.Remove(assetPath);
            }
        }

        /// <summary>
        /// Release all target which in cache.
        /// </summary>
        public void ReleaseAll()
        {
            foreach (var pair in _assetHandleCacheDict)
                pair.Value.Release();

            _assetHandleCacheDict.Clear();
        }

        private IEnumerator BeginDownloadAsync(AssetBundleMessage_DownloadASync message)
        {
            var downloaderOperation = EventBus_Patchs.PatchConfig.ResourceDownloaderOperation;
            downloaderOperation.OnDownloadProgressCallback = delegate (int totalDownloadCount, int currentDownloadCount, long totalDownloadSizeBytes, long currentDownloadSizeBytes)
            {
                message.OnDownloadProgressedCallback.Invoke(totalDownloadCount, currentDownloadCount, totalDownloadSizeBytes, currentDownloadSizeBytes);
            };
            downloaderOperation.OnDownloadErrorCallback = delegate (string fileName, string error)
            {
                message.OnDownloadErrorCallback.Invoke(fileName, error);
            };
            downloaderOperation.BeginDownload();
            yield return downloaderOperation;

            if (downloaderOperation.Status != EOperationStatus.Succeed)
            {
                message.OnDownloadDoneCallback.Invoke(false);
                yield break;
            }

            message.OnDownloadDoneCallback(true);
        }
    }
}