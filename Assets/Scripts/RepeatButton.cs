using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class RepeatButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
  public UnityEvent onHold;
  private bool _isHeld;

  public void OnPointerDown(PointerEventData eventData)
  {
    _isHeld = true;
  }

  public void OnPointerUp(PointerEventData eventData)
  {
    _isHeld = false;
  }

  private void Update()
  {
    if (_isHeld)
    {
      onHold?.Invoke();
    }
  }
}
