using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameLayout : MonoBehaviour
{
    public GameProcedures procedures;

    public EventButton chargeButton;
    public GuageBar chargeGuage;
    public GameObject diceScreen;
    public TMP_Text diceText;

    public float maxChargeTime;

    float nowGuage;
    float chargedTime;
    bool isCharging;

    private void Start()
    {
        chargeButton.SetPointerDownEvent((data) => SetChargeState(true));
        chargeButton.SetPointerUpEvent((data) => SetChargeState(false));
        diceScreen.SetActive(false);
    }
    
    void Update()
    {
        if (maxChargeTime > 0)
        {
            if (isCharging)
            {
                chargedTime += Time.deltaTime / maxChargeTime;
            }
            else if (nowGuage > 0)
            {
                procedures.RollDice(nowGuage);
                chargedTime = 0;
            }
            
            var caculated = chargedTime % (2 * maxChargeTime);
            if (caculated < maxChargeTime)
                nowGuage = caculated / maxChargeTime;
            else
                nowGuage = (2 * maxChargeTime - caculated) / maxChargeTime;

            chargeGuage.SetGuage(nowGuage);
        }
    }

    public void SetChargeButtonEnable(bool enable)
    {
        chargeButton.Enable = enable;
    }

    public void ShowDiceScreen(bool show)
    {
        diceScreen.SetActive(show);
    }

    public void UpdateDiceText(int dice)
    {
        diceText.text = dice.ToString();
    }

    void SetChargeState(bool isPressed)
    {
        isCharging = isPressed;
        chargeButton.transform.localScale = Vector3.one * (isPressed ? 0.9f : 1f);
    }
}
