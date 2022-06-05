﻿#if UNITY_EDITOR
using System;
using System.Collections.Generic;

internal static class UIPropertyTypes
{
    private static readonly IDictionary<string, Type> _nativeUITypes = new Dictionary<string, Type>()
    {
        // Containers
        { "VisualElement", typeof(UnityEngine.UIElements.VisualElement) },
        { "ScrollView", typeof(UnityEngine.UIElements.ScrollView) },
        { "ListView", typeof(UnityEngine.UIElements.ListView) },
        { "IMGUIContainer", typeof(UnityEngine.UIElements.IMGUIContainer) },
        { "GroupBox", typeof(UnityEngine.UIElements.GroupBox) },
        // Controls
        { "Label", typeof(UnityEngine.UIElements.Label) },
        { "Button", typeof(UnityEngine.UIElements.Button) },
        { "Toggle", typeof(UnityEngine.UIElements.Toggle) },
        { "Scroller", typeof(UnityEngine.UIElements.Scroller) },
        { "TextField", typeof(UnityEngine.UIElements.TextField) },
        { "Foldout", typeof(UnityEngine.UIElements.Foldout) },
        { "Slider", typeof(UnityEngine.UIElements.Slider) },
        { "SliderInt", typeof(UnityEngine.UIElements.SliderInt) },
        { "MinMaxSlider", typeof(UnityEngine.UIElements.MinMaxSlider) },
        { "ProgressBar", typeof(UnityEngine.UIElements.ProgressBar) },
        { "DropdownField", typeof(UnityEngine.UIElements.DropdownField) },
        { "RadioButton", typeof(UnityEngine.UIElements.RadioButton) },
        { "RadioButtonGroup", typeof(UnityEngine.UIElements.RadioButtonGroup) },
    };

    public static Type GetUIElementType(string uiElementName)
    {
        return _nativeUITypes.TryGetValue(uiElementName, out Type type) ? type : null;
    }
}
#endif