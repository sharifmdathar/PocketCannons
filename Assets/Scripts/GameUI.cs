using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Button incrementAngleButton;
    [SerializeField] private Button decrementAngleButton;
    [SerializeField] private TextMeshProUGUI angleTMP;

    void Start()
    {
        incrementAngleButton.onClick.AddListener(OnIncrementAngleClicked);
        decrementAngleButton.onClick.AddListener(OnDecrementAngleClicked);
        
        // Initialize UI with current value
        UpdateAngleText(GameManager.Instance.CurrentAngle);
        
        // Subscribe to future changes
        GameManager.Instance.OnAngleChanged += UpdateAngleText;
    }

    private void OnDestroy()
    {
        // Clean up listener to avoid memory leaks
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnAngleChanged -= UpdateAngleText;
        }
    }

    private void OnIncrementAngleClicked()
    {
        GameManager.Instance.IncrementAngle();
    }

    private void OnDecrementAngleClicked()
    {
        GameManager.Instance.DecrementAngle();
    }

    private void UpdateAngleText(int newAngle)
    {
        angleTMP.text = $"Angle: {newAngle}°";
    }
}
