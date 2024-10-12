using UnityEngine;
using UnityEngine.Events;
using YooAsset;

namespace SangoUtils.Patchs_YooAsset
{
    public class SangoPatchEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onUpdaterDone;

        public UnityEvent OnUpdaterDone { get => _onUpdaterDone; }
    }
}
