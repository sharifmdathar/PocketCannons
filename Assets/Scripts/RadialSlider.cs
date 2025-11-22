using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
  public class RadialSlider : MonoBehaviour, IDragHandler, IPointerDownHandler
  {
    [SerializeField] private RectTransform handle;
    [SerializeField] private float radius = 100f; // Distance of handle from center
    [SerializeField] private float centerDeadZoneRadius = 40f; // Clicks inside this radius will close the slider

    public System.Action OnCloseRequested;

    private RectTransform _rectTransform;

    private void Awake()
    {
      _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
      // Initialize handle position based on current angle
      if (GameManager.Instance != null)
      {
        UpdateHandlePosition(GameManager.Instance.CurrentAngle);
        GameManager.Instance.OnAngleChanged += UpdateHandlePosition;
      }
    }

    private void OnDisable()
    {
      if (GameManager.Instance != null)
      {
        GameManager.Instance.OnAngleChanged -= UpdateHandlePosition;
      }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
      UpdateAngleFromInput(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
      UpdateAngleFromInput(eventData);
    }

    private void UpdateAngleFromInput(PointerEventData eventData)
    {
      Vector2 localPoint;
      if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position,
            eventData.pressEventCamera, out localPoint))
      {
        // Check if click is in the center dead zone
        if (localPoint.magnitude < centerDeadZoneRadius)
        {
          OnCloseRequested?.Invoke();
          return;
        }

        float angle = Mathf.Atan2(localPoint.y, localPoint.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;

        GameManager.Instance.SetAngle((int)angle);
      }
    }

    private void UpdateHandlePosition(int angle)
    {
      if (handle == null) return;

      // Convert angle to radians
      float radians = angle * Mathf.Deg2Rad;

      // Calculate position on the circle
      Vector2 position = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * radius;
      handle.anchoredPosition = position;

      // Rotate handle to face outward
      handle.localRotation = Quaternion.Euler(0, 0, angle);
    }
  }
}
