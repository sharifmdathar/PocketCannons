using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class RadialSlider : MonoBehaviour, IDragHandler, IPointerDownHandler
{
  [SerializeField] private RectTransform handle;
  [SerializeField] private float radius = 100f;
  [SerializeField] private float centerDeadZoneRadius = 40f;

  public System.Action OnCloseRequested;

  private RectTransform _rectTransform;

  private void Awake()
  {
    _rectTransform = GetComponent<RectTransform>();
  }

  private void OnEnable()
  {
    if (GameManager.Instance == null) return;
    UpdateHandlePosition(GameManager.Instance.CurrentAngle);
    GameManager.Instance.OnAngleChanged += UpdateHandlePosition;
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
    if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position,
          eventData.pressEventCamera, out localPoint)) return;
    if (localPoint.magnitude < centerDeadZoneRadius)
    {
      OnCloseRequested?.Invoke();
      return;
    }

    var angle = Mathf.Atan2(localPoint.y, localPoint.x) * Mathf.Rad2Deg;
    if (angle < 0) angle += 360;

    GameManager.Instance.SetAngle((int)angle);
  }

  private void UpdateHandlePosition(int angle)
  {
    if (handle == null) return;

    var radians = angle * Mathf.Deg2Rad;

    var position = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * radius;
    handle.anchoredPosition = position;

    handle.localRotation = Quaternion.Euler(0, 0, angle);
  }
}