using UnityEditor;

namespace SangoUtils.Patchs_YooAsset
{
    [CustomEditor(typeof(PatchConfigObj))]
    internal class SangoPatchConfigInspector : Editor
    {
        private SerializedProperty _cdnPathAndroidEditor;
        private SerializedProperty _cdnPathIosEditor;
        private SerializedProperty _cdnPathWebglEditor;
        private SerializedProperty _cdnPathOthersEditor;

        private SerializedProperty _cdnPathAndroidRuntime;
        private SerializedProperty _cdnPathIosRuntime;
        private SerializedProperty _cdnPathWebglRuntime;
        private SerializedProperty _cdnPathOthersRuntime;

        private SerializedProperty _packageName;
        private SerializedProperty _defaultTag;

        private SerializedProperty _defaultDownloadingMaxNumber;
        private SerializedProperty _defaultFailedTryAgainCount;
        private SerializedProperty _defaultTimeout;

        private SerializedProperty _playMode;
        private SerializedProperty _buildPipeline;

        private SerializedProperty _aotAssemblyFileNames;
        private SerializedProperty _hotFixAssemblyFileNames;
        private SerializedProperty _hotFixRootPrefabPath;

        private void OnEnable()
        {
            serializedObject.Update();

            _cdnPathAndroidEditor = serializedObject.FindProperty("_cdnPathAndroidEditor");
            _cdnPathIosEditor = serializedObject.FindProperty("_cdnPathIosEditor");
            _cdnPathWebglEditor = serializedObject.FindProperty("_cdnPathWebglEditor");
            _cdnPathOthersEditor = serializedObject.FindProperty("_cdnPathOthersEditor");

            _cdnPathAndroidRuntime = serializedObject.FindProperty("_cdnPathAndroidRuntime");
            _cdnPathIosRuntime = serializedObject.FindProperty("_cdnPathIosRuntime");
            _cdnPathWebglRuntime = serializedObject.FindProperty("_cdnPathWebglRuntime");
            _cdnPathOthersRuntime = serializedObject.FindProperty("_cdnPathOthersRuntime");

            _packageName = serializedObject.FindProperty("_packageName");
            _defaultTag = serializedObject.FindProperty("_defaultTag");

            _defaultDownloadingMaxNumber = serializedObject.FindProperty("_defaultDownloadingMaxNumber");
            _defaultFailedTryAgainCount = serializedObject.FindProperty("_defaultFailedTryAgainCount");
            _defaultTimeout = serializedObject.FindProperty("_defaultTimeout");

            _playMode = serializedObject.FindProperty("_playMode");
            _buildPipeline = serializedObject.FindProperty("_buildPipeline");

            _aotAssemblyFileNames = serializedObject.FindProperty("_aotAssemblyFileNames");
            _hotFixAssemblyFileNames = serializedObject.FindProperty("_hotFixAssemblyFileNames");
            _hotFixRootPrefabPath = serializedObject.FindProperty("_hotFixRootPrefabPath");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Description: This is a config for YooAsset&HybridCLR Settings.");
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("These fields using for local test.", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_cdnPathAndroidEditor);
            EditorGUILayout.PropertyField(_cdnPathIosEditor);
            EditorGUILayout.PropertyField(_cdnPathWebglEditor);
            EditorGUILayout.PropertyField(_cdnPathOthersEditor);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("[Warnning] These fields using for online, check it carefully.", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_cdnPathAndroidRuntime);
            EditorGUILayout.PropertyField(_cdnPathIosRuntime);
            EditorGUILayout.PropertyField(_cdnPathWebglRuntime);
            EditorGUILayout.PropertyField(_cdnPathOthersRuntime);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("You need make sure this config will no conflicts with yooConfig in Editor Window.", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_packageName);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Assets with this tag will be download in Game Start.", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_defaultTag);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("'Maximum file number', 'Try count' and 'Timeout' of downloads at the same time by default.", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_defaultDownloadingMaxNumber);
            EditorGUILayout.PropertyField(_defaultFailedTryAgainCount);
            EditorGUILayout.PropertyField(_defaultTimeout);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("YooAsset State, see the Document.", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_playMode);
            EditorGUILayout.PropertyField(_buildPipeline);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("The Hybrid Settings, you need separate all assemblies into AOTs and HotFixs.", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_aotAssemblyFileNames);
            EditorGUILayout.PropertyField(_hotFixAssemblyFileNames);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("This GameObject will use to load HotFix Assemblies async.", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_hotFixRootPrefabPath);

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}
