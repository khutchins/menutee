using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Menutee {
    [RequireComponent(typeof(ScrollRect))]
    public class DropdownAutoScroll : MonoBehaviour {
        private ScrollRect _scrollRect;

        private void Awake() {
            _scrollRect = GetComponent<ScrollRect>();
        }

        private void OnEnable() {
            StartCoroutine(SetupItems());
        }

        private IEnumerator SetupItems() {
            yield return new WaitForEndOfFrame();

            var items = _scrollRect.content.GetComponentsInChildren<Selectable>();

            foreach (var item in items) {
                if (!item.gameObject.TryGetComponent<DropdownItemListener>(out var trigger)) {
                    trigger = item.gameObject.AddComponent<DropdownItemListener>();
                }
                trigger.Init(this);
            }

            if (EventSystem.current.currentSelectedGameObject != null &&
                EventSystem.current.currentSelectedGameObject.transform.IsChildOf(_scrollRect.content)) {
                ScrollTo(EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>());
            }
        }

        public void ScrollTo(RectTransform target) {
            Canvas.ForceUpdateCanvases();

            float contentHeight = _scrollRect.content.rect.height;
            float viewportHeight = _scrollRect.viewport.rect.height;

            if (contentHeight < viewportHeight) return;

            float targetY = -target.localPosition.y - (target.rect.height / 2);
            float normalized = 1 - (targetY / (contentHeight - viewportHeight));

            _scrollRect.verticalNormalizedPosition = Mathf.Clamp01(normalized);
        }
    }

    public class DropdownItemListener : MonoBehaviour, ISelectHandler {
        private DropdownAutoScroll _mainScroll;

        public void Init(DropdownAutoScroll main) {
            _mainScroll = main;
        }

        public void OnSelect(BaseEventData eventData) {
            if (_mainScroll != null) {
                _mainScroll.ScrollTo(GetComponent<RectTransform>());
            }
        }
    }
}