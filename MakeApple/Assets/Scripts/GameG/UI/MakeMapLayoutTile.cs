using GameG;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;

public class MakeMapLayoutTile : MonoBehaviour
{
    public KButton centerButton;
    public TMP_Text centerText;
    public TMP_Text wallMaskText;

    public void SetText(string text)
    {
        centerText.text = text;
    }

    public void SetWallMaskText(int wallMask)
    {
        wallMaskText.text = Convert.ToString(wallMask, 2).PadLeft(4, '0');
    }

    public void SetClickEvent(UnityAction action)
    {
        centerButton.onClick.RemoveAllListeners();
        centerButton.onClick.AddListener(action);
    }
}
