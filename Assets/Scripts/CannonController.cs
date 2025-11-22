using UnityEngine;
using Assets.Scripts;

public class CannonController : MonoBehaviour
{
    [SerializeField] private Transform barrelTransform;

    private void Start()
    {
        if (GameManager.Instance == null) return;
        UpdateRotation(GameManager.Instance.CurrentAngle);
        GameManager.Instance.OnAngleChanged += UpdateRotation;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnAngleChanged -= UpdateRotation;
        }
    }

    private void UpdateRotation(int angle)
    {
        if (barrelTransform != null)
        {
            barrelTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
