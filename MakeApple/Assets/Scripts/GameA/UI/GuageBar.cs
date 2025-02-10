using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuageBar : MonoBehaviour
{
    public Image fill;
    public TMP_Text amountText;

    public void SetGuage(long fullAmount, long nowAmount)
    {
        if (fullAmount == 0)
            return;

        if (nowAmount < 0)
            nowAmount = 0;

        fill.fillAmount = (float)nowAmount / fullAmount;

        if (amountText != null)
            amountText.text = $"{nowAmount}/{fullAmount}";
    }

    public void SetGuage(float fullAmount, float nowAmount)
    {
        if (fullAmount == 0)
            return;

        if (nowAmount < 0)
            nowAmount = 0;

        fill.fillAmount = nowAmount / fullAmount;

        if (amountText != null)
            amountText.text = $"{nowAmount:F1}/{fullAmount:F1}";
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
