//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Rosalina Code Generator tool.
//     Version: 1.0.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UIElements;

public partial class LoginDocument
{
    [SerializeField]
    private UIDocument _document;
    public Label TitleLabel { get; private set; }

    public Button Button1 { get; private set; }

    public Toggle CheckboxToggle { get; private set; }

    public Button ButtonWow { get; private set; }

    public VisualElement name { get; private set; }

    public VisualElement Root
    {
        get
        {
            return _document?.rootVisualElement;
        }
    }

    public void InitializeDocument()
    {
        TitleLabel = (Label)Root?.Q("TitleLabel");
        Button1 = (Button)Root?.Q("Button1");
        CheckboxToggle = (Toggle)Root?.Q("CheckboxToggle");
        ButtonWow = (Button)Root?.Q("ButtonWow");
        name = (VisualElement)Root?.Q("name");
    }
}