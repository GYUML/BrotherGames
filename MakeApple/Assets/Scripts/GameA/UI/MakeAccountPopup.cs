using System;
using TMPro;
using UnityEngine;

public class MakeAccountPopup : UIPopup
{
    public TMP_InputField nicknameField;
    public KButton registerButton;

    Action<string> onRegister;

    private void Start()
    {
        registerButton.onClick.AddListener(() =>
        {
            onRegister?.Invoke(nicknameField.text);
        });
    }

    public void Set(Action<string> onRegister)
    {
        this.onRegister = onRegister;
    }
}
