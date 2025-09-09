using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MakeMapLayoutTile : MonoBehaviour
{
    public KButton centerButton;
    public TMP_Text centerText;

    public void SetText(string text)
    {
        centerText.text = text;
    }

    public void SetClickEvent(UnityAction action)
    {
        centerButton.onClick.RemoveAllListeners();
        centerButton.onClick.AddListener(action);
    }
}
