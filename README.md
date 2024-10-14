# Intro

This is a helper to make you using YooAsset easy by just by few step. With this helper, you no need to care the downloader, event, machine and etc. That`s wonderful!

# How to use?
## 1. Config the Patch Behaviour

1. Add a empty GameObject in Hierarchy.
2. Add component "SangoPatchRoot" in this GameObject and config it.
3. Focused on the field "Patch Config", that means you need set a ScriptbleObject on it. Trun to Project panel, choose a proper folder, then click right key with your mouse, find "SangoUtils/PatchConfig". After customize all the field, then set it to the field above.
4. If work well, a component "SangoPathEvent" will be autoAdd to this GameObject, you can drag event to it.

## 2. Customize the Patch UI

1. Add a Canvas to customize your own UI.
2. New a MonoBehaviour, and implement interface "ISangoPatchWnd".
3. Add this behaviour and set it to the component "SangoPatchRoot".

## 3. Work flow

1. Hybrid Compile ActiveBuildTarget && Generate All.
2. YooAsset Builder collect && build bundles.
3. Move all bundles to CDN.
4. Use this tool to load bundles.

## 4. Runtime API

```cs
public class AssetBundleService : MonoBehaviour
{
    //Use this API as Instance, it will auto set singleton.
    public static AssetBundleService Instance;
    //Download asset bundles anytime.
    public void DownloadAssetBundlesAsync(AssetBundleMessage_DownloadASync message);
    //Load asset Async;
    public void LoadAssetAsync<T>(string assetPath, Action<T> onAssetLoaded, bool isCache = true, uint priority = 0);
    //You also can get asset handle directly.
    public void GetAssetHandleAsync<T>(string assetPath, Action<AssetHandle> onHandleLoaded, uint priority = 0);
    //Release the handle.
    public void ReleaseAssetHandle(string assetPath);
    //If you lost the key, you can release all.
    public void ReleaseAll();
}
```

# Get stuck?

You can see the sample in Path "Runtime/Samples/SangoPatchRootSample".
