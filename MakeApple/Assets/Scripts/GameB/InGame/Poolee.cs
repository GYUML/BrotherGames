using UnityEngine;

namespace GameB
{
    public class Poolee : MonoBehaviour
    {
        public string Key
        {
            get { return key; }
        }

        string key;

        public void OnInstantiated(string key)
        {
            this.key = key;
        }
    }
}
