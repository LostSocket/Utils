using UnityEngine;
using UnityEngine.UI;

namespace Utils.UI
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField]
        private Image Background;
        
        [SerializeField]
        private Image Foreground;

        public float Value { get; private set; } = 1;

        public void SetValue(float value)
        {
            Value = value;
            Foreground.fillAmount = Value;
        }
    }
}
