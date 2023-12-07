#if UNITY_EDITOR

using System;
using UnityEngine;

[Serializable]
public class RosalinaFileSetting
{
    [SerializeField] private RosalinaGenerationType _type;

    [SerializeField] private string _generatedPath;

    [SerializeField] private string _namespace;

    [SerializeField] private string _filePrefix;

    [SerializeField] private string _fileSuffix;

    [SerializeField] private string _path;

    [SerializeField] private string _lastGeneratedFullPath;

    [SerializeField] private string _lastGeneratedScriptFullPath;

    public string LastGeneratedScriptFullPath
    {
        get => _lastGeneratedScriptFullPath;
        set => _lastGeneratedScriptFullPath = value;
    }

    public string LastGeneratedFullPath
    {
        get => _lastGeneratedFullPath;
        set => _lastGeneratedFullPath = value;
    }

    public string GeneratedPath
    {
        get => _generatedPath;
        set => _generatedPath = value;
    }

    public string Namespace
    {
        get => _namespace;
        set => _namespace = value;
    }

    public string FilePrefix
    {
        get => _filePrefix;
        set => _filePrefix = value;
    }

    public string FileSuffix
    {
        get => _fileSuffix;
        set => _fileSuffix = value;
    }

    /// <summary>
    ///     Gets or sets the generation type.
    /// </summary>
    public RosalinaGenerationType Type
    {
        get => _type;
        set => _type = value;
    }

    /// <summary>
    ///     Gets or sets the asset path;
    /// </summary>
    public string Path
    {
        get => _path;
        set => _path = value;
    }
}

#endif