#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
///     Provides a data structure for Rosalina settings.
/// </summary>
[FilePath("ProjectSettings/RosalinaSettings.asset", FilePathAttribute.Location.ProjectFolder)]
public class RosalinaSettings : ScriptableSingleton<RosalinaSettings>
{
    [SerializeField] private bool _isEnabled;

    [SerializeField] private string _defaultGeneratedPath;

    [SerializeField] private string _defaultNamespace;

    [SerializeField] private List<RosalinaFileSetting> _files = new();

    public string DefaultNamespace
    {
        get => _defaultNamespace;
        set => _defaultNamespace = value;
    }

    /// <summary>
    ///     Gets or sets a boolean value that indicates if Rosalina is enabled.
    /// </summary>
    public bool IsEnabled
    {
        get => _isEnabled;
        set => _isEnabled = value;
    }

    public string DefaultGeneratedPath
    {
        get => _defaultGeneratedPath;
        set => _defaultGeneratedPath = value;
    }

    /// <summary>
    ///     Gets or sets the asset files that Rosalina should process.
    /// </summary>
    public List<RosalinaFileSetting> Files
    {
        get => _files;
        set => _files = value;
    }

    /// <summary>
    ///     Save the settings.
    /// </summary>
    public void Save()
    {
        Save(true);
    }

    /// <summary>
    ///     Gets a file setting based on the given path.
    /// </summary>
    /// <param name="path">Asset path.</param>
    /// <returns>The file settings if found; null otherwise.</returns>
    public RosalinaFileSetting GetFileSetting(string path)
    {
        return Files.FirstOrDefault(x => x.Path == path);
    }

    /// <summary>
    ///     Gets a boolean value that indicates if the file setting exists.
    /// </summary>
    /// <param name="path">Asset path.</param>
    /// <returns>True if the file exists within the Rosalina settings; false otherwise.</returns>
    public bool ContainsFile(string path)
    {
        return Files.Any(x => x.Path == path);
    }
}

#endif