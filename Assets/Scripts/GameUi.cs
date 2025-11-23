using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI angleTMP;
    [SerializeField] private Button incrementAngleButton;
    [SerializeField] private Button decrementAngleButton;
    [SerializeField] private Slider powerSlider;
    [SerializeField] private TextMeshProUGUI powerTMP;
    [SerializeField] private Button fireButton;
    [SerializeField] private GameObject radialSliderPopup;
    [SerializeField] private Slider p1HealthSlider;
    [SerializeField] private Image p1FillImage;
    [SerializeField] private Slider p2HealthSlider;
    [SerializeField] private Image p2FillImage;
    [SerializeField] private RepeatButton moveLeftButton;
    [SerializeField] private RepeatButton moveRightButton;

    private void Start()
    {
        incrementAngleButton.onClick.AddListener(OnIncrementAngleClicked);
        decrementAngleButton.onClick.AddListener(OnDecrementAngleClicked);

        if (moveLeftButton != null)
        {
            moveLeftButton.onHold.AddListener(() => GameManager.Instance.Move(-1f));
        }

        if (moveRightButton != null)
        {
            moveRightButton.onHold.AddListener(() => GameManager.Instance.Move(1f));
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
        GameManager.Instance.OnHealthChanged += UpdateHealthUI;

        UpdateHealthUI(GameManager.Turn.Player1, GameManager.Instance.GetHealth(GameManager.Turn.Player1));
        UpdateHealthUI(GameManager.Turn.Player2, GameManager.Instance.GetHealth(GameManager.Turn.Player2));
    }

    private void OnDestroy()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnAngleChanged -= UpdateAngleText;
        GameManager.Instance.OnPowerChanged -= UpdatePowerUI;
        GameManager.Instance.OnHealthChanged -= UpdateHealthUI;
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
        if (radialSliderPopup != null && radialSliderPopup.activeSelf)
        {
            angleTMP.text = $"{newAngle}°";
        }
        else
        {
            angleTMP.text = $"Angle: {newAngle}°";
        }
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

    private void UpdateHealthUI(GameManager.Turn player, float health)
    {
        switch (player)
        {
            case GameManager.Turn.Player1 when p1HealthSlider != null:
            {
                p1HealthSlider.value = health;
                if (p1FillImage != null) p1FillImage.color = Color.Lerp(Color.red, Color.green, health / 100f);
                break;
            }
            case GameManager.Turn.Player2 when p2HealthSlider != null:
            {
                p2HealthSlider.value = health;
                if (p2FillImage != null) p2FillImage.color = Color.Lerp(Color.red, Color.green, health / 100f);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(player), player, null);
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
        if (radialSliderPopup == null) return;
        var isSliderActive = !radialSliderPopup.activeSelf;
        radialSliderPopup.SetActive(isSliderActive);
        UpdateAngleText(GameManager.Instance.CurrentAngle);

        if (!isSliderActive) return;
        var sliderScript = radialSliderPopup.GetComponent<RadialSlider>();
        if (sliderScript != null)
        {
            sliderScript.OnCloseRequested = () =>
            {
                radialSliderPopup.SetActive(false);
                UpdateAngleText(GameManager.Instance.CurrentAngle);
            };
        }
    }
}