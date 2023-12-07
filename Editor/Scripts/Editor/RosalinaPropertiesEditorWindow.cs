#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class RosalinaPropertiesEditorWindow : EditorWindow
{
    [SerializeField] private VisualTreeAsset _visualTreeAsset;

    private RosalinaFileSetting _fileSettings = null;

    public VisualElement Container { get; private set; }

    public VisualElement BasicSettingsContainer { get; private set; }

    public Toggle EnableFile { get; private set; }

    public EnumField GeneratorTypeSelector { get; private set; }

    public TextField GeneratedPath { get; private set; }

    public TextField Namespace { get; private set; }

    public TextField FilePrefix { get; private set; }

    public TextField FileSuffix { get; private set; }

    public Button GenerateBindingsButton { get; private set; }

    public Button GenerateScriptButton { get; private set; }

    public Button ClearBindingsButton { get; private set; }

    private void OnEnable()
    {
    }

    private void OnDestroy()
    {
        EnableFile.UnregisterValueChangedCallback(OnEnableFileChanged);
        GeneratorTypeSelector.UnregisterValueChangedCallback(OnGeneratorTypeSelectionChanged);
        GenerateBindingsButton.clicked -= OnGenerateBindings;
        GenerateScriptButton.clicked -= OnGenerateScripts;
        ClearBindingsButton.clicked -= OnClearBindings;
    }

    private void CreateGUI()
    {
        rootVisualElement.Add(_visualTreeAsset.Instantiate());

        Container = rootVisualElement.Q<VisualElement>("Container");
        BasicSettingsContainer = rootVisualElement.Q<VisualElement>("BasicSettingsContainer");
        EnableFile = rootVisualElement.Q<Toggle>("EnableFile");
        GeneratorTypeSelector = rootVisualElement.Q<EnumField>("GeneratorTypeSelector");
        GeneratedPath = rootVisualElement.Q<TextField>("GeneratedPath");
        Namespace = rootVisualElement.Q<TextField>("Namespace");
        FilePrefix = rootVisualElement.Q<TextField>("FilePrefix");
        FileSuffix = rootVisualElement.Q<TextField>("FileSuffix");
        GenerateBindingsButton = rootVisualElement.Q<Button>("GenerateBindingsButton");
        GenerateScriptButton = rootVisualElement.Q<Button>("GenerateScriptButton");
        ClearBindingsButton = rootVisualElement.Q<Button>("ClearBindingsButton");

        OnSelectionChange();
        EnableFile.RegisterValueChangedCallback(OnEnableFileChanged);
        GeneratorTypeSelector.RegisterValueChangedCallback(OnGeneratorTypeSelectionChanged);
        GeneratedPath.RegisterValueChangedCallback(OnPathChanged);
        Namespace.RegisterValueChangedCallback(OnNamespaceChanged);
        FilePrefix.RegisterValueChangedCallback(OnFilePrefixChanged);
        FileSuffix.RegisterValueChangedCallback(OnFileSuffixChanged);
        GenerateBindingsButton.clicked += OnGenerateBindings;
        GenerateScriptButton.clicked += OnGenerateScripts;
        ClearBindingsButton.clicked += OnClearBindings;
    }

    private void OnSelectionChange()
    {
        bool isActive = Selection.activeObject != null && Selection.activeObject.GetType() == typeof(VisualTreeAsset);

        Container.SetEnabled(isActive);

        if (isActive)
        {
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            _fileSettings = RosalinaSettings.instance.GetFileSetting(assetPath);
        }
        else
        {
            _fileSettings = null;
        }

        RefreshFileSettings();
    }

    private void RefreshFileSettings()
    {
        EnableFile.SetValueWithoutNotify(_fileSettings != null);
        BasicSettingsContainer.SetEnabled(_fileSettings != null);

        if (_fileSettings != null)
        {
            GeneratorTypeSelector.value = _fileSettings.Type;
            GeneratedPath.value = _fileSettings.GeneratedPath;
            Namespace.value = _fileSettings.Namespace;
            FilePrefix.value = _fileSettings.FilePrefix;
            FileSuffix.value = _fileSettings.FileSuffix;
        }
        else
        {
            GeneratorTypeSelector.value = null;
            GeneratedPath.value = null;
            Namespace.value = null;
            FilePrefix.value = null;
            FileSuffix.value = null;
        }
    }

    private void OnEnableFileChanged(ChangeEvent<bool> @event)
    {
        if (@event.newValue)
        {
            _fileSettings = new RosalinaFileSetting
            {
                Path = AssetDatabase.GetAssetPath(Selection.activeObject),
                Type = RosalinaGenerationType.Document,
                GeneratedPath = RosalinaSettings.instance.DefaultGeneratedPath,
                Namespace = RosalinaSettings.instance.DefaultNamespace
            };
            RosalinaSettings.instance.Files.Add(_fileSettings);
        }
        else
        {
            RosalinaSettings.instance.Files.Remove(_fileSettings);
            _fileSettings = null;
        }

        RefreshFileSettings();
        OnSettingsChanged();
    }

    private void OnGeneratorTypeSelectionChanged(ChangeEvent<Enum> @event)
    {
        if (@event.newValue != null && @event.newValue != @event.previousValue)
        {
            _fileSettings.Type = (RosalinaGenerationType)@event.newValue;

            RefreshFileSettings();
            OnSettingsChanged();
        }
    }

    private void OnPathChanged(ChangeEvent<string> @event)
    {
        if (@event.newValue != null && @event.newValue != @event.previousValue)
        {
            _fileSettings.GeneratedPath = @event.newValue;

            RefreshFileSettings();
            OnSettingsChanged();
        }
    }

    private void OnNamespaceChanged(ChangeEvent<string> @event)
    {
        if (@event.newValue != null && @event.newValue != @event.previousValue)
        {
            _fileSettings.Namespace = @event.newValue;

            RefreshFileSettings();
            OnSettingsChanged();
        }
    }

    private void OnFilePrefixChanged(ChangeEvent<string> @event)
    {
        if (@event.newValue != null && @event.newValue != @event.previousValue)
        {
            _fileSettings.FilePrefix = @event.newValue;

            RefreshFileSettings();
            OnSettingsChanged();
        }
    }

    private void OnFileSuffixChanged(ChangeEvent<string> @event)
    {
        if (@event.newValue != null && @event.newValue != @event.previousValue)
        {
            _fileSettings.FileSuffix = @event.newValue;

            RefreshFileSettings();
            OnSettingsChanged();
        }
    }

    private void OnGenerateBindings()
    {
        string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        UIDocumentAsset document = new(assetPath);

        document.GenerateBindings();
        AssetDatabase.Refresh();
    }

    private void OnGenerateScripts()
    {
        string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        UIDocumentAsset document = new(assetPath);

        bool bindingsGenerated = RosalinaScriptGeneratorUtilities.TryGenerateBindings(document);
        bool scriptGenerated = RosalinaScriptGeneratorUtilities.TryGenerateScript(document);

        if (bindingsGenerated || scriptGenerated)
        {
            AssetDatabase.Refresh();
        }
    }

    private void OnClearBindings()
    {
        string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        UIDocumentAsset document = new(assetPath);

        document.ClearBindings();
        AssetDatabase.Refresh();
    }

    private void OnSettingsChanged()
    {
        RosalinaSettings.instance.Save();
    }

    [MenuItem("Assets/Rosalina/Properties...", validate = true)]
    public static bool ShowWindowValidation()
    {
        return RosalinaSettings.instance.IsEnabled && Selection.activeObject != null && Selection.activeObject.GetType() == typeof(VisualTreeAsset);
    }

    [MenuItem("Assets/Rosalina/Properties...", priority = 1200)]
    public static void ShowWindow()
    {
        if (HasOpenInstances<RosalinaPropertiesEditorWindow>())
        {
            FocusWindowIfItsOpen<RosalinaPropertiesEditorWindow>();
            return;
        }

        Type inspectorWindowType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
        EditorWindow window = CreateWindow<RosalinaPropertiesEditorWindow>(inspectorWindowType);

        window.titleContent = new GUIContent("Rosalina", EditorGUIUtility.FindTexture("SettingsIcon"));
    }
}

#endif