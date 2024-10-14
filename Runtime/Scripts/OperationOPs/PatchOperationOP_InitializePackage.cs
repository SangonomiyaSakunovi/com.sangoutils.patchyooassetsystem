using SangoUtils.Patchs_YooAsset.Utils;
using System.Collections;
using System.IO;
using UnityEngine;
using YooAsset;

namespace SangoUtils.Patchs_YooAsset
{
    internal class PatchOperationOP_InitializePackage : PatchOperationOP_Base
    {
        internal override PatchOperationEventCode PatchOperationEventCode => PatchOperationEventCode.InitializePackage;

        internal override void OnEvent()
        {
            InitPackageASync().Start();
        }

        private IEnumerator InitPackageASync()
        {
            EPlayMode playMode = EventBus_Patchs.PatchConfig.PlayMode;
            string packageName = EventBus_Patchs.PatchConfig.PackageName;
            string buildPipeline = EventBus_Patchs.PatchConfig.BuildPipeline.ToString();

            var package = YooAssets.TryGetPackage(packageName);
            if (package == null)
                package = YooAssets.CreatePackage(packageName);

            InitializationOperation initializationOperation = null;
            if (playMode == EPlayMode.EditorSimulateMode)
            {
                var simulateBuildResult = EditorSimulateModeHelper.SimulateBuild(buildPipeline, packageName);
                var createParameters = new EditorSimulateModeParameters();
                createParameters.EditorFileSystemParameters = FileSystemParameters.CreateDefaultEditorFileSystemParameters(simulateBuildResult);
                initializationOperation = package.InitializeAsync(createParameters);
            }

            if (playMode == EPlayMode.OfflinePlayMode)
            {
                var createParameters = new OfflinePlayModeParameters();
                createParameters.BuildinFileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
                initializationOperation = package.InitializeAsync(createParameters);
            }

            if (playMode == EPlayMode.HostPlayMode)
            {
                string defaultHostServer = GetHostServerURL();
                string fallbackHostServer = GetHostServerURL();
                IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                var createParameters = new HostPlayModeParameters();
                createParameters.BuildinFileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
                createParameters.CacheFileSystemParameters = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
                initializationOperation = package.InitializeAsync(createParameters);
            }

            if (playMode == EPlayMode.WebPlayMode)
            {
                var createParameters = new WebPlayModeParameters();

#if UNITY_WEBGL && WEIXINMINIGAME && !UNITY_EDITOR
			    string defaultHostServer = GetHostServerURL();
                string fallbackHostServer = GetHostServerURL();
                IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                createParameters.WebFileSystemParameters = WechatFileSystemCreater.CreateWechatFileSystemParameters(remoteServices);
#else
                createParameters.WebFileSystemParameters = FileSystemParameters.CreateDefaultWebFileSystemParameters();
#endif
                initializationOperation = package.InitializeAsync(createParameters);
            }

            yield return initializationOperation;

            if (initializationOperation.Status != EOperationStatus.Succeed)
            {
                Debug.LogWarning($"{initializationOperation.Error}");
                EventBus_Patchs.CallPatchSystemEvent(this, new PatchSystemEventArgs(PatchSystemEventCode.InitializeFailed));
            }
            else
            {
                EventBus_Patchs.CallPatchOperationEvent(this, new PatchOperationEventArgs(PatchOperationEventCode.UpdatePackageVersion));
            }
        }

        private string GetHostServerURL()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
                return EventBus_Patchs.PatchConfig.CDNPathAndroidEditor;
            else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
                return EventBus_Patchs.PatchConfig.CDNPathIOSEditor;
            else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
                return EventBus_Patchs.PatchConfig.CDNPathWebGLEditor;
            else
                return EventBus_Patchs.PatchConfig.CDNPathOthersEditor;
#else
		if (Application.platform == RuntimePlatform.Android)
			return EventBus_Patchs.PatchConfig.CDNPathAndroidRuntime;
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
			return EventBus_Patchs.PatchConfig.CDNPathIOSRuntime;
		else if (Application.platform == RuntimePlatform.WebGLPlayer)
			return EventBus_Patchs.PatchConfig.CDNPathWebGLRuntime;
		else
			return EventBus_Patchs.PatchConfig.CDNPathOthersRuntime;
#endif
        }

