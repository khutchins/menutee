using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menutee.Samples {
    public class AnimatingMenuManager : MenuManager {
        public RectTransform Rect;
        public RectTransform ShowPosition;
        public RectTransform HidePosition;
        public float PanelWidth = 600;
        public float AnimTime = 0.2f;

        private Coroutine _animCoroutine;
        private Coroutine _slideCoroutine;

        protected override void SetMenuIsUp(bool isUp, string newKey) {
            if (isUp) {
                // If it's going to be displayed, show the canvas at the start,
                // so it will visibly animate in.
                Canvas.enabled = isUp;
                ActivatePanel(newKey, true);
            }
            if (_animCoroutine != null) {
                StopCoroutine(_animCoroutine);
                _animCoroutine = null;
            }
            _animCoroutine = StartCoroutine(AnimateRaiseMenu(isUp, () => {
                if (!isUp) {
                    // If it's going to be hidden, hide the canvas at the end,
                    // so it will visibly animate out.
                    Canvas.enabled = isUp;
                    ActivatePanel(newKey, true);
                }
            }));
        }

        protected override void SetOnTop(bool isOnTop) {
            // Don't hide if not on top
        }

        protected override void ActivatePanel(PanelManager oldPanel, PanelManager newPanel, bool fromPush) {
            if (_slideCoroutine != null) {
                StopCoroutine(_slideCoroutine);
                _slideCoroutine = null;
            }

            _slideCoroutine = StartCoroutine(AnimateSlidePanel(oldPanel, newPanel, fromPush, () => {
                // Hide non-active panels.
                DisableOtherPanels(newPanel);
            }));
        }

        IEnumerator AnimateSlidePanel(PanelManager oldPanel, PanelManager newPanel, bool fromPush, Action finished = null) {
            // Enable the new panel, as both panels will be visible
            // for the duration of the animation.
            EnablePanel(newPanel, fromPush);

            if (oldPanel != null && newPanel != null) {
                RectTransform oldRect = oldPanel.GetComponent<RectTransform>();
                RectTransform newRect = newPanel.GetComponent<RectTransform>();
                // Slides from right for a push, from left for a pop.
                Vector2 offset = new Vector2(PanelWidth * (fromPush ? -1 : 1), 0);
                Vector2 oldAnchorFrom = Rect.anchoredPosition;
                Vector2 oldAnchorTo = oldAnchorFrom + offset;
                Vector2 newAnchorTo = Rect.anchoredPosition;
                Vector2 newAnchorFrom = newAnchorTo - offset;

                yield return DoPercentAction((float percent) => {
                    oldRect.anchoredPosition = Vector2.Lerp(oldAnchorFrom, oldAnchorTo, percent);
                    newRect.anchoredPosition = Vector2.Lerp(newAnchorFrom, newAnchorTo, percent);
                }, AnimTime);
            } else {
                // No need to animate: no pre-existing panel or going to null panel.
            }

            finished?.Invoke();
        }

        IEnumerator AnimateRaiseMenu(bool newUp, Action finished = null) {
            RectTransform from = newUp ? HidePosition : ShowPosition;
            RectTransform to = newUp ? ShowPosition : HidePosition;

            float totalDist = Vector2.Distance(ShowPosition.anchoredPosition, HidePosition.anchoredPosition);
            Vector2 start = Rect.anchoredPosition;
            Vector2 target = to.anchoredPosition;
            float distToTarget = Vector2.Distance(start, target);
            float actualTime = (distToTarget / Mathf.Max(1, totalDist)) * AnimTime;

            float startTime = Time.time;

            yield return DoPercentAction((float percent) => {
                Rect.anchoredPosition = Vector2.Lerp(start, target, percent);
            }, actualTime);

            _animCoroutine = null;

            finished?.Invoke();
        }

        // This is just a simplified EZTween from KHPackageCore.
        public static IEnumerator DoPercentAction(Action<float> action, float duration) {
            float startTime = Time.unscaledTime;
            while (true) {
                float num;
                float percent = num = (Time.unscaledTime - startTime) / duration;
                if (!(num < 1f)) {
                    break;
                }

                action(percent);
                yield return null;
            }

            action(1f);
        }
    }
}