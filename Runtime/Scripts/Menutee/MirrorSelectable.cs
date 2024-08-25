using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Serialization;

namespace Menutee {
    /// <summary>
    /// Mirror the selectable state to the target graphic. Must be placed
    /// on the selectable to be mirrored.
    /// </summary>
    [RequireComponent(typeof(Selectable))]
    public class MirrorSelectable : MonoBehaviour, IPaletteReceptor,
            ISelectHandler, IDeselectHandler, 
            IPointerEnterHandler, IPointerExitHandler, 
            IPointerDownHandler, IPointerUpHandler {
        [Tooltip("Palette to read from. If on an object with UIElementManager, this will be set automatically.")]
        [SerializeField] PaletteConfig Palette;
        [Tooltip("Graphic to tint.")]
        [SerializeField] Graphic TargetGraphic;
        [Tooltip("If disabled, the diabled state will not be entered.")]
        [SerializeField] bool ProcessDisabled = true;
        [Tooltip("If disabled, the pressed state will not be entered.")]
        [SerializeField] bool ProcessPressed = true;
        [Tooltip("If disabled, the selected state will not be entered.")]
        [SerializeField] bool ProcessSelected = true;
        [Tooltip("If disabled, the highlighted state will not be entered.")]
        [SerializeField] bool ProcessHighlight = true;

        bool _selected;
        bool _highlighted;
        bool _pointerDown;
        Selectable _selectable;

        private enum SelectState {
            Normal = 0,
            Highlighted = 1,
            Pressed = 2,
            Selected = 3,
            Disabled = 4
        }

        void Awake() {
            _selectable = GetComponentInParent<Selectable>();
            UpdateGraphic(true);
        }

        public void OnSelect(BaseEventData eventData) {
            _selected = true;
            UpdateGraphic();
        }

        public void OnDeselect(BaseEventData data) {
            _selected = false;
            UpdateGraphic();
        }

        public void OnPointerEnter(PointerEventData eventData) {
            _highlighted = true;
            UpdateGraphic();
        }

        public void OnPointerExit(PointerEventData eventData) {
            _highlighted = false;
            UpdateGraphic();
        }

        public void OnPointerDown(PointerEventData eventData) {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            _pointerDown = true;
            UpdateGraphic();
        }

        public void OnPointerUp(PointerEventData eventData) {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            _pointerDown = false;
            UpdateGraphic();
        }

        bool ComputedDisabled { get => ProcessDisabled && !_selectable.IsInteractable(); }
        bool ComputedPressed { get => ProcessPressed && _pointerDown; }
        bool ComputedSelected { get => ProcessSelected && _selected; }
        bool ComputedHighlighted { get => ProcessHighlight && _highlighted; }

        SelectState CurrentSelectState {
            get {
                if (ComputedDisabled) return SelectState.Disabled;
                if (ComputedPressed) return SelectState.Pressed;
                if (ComputedSelected) return SelectState.Selected;
                if (ComputedHighlighted) return SelectState.Highlighted;
                return SelectState.Normal;
            }
        }

        private void UpdateGraphic(bool instant = false) {
            if (Palette == null || _selectable == null) return;
            Color col = Palette.NormalColor;
            switch (CurrentSelectState) {
                case SelectState.Normal:
                default:
                    break;
                case SelectState.Highlighted:
                    col = Palette.HighlightedColor;
                    break;
                case SelectState.Pressed:
                    col = Palette.PressedColor;
                    break;
                case SelectState.Selected:
                    col = Palette.SelectedColor;
                    break;
                case SelectState.Disabled:
                    col = Palette.DisabledColor;
                    break;
            }
            UpdateColor(col * Palette.ColorMultiplier, instant);
        }

        private void UpdateColor(Color color, bool instant) {
            if (TargetGraphic == null) return;
            TargetGraphic.CrossFadeColor(color, instant ? 0f : Palette.FadeDuration, true, true);
        }

        public void ReceivePalette(PaletteConfig config) {
            Palette = config;
            UpdateGraphic(true);
        }

        public void ForceRefresh() {
            UpdateGraphic();
        }
    }
}