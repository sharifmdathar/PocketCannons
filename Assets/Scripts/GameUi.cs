using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI angleTMP;
    [SerializeField] private Button incrementAngleButton;
    [SerializeField] private Button decrementAngleButton;
    [SerializeField] private TMP_InputField angleInputField;
    [SerializeField] private Slider powerSlider;
    [SerializeField] private TextMeshProUGUI powerTMP;
    [SerializeField] private Button fireButton;
    [SerializeField] private GameObject radialSliderPopup;

    private float _lastClickTime;
    private const float DoubleClickTime = 0.3f;

    private void Start()
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

        if (fireButton != null)
        {
            fireButton.onClick.AddListener(OnFireClicked);
        }

        UpdateAngleText(GameManager.Instance.CurrentAngle);
        UpdatePowerUI(GameManager.Instance.CurrentPower);

        GameManager.Instance.OnAngleChanged += UpdateAngleText;
        GameManager.Instance.OnPowerChanged += UpdatePowerUI;
    }

    private static char ValidateAngleInput(string text, int charIndex, char addedChar)
    {
        if (text.Contains("°") && charIndex > text.IndexOf("°", StringComparison.Ordinal))
        {
            return '\0';
        }

        return char.IsDigit(addedChar) ? addedChar : '\0';
    }

    private void OnAngleInputValueChanged(string value)
    {
        if (value.EndsWith("°")) return;
        angleInputField.text = value + "°";
        angleInputField.caretPosition = angleInputField.text.Length - 1;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnAngleChanged -= UpdateAngleText;
        GameManager.Instance.OnPowerChanged -= UpdatePowerUI;
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

    private static void OnPowerSliderChanged(float value)
    {
        GameManager.Instance.SetPower(value);
    }

    private static void OnFireClicked()
    {
        GameManager.Instance.Fire();
    }

    public void OnAngleTextClicked()
    {
        if (Time.time - _lastClickTime < DoubleClickTime)
        {
            EnableAngleEditing();
            if (radialSliderPopup != null)
            {
                radialSliderPopup.SetActive(false);
            }
        }
        else
        {
            if (radialSliderPopup != null)
            {
                bool isSliderActive = !radialSliderPopup.activeSelf;
                radialSliderPopup.SetActive(isSliderActive);

                if (isSliderActive)
                {
                    var sliderScript = radialSliderPopup.GetComponent<RadialSlider>();
                    if (sliderScript != null)
                    {
                        sliderScript.OnCloseRequested = () =>
                        {
                            radialSliderPopup.SetActive(false);
                            if (angleTMP != null) angleTMP.gameObject.SetActive(true);
                        };
                    }
                }

                if (angleTMP != null)
                {
                    angleTMP.gameObject.SetActive(!isSliderActive);
                }
            }
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
        var cleanValue = value.Replace("°", "");
        if (int.TryParse(cleanValue, out int result))
        {
            GameManager.Instance.SetAngle(result);
        }

        angleInputField.gameObject.SetActive(false);
        angleTMP.gameObject.SetActive(true);
    }
}