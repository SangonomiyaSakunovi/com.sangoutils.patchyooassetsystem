using UnityEngine;
using YooAsset;

namespace SangoUtils.Patchs_YooAsset
{
    [CreateAssetMenu(menuName = "SangoUtils/PatchConfig")]
    internal class PatchConfigObj : ScriptableObject
    {
        [SerializeField] private string _cdnPathAndroidEditor = "https://127.0.0.1:8080/CDN/SampleApp/Android/1.0.0";
        [SerializeField] private string _cdnPathIosEditor = "https://127.0.0.1:8080/CDN/SampleApp/IOS/1.0.0";
        [SerializeField] private string _cdnPathWebglEditor = "https://127.0.0.1:8080/CDN/SampleApp/WebGL/1.0.0";
        [SerializeField] private string _cdnPathOthersEditor = "https://127.0.0.1:8080/CDN/SampleApp/Other/1.0.0";

        [SerializeField] private string _cdnPathAndroidRuntime = "https://127.0.0.1:8081/CDN/SampleApp/Android/1.0.0";
        [SerializeField] private string _cdnPathIosRuntime = "https://127.0.0.1:8081/CDN/SampleApp/IOS/1.0.0";
        [SerializeField] private string _cdnPathWebglRuntime = "https://127.0.0.1:8081/CDN/SampleApp/WebGL/1.0.0";
        [SerializeField] private string _cdnPathOthersRuntime = "https://127.0.0.1:8081/CDN/SampleApp/Other/1.0.0";

        [SerializeField] private string _packageName = "DefaultPackage";
        [SerializeField] private string _defaultTag = "DefaultTag";

        [SerializeField] private int _defaultDownloadingMaxNumber = 10;
        [SerializeField] private int _defaultFailedTryAgainCount = 3;
        [SerializeField] private int _defaultTimeout = 60;

        [SerializeField] private EPlayMode _playMode = EPlayMode.HostPlayMode;
        [SerializeField] private EDefaultBuildPipeline _buildPipeline = EDefaultBuildPipeline.BuiltinBuildPipeline;

        [SerializeField] private string[] _aotAssemblyFileNames = new string[] { "mscorlib.dll", "System.dll", "System.Core.dll" };
        [SerializeField] private string[] _hotFixAssemblyFileNames = new string[] { "HotFix.dll" };
        [SerializeField] private string _hotFixRootPrefabPath = "HotFixGameRoot";

        public string CDNPathAndroidEditor { get => _cdnPathAndroidEditor; }
        public string CDNPathIOSEditor { get => _cdnPathAndroidEditor; }
        public string CDNPathWebGLEditor { get => _cdnPathWebglEditor; }
        public string CDNPathOthersEditor { get => _cdnPathOthersEditor; }

        public string CDNPathAndroidRuntime { get => _cdnPathAndroidRuntime; }
        public string CDNPathIOSRuntime { get => _cdnPathAndroidRuntime; }
        public string CDNPathWebGLRuntime { get => _cdnPathWebglRuntime; }
        public string CDNPathOthersRuntime { get => _cdnPathOthersRuntime; }

        public string PackageName { get => _packageName; }
        public string DefaultTag { get => _defaultTag; }

        public int DefaultDownloadingMaxNumber { get => _defaultDownloadingMaxNumber; }
        public int DefaultFailedTryAgainCount {  get => _defaultFailedTryAgainCount; }
        public int DefaultTimeout { get => _defaultTimeout; }

        public EPlayMode PlayMode { get => _playMode; }
        public EDefaultBuildPipeline BuildPipeline { get => _buildPipeline; }

        public string[] AOTAssemblyFileNames { get => _aotAssemblyFileNames; }
        public string[] HotFixAssemblyFileNames { get => _hotFixAssemblyFileNames; }
        public string HotFixRootPrefabPath { get => _hotFixRootPrefabPath; }
    }
}
