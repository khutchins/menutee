using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UniformInteraction : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IPointerExitHandler {
    [Tooltip("Called when the object is selected.")]
    public UnityEvent OnSelectOccurred;

    public void OnSelect(BaseEventData eventData) {
        OnSelectOccurred?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (EventSystem.current.currentSelectedGameObject == gameObject) {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
