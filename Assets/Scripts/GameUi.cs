using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameUi : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI angleTMP;
        [SerializeField] private Button incrementAngleButton;
        [SerializeField] private Button decrementAngleButton;
        [SerializeField] private TMP_InputField angleInputField;
        [SerializeField] private Slider powerSlider;
        [SerializeField] private TextMeshProUGUI powerTMP;

        private float _lastClickTime;
        private const float DoubleClickTime = 0.3f;

        void Start()
        {
            incrementAngleButton.onClick.AddListener(OnIncrementAngleClicked);
            decrementAngleButton.onClick.AddListener(OnDecrementAngleClicked);

            if (angleInputField != null)
            {
                angleInputField.gameObject.SetActive(false);
                angleInputField.onEndEdit.AddListener(OnAngleInputSubmitted);
                angleInputField.onValueChanged.AddListener(OnAngleInputValueChanged);

                angleInputField.contentType = TMP_InputField.ContentType.Custom;
                angleInputField.keyboardType = TouchScreenKeyboardType.NumberPad;
                angleInputField.onValidateInput += ValidateAngleInput;
            }

            if (powerSlider != null)
            {
                powerSlider.onValueChanged.AddListener(OnPowerSliderChanged);
            }

            UpdateAngleText(GameManager.Instance.CurrentAngle);
            UpdatePowerUI(GameManager.Instance.CurrentPower);

            GameManager.Instance.OnAngleChanged += UpdateAngleText;
            GameManager.Instance.OnPowerChanged += UpdatePowerUI;
        }

        private char ValidateAngleInput(string text, int charIndex, char addedChar)
        {
            if (text.Contains("°") && charIndex > text.IndexOf("°"))
            {
                return '\0';
            }

            return char.IsDigit(addedChar) ? addedChar : '\0';
        }

        private void OnAngleInputValueChanged(string value)
        {
            if (!value.EndsWith("°"))
            {
                angleInputField.text = value + "°";
                angleInputField.caretPosition = angleInputField.text.Length - 1;
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnAngleChanged -= UpdateAngleText;
                GameManager.Instance.OnPowerChanged -= UpdatePowerUI;
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

        private void UpdatePowerUI(float newPower)
        {
            if (powerSlider != null)
            {
                powerSlider.SetValueWithoutNotify(newPower);
            }

            if (powerTMP != null)
            {
                powerTMP.text = $"Power: {newPower:F0}%";
            }
        }

        private void OnPowerSliderChanged(float value)
        {
            GameManager.Instance.SetPower(value);
        }

        public void OnAngleTextClicked()
        {
            if (Time.time - _lastClickTime < DoubleClickTime)
            {
                EnableAngleEditing();
            }

            _lastClickTime = Time.time;
        }

        private void EnableAngleEditing()
        {
            if (angleInputField == null) return;

            angleTMP.gameObject.SetActive(false);
            angleInputField.gameObject.SetActive(true);
            angleInputField.text = $"{GameManager.Instance.CurrentAngle}°";
            angleInputField.ActivateInputField();

            angleInputField.caretPosition = angleInputField.text.Length - 1;
        }

        private void OnAngleInputSubmitted(string value)
        {
            string cleanValue = value.Replace("°", "");
            if (int.TryParse(cleanValue, out int result))
            {
                GameManager.Instance.SetAngle(result);
            }

            angleInputField.gameObject.SetActive(false);
            angleTMP.gameObject.SetActive(true);
        }
    }
}
