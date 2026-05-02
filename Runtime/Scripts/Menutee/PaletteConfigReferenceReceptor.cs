using Menutee;
using UnityEngine;

public class PaletteConfigReferenceReceptor : MonoBehaviour {
    public PaletteConfigReference Reference;
    private UIElementManager _manager;

    void Awake() {
        _manager = GetComponent<UIElementManager>();
    }

    void OnEnable() {
        if (Reference != null) {
            Reference.ValueChanged += OnPaletteChanged;
            OnPaletteChanged(Reference.Value);
        }
    }

    void OnDisable() {
        if (Reference != null) Reference.ValueChanged -= OnPaletteChanged;
    }

    private void OnPaletteChanged(PaletteConfig newPalette) {
        if (_manager != null && newPalette != null) {
            _manager.SetColors(newPalette);
        }
    }
}