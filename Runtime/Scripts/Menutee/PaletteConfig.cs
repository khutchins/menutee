using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Menutee {
	[CreateAssetMenu]
	public class PaletteConfig : ScriptableObject {
		public Color NormalColor = Color.white;
		public Color HighlightedColor = Color.white;
		public Color PressedColor = Color.white;
		public Color SelectedColor = Color.white;
		public Color DisabledColor = Color.white;

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
			selectable.colors = block;
		}
	}
}