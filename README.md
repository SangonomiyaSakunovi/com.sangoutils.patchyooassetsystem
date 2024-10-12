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

## 3. Have a Try

# Get stuck?

You can see the sample in Path "Runtime/Samples/SangoPatchRootSample".
