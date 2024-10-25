using HybridCLR;
using SangoUtils.Patchs_YooAsset.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using YooAsset;

namespace SangoUtils.Patchs_YooAsset
{
    internal class PatchOperationOP_LoadDll : PatchOperationOP_Base
    {
        internal override PatchOperationEventCode PatchOperationEventCode => PatchOperationEventCode.LoadDll;

        private Dictionary<string, byte[]> _assetDataDict = new();
        private List<Assembly> _hotFixAssemblies = new();

        internal override void OnEvent()
        {
            LoadAssetAsync().Start();
        }

        private IEnumerator LoadAssetAsync()
        {
            var package = YooAssets.GetPackage(EventBus_Patchs.PatchConfig.PackageName);

            foreach (var aotAssetName in EventBus_Patchs.PatchConfig.AOTAssemblyFileNames)
            {
                var handle1 = package.LoadRawFileAsync(aotAssetName);
                yield return handle1;
                byte[] bytes1 = handle1.GetRawFileData();
                if (!_assetDataDict.ContainsKey(aotAssetName))
                    _assetDataDict.Add(aotAssetName, bytes1);
            }

            foreach (var hotFixAssetName in EventBus_Patchs.PatchConfig.HotFixAssemblyFileNames)
            {
                var handle2 = package.LoadRawFileAsync(hotFixAssetName);
                yield return handle2;
                byte[] bytes2 = handle2.GetRawFileData();
                if (!_assetDataDict.ContainsKey(hotFixAssetName))
                    _assetDataDict.Add(hotFixAssetName, bytes2);
            }

            LoadDll();
        }

        private void LoadDll()
        {
            LoadMetaDataForAOTAssemblies();

            var hotFixAssemblyFileNames = EventBus_Patchs.PatchConfig.HotFixAssemblyFileNames;
            for (int i = 0; i < hotFixAssemblyFileNames.Length; i++)
            {
#if UNITY_EDITOR
                Assembly assembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(assem => (assem.GetName().Name + ".dll") == hotFixAssemblyFileNames[i]);
                _hotFixAssemblies.Add(assembly);
#else
                byte[] bytes = ReadBytesFromStreamingAssets(hotFixAssemblyFileNames[i]);
                Assembly assembly = Assembly.Load(bytes);
                _hotFixAssemblies.Add(assembly);
#endif
                BeginInstantiateComponentByAssetASync().Start();
            }
        }

        private IEnumerator BeginInstantiateComponentByAssetASync()
        {
            var package = YooAssets.GetPackage(EventBus_Patchs.PatchConfig.PackageName);
            var handle = package.LoadAssetAsync<GameObject>(EventBus_Patchs.PatchConfig.HotFixRootPrefabPath);
            yield return handle;
            handle.Completed += Handle_Completed;
        }

        private void Handle_Completed(AssetHandle obj)
        {
            GameObject go = obj.InstantiateSync();
            OnLoadedDll();
        }

        #region Load MetaData
        private byte[] ReadBytesFromStreamingAssets(string dllName)
        {
            if (_assetDataDict.ContainsKey(dllName))
                return _assetDataDict[dllName];
            else
                return Array.Empty<byte>();
        }

        private void LoadMetaDataForAOTAssemblies()
        {
            //Only the AOT Assemblies need Meta Data, the HotFix Assemblies already had all Metas.
            HomologousImageMode mode = HomologousImageMode.SuperSet;
            var AOTAssemblyFileNames = EventBus_Patchs.PatchConfig.AOTAssemblyFileNames;
            foreach (var aotDllName in AOTAssemblyFileNames)
            {
                byte[] dllBytes = ReadBytesFromStreamingAssets(aotDllName);
                LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
            }
        }
        #endregion

        private void OnLoadedDll()
        {
            EventBus_Patchs.CallPatchOperationEvent(this, new PatchOperationEventArgs(PatchOperationEventCode.PatchDone));
        }
    }
}
