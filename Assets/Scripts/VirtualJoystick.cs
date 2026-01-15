using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private RectTransform _containerRect;
    private RectTransform _handleRect;
    private Vector2 _inputVector = Vector2.zero;
    private float _joystickRadius;
    
    public UnityEvent<Vector2> OnJoystickMove = new UnityEvent<Vector2>();
    
    private void Start()
    {
        _containerRect = GetComponent<RectTransform>();
        _handleRect = transform.GetChild(0).GetComponent<RectTransform>();
        _joystickRadius = _containerRect.sizeDelta.x / 2f;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        HandleInput(eventData.position);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        HandleInput(eventData.position);
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        _inputVector = Vector2.zero;
        _handleRect.anchoredPosition = Vector2.zero;
        OnJoystickMove.Invoke(_inputVector);
    }
    
    private void HandleInput(Vector2 touchPosition)
    {
        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_containerRect, touchPosition, null, out localPosition);
        
        _inputVector = localPosition.normalized;
        float distance = localPosition.magnitude;
        
        if (distance > _joystickRadius)
        {
            _inputVector = _inputVector.normalized;
        }
        else
        {
            _inputVector = (localPosition / _joystickRadius).normalized;
        }
        
        _handleRect.anchoredPosition = _inputVector * (_joystickRadius * 0.6f);
        OnJoystickMove.Invoke(_inputVector);
    }
    
    public Vector2 GetInputVector() => _inputVector;
}
