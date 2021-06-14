using CustomAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Spacetaurant
{
    [RequireComponent(typeof(GraphicRaycaster))]
    public class IngameCameraDrag : MonoBehaviour
    {
        private int? _fingerId = null;
        public Vector2 totalDragDelta;
        public Vector2 dragDelta;
        [SerializeField]
        private UnityEvent<Vector2> OnDrag;
        [SerializeField]
        private UnityEvent OnStopDrag;
        private void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch? tempTouch = null;

                if (_fingerId != null)
                {
                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        if (Input.GetTouch(i).fingerId == _fingerId.Value)
                        {
                            tempTouch = Input.GetTouch(i);
                            break;
                        }
                    }
                }

                if (tempTouch == null)
                {
                    ResetDrag();

                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        Touch _touchI = Input.GetTouch(i);
                        if (_touchI.phase == TouchPhase.Began)
                        {
                            if (OverUIObject(_touchI.position))
                                continue;

                            tempTouch = _touchI;
                            StartDrag(tempTouch.Value);
                        }
                    }
                }

                if (tempTouch == null)
                    return;

                Touch touch = tempTouch.Value;
                _fingerId = touch.fingerId;

                switch (touch.phase)
                {
                    case TouchPhase.Moved:
                        Drag(touch);
                        break;
                    default:
                    case TouchPhase.Began:
                    case TouchPhase.Stationary:
                        break;
                    case TouchPhase.Ended:
                        DragStop();
                        break;
                    case TouchPhase.Canceled:
                        DragStop();
                        break;
                }
            }
            else
            {
                ResetDrag();
            }
        }



        private void StartDrag(Touch touch)
        {
            Debug.Log("Drag start");
            dragDelta = Vector2.zero;
            totalDragDelta = Vector2.zero;
        }
        private void Drag(Touch touch)
        {
            dragDelta = touch.deltaPosition;
            totalDragDelta += dragDelta;
            OnDrag?.Invoke(dragDelta);
        }
        private void ResetDrag()
        {
            totalDragDelta = Vector2.zero;
            dragDelta = Vector2.zero;
            _fingerId = null;
        }
        private void DragStop()
        {
            Debug.Log("Drag stop");
            ResetDrag();
            OnStopDrag?.Invoke();

        }

        private bool OverUIObject(Vector2 screenPos)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = screenPos;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
    }
}
