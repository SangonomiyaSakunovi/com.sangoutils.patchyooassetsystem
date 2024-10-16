using System;

namespace SangoUtils.Patchs_YooAsset
{
    internal class EventBus_Patchs
    {
        private static EventBus_Patchs _instance;

        public EventBus_Patchs() { _instance = this; }

        private PatchConfig _patchConfig;
        internal static PatchConfig PatchConfig { get => _instance._patchConfig; set => _instance._patchConfig ??= value; }

        private PatchEvent _patchEvent;
        internal static void SetCustomPatchEvent(PatchEvent patchEvent) { _instance._patchEvent = patchEvent; }
        internal static void CallCustomPatchEvent(CustomPatchEventCode code) { _instance._patchEvent.CallEvent(code); }

        private event Action<object, PatchSystemEventArgs> _patchSystemEvent;
        public static void AddPatchSystemEvent(Action<object, PatchSystemEventArgs> action) { _instance._patchSystemEvent += action; }
        public static void RemovePatchSystemEvent(Action<object, PatchSystemEventArgs> action) { _instance._patchSystemEvent -= action; }
        public static void CallPatchSystemEvent(object sender, PatchSystemEventArgs eventArgs) { _instance._patchSystemEvent?.Invoke(sender, eventArgs); }

        private event Action<object, PatchUserEventArgs> _patchUserEvent;
        public static void AddPatchUserEvent(Action<object, PatchUserEventArgs> action) { _instance._patchUserEvent += action; }
        public static void RemovePatchUserEvent(Action<object, PatchUserEventArgs> action) { _instance._patchUserEvent -= action; }
        public static void CallPatchUserEvent(object sender, PatchUserEventArgs eventArgs) { _instance._patchUserEvent?.Invoke(sender, eventArgs); }

        private event Action<object, PatchOperationEventArgs> _patchOperationEvent;
        public static void AddPatchOperationEvent(Action<object, PatchOperationEventArgs> action) { _instance._patchOperationEvent += action; }
        public static void RemovePatchOperationEvent(Action<object, PatchOperationEventArgs> action) { _instance._patchOperationEvent -= action; }
        public static void CallPatchOperationEvent(object sender, PatchOperationEventArgs eventArgs) { _instance._patchOperationEvent?.Invoke(sender, eventArgs); }

        private event Action<object, PatchSystem_DownloadProgressUpdateEventArgs> _patchSystem_DownloadProgressUpdateEvent;
        public static void AddPatchSystem_DownloadProgressUpdateEvent(Action<object, PatchSystem_DownloadProgressUpdateEventArgs> action) { _instance._patchSystem_DownloadProgressUpdateEvent += action; }
        public static void RemovePatchSystem_DownloadProgressUpdateEvent(Action<object, PatchSystem_DownloadProgressUpdateEventArgs> action) { _instance._patchSystem_DownloadProgressUpdateEvent -= action; }
        public static void CallPatchSystem_DownloadProgressUpdateEvent(object sender, PatchSystem_DownloadProgressUpdateEventArgs eventArgs) { _instance._patchSystem_DownloadProgressUpdateEvent?.Invoke(sender, eventArgs); }
    }
}
