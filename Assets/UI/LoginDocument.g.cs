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

public partial class LoginDocument : MonoBehaviour
{
    [SerializeField]
    private UIDocument _document;
    private Label _titleLabel;
    private Button _button1;
    private Toggle _checkboxTogglee;
    public VisualElement Root
    {
        get
        {
            return _document?.rootVisualElement;
        }
    }

    public void InitializeDocument()
    {
        _titleLabel = (Label)Root?.Q("TitleLabel");
        _button1 = (Button)Root?.Q("Button1");
        _checkboxTogglee = (Toggle)Root?.Q("CheckboxTogglee");
    }
}