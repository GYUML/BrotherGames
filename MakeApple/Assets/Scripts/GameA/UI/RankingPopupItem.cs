using TMPro;
using UnityEngine;

public class RankingPopupItem : MonoBehaviour
{
    public TMP_Text rankText;
    public TMP_Text nameText;
    public TMP_Text scoreText;
    public void Set(int rank, string name, int score)
    {
        rankText.text = $"#{rank}";
        nameText.text = name;
        scoreText.text = score.ToFormat();
    }
}
