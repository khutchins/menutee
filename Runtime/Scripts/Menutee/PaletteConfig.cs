using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Menutee {
	[CreateAssetMenu(menuName = "Menutee/Palette Config")]
	public class PaletteConfig : ScriptableObject {
		public Color NormalColor = Color.white;
		public Color HighlightedColor = Color.white;
		public Color PressedColor = Color.white;
		public Color SelectedColor = Color.white;
		public Color DisabledColor = Color.white;
		public float ColorMultiplier = 1f;
		public float FadeDuration = 0.1f;

		public void ApplyToSelectable(Selectable selectable) {
			if (selectable == null) {
				return;
			}

			ColorBlock block = selectable.colors;
			block.normalColor = NormalColor;
			block.highlightedColor = HighlightedColor;
			block.pressedColor = PressedColor;
			block.selectedColor = SelectedColor;
			block.disabledColor = DisabledColor;
			block.colorMultiplier = ColorMultiplier;
			block.fadeDuration = FadeDuration;
			selectable.colors = block;
		}
	}
}