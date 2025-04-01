using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameC
{
    public class GuageBar : MonoBehaviour
    {
        public Image fillGuage;
        public TMP_Text fillText;

        RectTransform rectTransform
        {
            get 
            {
                if (mRectTransform == null)
                    mRectTransform = GetComponent<RectTransform>();
                return mRectTransform;
            }
        }

        RectTransform mRectTransform;

        public void SetGuage(long max, long now)
        {
            fillGuage.fillAmount = (float)now / max;

            if (fillText != null)
                fillText.text = $"{now} / {max}";
        }

        public void SetPosition(Vector2 anchoredPosition)
        {
            rectTransform.anchoredPosition = anchoredPosition;
        }
    }
}
