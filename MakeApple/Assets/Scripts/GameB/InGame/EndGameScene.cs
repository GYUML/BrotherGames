using DG.Tweening;
using GameB;
using UnityEngine;

public class EndGameScene : MonoBehaviour
{
    public Animator playerAnimator;

    public void Play()
    {
        playerAnimator.SetTrigger("2_Attack");
        playerAnimator.transform.position = new Vector2(0f, 8f);
        playerAnimator.transform.DOMoveY(-4.2f, 0.5f).SetEase(Ease.Linear)
            .OnComplete(() => Managers.UI.ShowPopup<FallingGameResultPopup>().StartEndEffect(10000, 3000));
    }
}
