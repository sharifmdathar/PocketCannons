using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameUi : MonoBehaviour
    {
        [SerializeField] private Button incrementAngleButton;
        [SerializeField] private Button decrementAngleButton;
        [SerializeField] private TextMeshProUGUI angleTMP;

        void Start()
        {
            incrementAngleButton.onClick.AddListener(OnIncrementAngleClicked);
            decrementAngleButton.onClick.AddListener(OnDecrementAngleClicked);

            UpdateAngleText(GameManager.Instance.CurrentAngle);

            GameManager.Instance.OnAngleChanged += UpdateAngleText;
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnAngleChanged -= UpdateAngleText;
            }
        }

        private static void OnIncrementAngleClicked()
        {
            GameManager.Instance.IncrementAngle();
        }

        private static void OnDecrementAngleClicked()
        {
            GameManager.Instance.DecrementAngle();
        }

        private void UpdateAngleText(int newAngle)
        {
            angleTMP.text = $"Angle: {newAngle}°";
        }
    }
}
