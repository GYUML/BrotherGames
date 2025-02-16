using System;
using TMPro;
using UnityEngine;

public class MessagePopup : UIPopup
{
    public TMP_Text titleText;
    public TMP_Text contentText;

    public KButton okButton;

    Action onYes;

    private void Start()
    {
        okButton.onClick.AddListener(() =>
        {
            onYes?.Invoke();
            Hide();
        });
    }

    public void Set(string title, string message, Action onYes = null)
    {
        titleText.text = title;
        contentText.text = message;
        this.onYes = onYes;
    }
}