        private class RemoteServices : IRemoteServices
        {
            private readonly string _defaultHostServer;
            private readonly string _fallbackHostServer;

            public RemoteServices(string defaultHostServer, string fallbackHostServer)
            {
                _defaultHostServer = defaultHostServer;
                _fallbackHostServer = fallbackHostServer;
            }
            string IRemoteServices.GetRemoteMainURL(string fileName)
            {
                return $"{_defaultHostServer}/{fileName}";
            }
            string IRemoteServices.GetRemoteFallbackURL(string fileName)
            {
                return $"{_fallbackHostServer}/{fileName}";
            }
        }

        private class FileStreamDecryption : IDecryptionServices
        {
            // <summary>
            /// 同步方式获取解密的资源包对象
            /// 注意：加载流对象在资源包对象释放的时候会自动释放
            /// </summary>
            AssetBundle IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo, out Stream managedStream)
            {
                BundleStream bundleStream = new BundleStream(fileInfo.FileLoadPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                managedStream = bundleStream;
                return AssetBundle.LoadFromStream(bundleStream, fileInfo.FileLoadCRC, GetManagedReadBufferSize());
            }

            /// <summary>
            /// 异步方式获取解密的资源包对象
            /// 注意：加载流对象在资源包对象释放的时候会自动释放
            /// </summary>
            AssetBundleCreateRequest IDecryptionServices.LoadAssetBundleAsync(DecryptFileInfo fileInfo, out Stream managedStream)
            {
                BundleStream bundleStream = new BundleStream(fileInfo.FileLoadPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                managedStream = bundleStream;
                return AssetBundle.LoadFromStreamAsync(bundleStream, fileInfo.FileLoadCRC, GetManagedReadBufferSize());
            }

            /// <summary>
            /// 获取解密的字节数据
            /// </summary>
            byte[] IDecryptionServices.ReadFileData(DecryptFileInfo fileInfo)
            {
                throw new System.NotImplementedException();
            }

            /// <summary>
            /// 获取解密的文本数据
            /// </summary>
            string IDecryptionServices.ReadFileText(DecryptFileInfo fileInfo)
            {
                throw new System.NotImplementedException();
            }

            private static uint GetManagedReadBufferSize()
            {
                return 1024;
            }
        }

        private class FileOffsetDecryption : IDecryptionServices
        {
            /// <summary>
            /// 同步方式获取解密的资源包对象
            /// 注意：加载流对象在资源包对象释放的时候会自动释放
            /// </summary>
            AssetBundle IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo, out Stream managedStream)
            {
                managedStream = null;
                return AssetBundle.LoadFromFile(fileInfo.FileLoadPath, fileInfo.FileLoadCRC, GetFileOffset());
            }

            /// <summary>
            /// 异步方式获取解密的资源包对象
            /// 注意：加载流对象在资源包对象释放的时候会自动释放
            /// </summary>
            AssetBundleCreateRequest IDecryptionServices.LoadAssetBundleAsync(DecryptFileInfo fileInfo, out Stream managedStream)
            {
                managedStream = null;
                return AssetBundle.LoadFromFileAsync(fileInfo.FileLoadPath, fileInfo.FileLoadCRC, GetFileOffset());
            }

            /// <summary>
            /// 获取解密的字节数据
            /// </summary>
            byte[] IDecryptionServices.ReadFileData(DecryptFileInfo fileInfo)
            {
                throw new System.NotImplementedException();
            }

            /// <summary>
            /// 获取解密的文本数据
            /// </summary>
            string IDecryptionServices.ReadFileText(DecryptFileInfo fileInfo)
            {
                throw new System.NotImplementedException();
            }

            private static ulong GetFileOffset()
            {
                return 32;
            }
        }
    }

    public class BundleStream : FileStream
    {
        public const byte KEY = 64;

        public BundleStream(string path, FileMode mode, FileAccess access, FileShare share) : base(path, mode, access, share)
        {
        }
        public BundleStream(string path, FileMode mode) : base(path, mode)
        {
        }

        public override int Read(byte[] array, int offset, int count)
        {
            var index = base.Read(array, offset, count);
            for (int i = 0; i < array.Length; i++)
            {
                array[i] ^= KEY;
            }
            return index;
        }
    }
}