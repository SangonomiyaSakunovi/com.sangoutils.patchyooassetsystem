using UnityEngine;
using UnityEngine.Events;

namespace SangoUtils.Patchs_YooAsset
{
    public class PatchEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onPatchDone = new();
        [SerializeField] private UnityEvent _onPatchFailed = new();

        public UnityEvent OnPatchDone { get => _onPatchDone; }
        public UnityEvent OnPatchFailed { get => _onPatchFailed; }

        internal void CallEvent(CustomPatchEventCode code)
        {
            switch (code)
            {
                case CustomPatchEventCode.PatchDone:
                    _onPatchDone?.Invoke(); break;
                case CustomPatchEventCode.PatchFailed:
                    _onPatchFailed?.Invoke(); break;
            }
        }
    }
}
