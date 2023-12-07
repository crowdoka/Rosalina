#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class RosalinaSettingsProvider : SettingsProvider
{
    private ReorderableList _fileList;
    private RosalinaSettings _settings;

    public RosalinaSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
        : base(path, scopes, keywords)
    {
    }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        _settings = RosalinaSettings.instance;
        _settings.Save();

        _fileList = new ReorderableList(RosalinaSettings.instance.Files, typeof(RosalinaFileSetting), false, false, false, true);
        _fileList.drawElementCallback += OnDrawElement;
    }

    public override void OnDeactivate()
    {
        if (_fileList != null)
        {
            _fileList.drawElementCallback -= OnDrawElement;
        }

        base.OnDeactivate();
    }

    private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        if (index < 0)
        {
            return;
        }

        if (_fileList.list[index] is RosalinaFileSetting element)
        {
            rect.y += 2;
            EditorGUI.BeginDisabledGroup(true);
            element.Type = (RosalinaGenerationType)EditorGUI.EnumPopup(
                new Rect(rect.x, rect.y, 120, EditorGUIUtility.singleLineHeight),
                element.Type);
            EditorGUI.TextField(
                new Rect(rect.x + 130, rect.y, rect.width - 200, EditorGUIUtility.singleLineHeight),
                element.Path);
            EditorGUI.EndDisabledGroup();
        }
    }

    public override void OnGUI(string searchContext)
    {
        using (CreateSettingsWindowGUIScope())
        {
            EditorGUI.BeginChangeCheck();

            _settings.IsEnabled = EditorGUILayout.Toggle("Is Enabled", _settings.IsEnabled);
            _settings.DefaultGeneratedPath = EditorGUILayout.TextField("Default Path", _settings.DefaultGeneratedPath);
            _settings.DefaultNamespace = EditorGUILayout.TextField("Default Namespace", _settings.DefaultNamespace);
            EditorGUILayout.LabelField("Files");
            _fileList.DoLayoutList();

            if (EditorGUI.EndChangeCheck())
            {
                _settings.Save();
            }
        }
    }

    [SettingsProvider]
    public static SettingsProvider CreateRosalinaSettingsProvider()
    {
        return new RosalinaSettingsProvider("Project/Rosalina", SettingsScope.Project);
    }

    private static IDisposable CreateSettingsWindowGUIScope()
    {
        Assembly unityEditorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
        Type type = unityEditorAssembly.GetType("UnityEditor.SettingsWindow+GUIScope");

        return Activator.CreateInstance(type) as IDisposable;
    }
}

#endif