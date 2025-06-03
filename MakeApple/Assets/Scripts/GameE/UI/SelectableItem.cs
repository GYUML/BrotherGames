using UnityEngine;

namespace GameE
{
    public class SelectableItem : MonoBehaviour
    {
        public GameObject selectImage;

        public void OnSelected(bool selected)
        {
            selectImage.SetActive(selected);
        }
    }
}
