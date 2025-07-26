using UnityEngine;
using UnityEngine.UI;

public class GameLayout : MonoBehaviour
{
    public EventButton chargeButton;
    public GuageBar chargeGuage;
    public FieldSpawner fieldSpawner;

    public float maxChargeTime;

    float nowGuage;
    float chargedTime;
    bool isCharging;

    // TODO
    int nowPosition = 0;

    private void Start()
    {
        chargeButton.SetPointerDownEvent((data) => SetChargeState(true));
        chargeButton.SetPointerUpEvent((data) => SetChargeState(false));
    }
    
    void Update()
    {
        if (maxChargeTime > 0)
        {
            if (isCharging)
            {
                chargedTime += Time.deltaTime / maxChargeTime;
            }
            else if (chargedTime > 0)
            {
                fieldSpawner.RollDice(1, 1);
                fieldSpawner.MoveFigure(nowPosition, nowPosition + 2);
                nowPosition += 2;
                chargedTime = 0f;
            }
            
            var caculated = chargedTime % (2 * maxChargeTime);
            if (caculated < maxChargeTime)
                nowGuage = caculated / maxChargeTime;
            else
                nowGuage = (2 * maxChargeTime - caculated) / maxChargeTime;

            chargeGuage.SetGuage(nowGuage);
        }
    }

    void SetChargeState(bool isPressed)
    {
        isCharging = isPressed;
        chargeButton.transform.localScale = Vector3.one * (isPressed ? 0.9f : 1f);
    }
}
