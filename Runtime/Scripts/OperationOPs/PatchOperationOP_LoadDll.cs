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

        private Dictionary<string, TextAsset> _assetDataDict = new();
        private List<Assembly> _hotFixAssemblies = new();

        internal override void OnEvent()
        {
            Debug.Log("[Attention] 看到此行信息代表你正在加载热更新代码！");
            LoadDll();
            OnLoadedDll();
        }

        private void LoadDll()
        {
            LoadMetaDataForAOTAssemblies();

            var hotFixAssemblyFileNames = EventBus_Patchs.PatchConfig.HotFixAssemblyFileNames;
            for (int i = 0; i <  hotFixAssemblyFileNames.Length; i++)
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
                Debug.Log("现在开始尝试运行热更代码");
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
            Debug.Log("准备实例化");
            GameObject go = obj.InstantiateSync();
            Debug.Log($"Prefab name is {go.name}");
        }

        #region Load MetaData
        private byte[] ReadBytesFromStreamingAssets(string dllName)
        {
            if (_assetDataDict.ContainsKey(dllName))
                return _assetDataDict[dllName].bytes;
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
                Debug.Log($"当前正在补充元数据的AOT程序集为: {aotDllName}. mode:{mode} ret:{err}");
            }
        }
        #endregion

        private void OnLoadedDll()
        {
            Debug.Log("[Attention] 看到此行信息代表加载热更新代码结束，但还没有被运行！");
            EventBus_Patchs.CallPatchOperationEvent(this, new PatchOperationEventArgs(PatchOperationEventCode.PatchDone));
        }
    }
}
