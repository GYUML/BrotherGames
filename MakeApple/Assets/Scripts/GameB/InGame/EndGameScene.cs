using DG.Tweening;
using GameB;
using UnityEngine;

public class EndGameScene : MonoBehaviour
{
    public Animator playerAnimator;

    public void Play(long maxHp, long damage)
    {
        playerAnimator.SetTrigger("2_Attack");
        playerAnimator.transform.position = new Vector2(0f, 8f);
        playerAnimator.transform.DOMoveY(-4.2f, 0.5f).SetEase(Ease.Linear)
            .OnComplete(() => Managers.UI.ShowPopup<FallingGameResultPopup>().StartEndEffect(maxHp, damage));
    }
}
