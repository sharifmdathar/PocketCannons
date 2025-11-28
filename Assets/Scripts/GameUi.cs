using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI angleTMP;
    [SerializeField] private Button incrementAngleButton;
    [SerializeField] private Button decrementAngleButton;
    [SerializeField] private TMP_Dropdown weaponDropdown;
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
    [SerializeField] private GameObject gameOverPopup;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button regenerateMapButton;

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

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }

        if (regenerateMapButton != null)
        {
            regenerateMapButton.onClick.AddListener(OnRegenerateMapClicked);
        }

        if (powerSlider != null)
        {
            powerSlider.onValueChanged.AddListener(OnPowerSliderChanged);
        }

        if (fireButton != null)
        {
            fireButton.onClick.AddListener(OnFireClicked);
        }

        if (weaponDropdown != null)
        {
            weaponDropdown.onValueChanged.AddListener(OnWeaponDropdownChanged);
            InitializeWeaponDropdown();
        }

        UpdateAngleText(GameManager.Instance.CurrentAngle);
        UpdatePowerUi(GameManager.Instance.CurrentPower);
        UpdateWeaponUi(GameManager.Instance.CurrentAttackType);

        GameManager.Instance.OnAngleChanged += UpdateAngleText;
        GameManager.Instance.OnPowerChanged += UpdatePowerUi;
        GameManager.Instance.OnAttackTypeChanged += UpdateWeaponUi;
        GameManager.Instance.OnHealthChanged += UpdateHealthUi;
        GameManager.Instance.OnGameOver += OnGameOver;

        UpdateHealthUi(GameManager.Turn.Player1, GameManager.Instance.GetHealth(GameManager.Turn.Player1));
        UpdateHealthUi(GameManager.Turn.Player2, GameManager.Instance.GetHealth(GameManager.Turn.Player2));
    }

    private void OnDestroy()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnAngleChanged -= UpdateAngleText;
        GameManager.Instance.OnPowerChanged -= UpdatePowerUi;
        GameManager.Instance.OnAttackTypeChanged -= UpdateWeaponUi;
        GameManager.Instance.OnHealthChanged -= UpdateHealthUi;
        GameManager.Instance.OnGameOver -= OnGameOver;
    }

    private void OnGameOver(GameManager.Turn winner)
    {
        if (gameOverPopup == null) return;
        gameOverPopup.SetActive(true);
        if (winnerText != null)
        {
            winnerText.text = $"{winner} Wins!";
        }
    }

    private static void OnRestartClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

    private void UpdatePowerUi(float newPower)
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

    private void UpdateHealthUi(GameManager.Turn player, float health)
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

    private void InitializeWeaponDropdown()
    {
        if (weaponDropdown == null) return;

        weaponDropdown.ClearOptions();
        weaponDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "Single Shot",
            "Triple Shot"
        });

        UpdateWeaponUi(GameManager.Instance.CurrentAttackType);
    }

    private static void OnWeaponDropdownChanged(int index)
    {
        var attackType = (GameManager.AttackType)index;
        GameManager.Instance.CurrentAttackType = attackType;
    }

    private void UpdateWeaponUi(GameManager.AttackType attackType)
    {
        if (weaponDropdown == null) return;
        
        var index = (int)attackType;
        if (index >= 0 && index < weaponDropdown.options.Count)
        {
            weaponDropdown.SetValueWithoutNotify(index);
        }
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

    private static void OnRegenerateMapClicked()
    {
        if (TerrainGenerator.Instance == null) return;

        TerrainGenerator.Instance.GenerateTerrain();

        var cannons = FindObjectsByType<CannonController>(FindObjectsSortMode.None);
        foreach (var c in cannons)
        {
            c.SnapToGround();
        }
    }
}