using YooAsset;

namespace SangoUtils.Patchs_YooAsset
{
    internal class PatchConfig
    {
        public string CDNPathAndroidEditor { get; set; }
        public string CDNPathIOSEditor { get; set; }
        public string CDNPathWebGLEditor { get; set; }
        public string CDNPathOthersEditor { get; set; }

        public string CDNPathAndroidRuntime { get; set; }
        public string CDNPathIOSRuntime { get; set; }
        public string CDNPathWebGLRuntime { get; set; }
        public string CDNPathOthersRuntime { get; set; }

        public string PackageName { get; set; }
        public string DefaultTag { get; set; }

        public int DefaultDownloadingMaxNumber { get; set; }
        public int DefaultFailedTryAgainCount { get; set; }
        public int DefaultTimeout { get; set; }

        public EPlayMode PlayMode { get; set; }
        public EDefaultBuildPipeline BuildPipeline { get; set; }
        public string PackageVersion { get; set; }
        public ResourceDownloaderOperation ResourceDownloaderOperation { get; set; }

        public string[] AOTAssemblyFileNames { get; set; }
        public string[] HotFixAssemblyFileNames { get; set; }
        public string HotFixRootPrefabPath { get; set; }
    }
}
