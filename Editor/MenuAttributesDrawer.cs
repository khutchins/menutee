using UnityEditor;
using UnityEngine;

namespace Menutee.EditorTools {
	[CustomPropertyDrawer(typeof(MenuAttributes))]
	public class MenuAttributesDrawer : PropertyDrawer {
		static readonly GUIContent OverrideLabel = new GUIContent("Override Time Scale",
			"If enabled, sets Time.timeScale while the menu is up. If disabled, the existing time scale is left alone.");
		static readonly GUIContent TimeScaleLabel = new GUIContent("Time Scale", "Time scale to apply while the menu is up.");

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);

			SerializedProperty lockMode = property.FindPropertyRelative("cursorLockMode");
			SerializedProperty visible = property.FindPropertyRelative("cursorVisible");
			SerializedProperty pause = property.FindPropertyRelative("pauseGame");
			SerializedProperty timeScale = property.FindPropertyRelative("timeScale");

			float line = EditorGUIUtility.singleLineHeight;
			float step = line + EditorGUIUtility.standardVerticalSpacing;
			Rect rect = new Rect(position.x, position.y, position.width, line);

			EditorGUI.PropertyField(rect, lockMode);
			rect.y += step;
			EditorGUI.PropertyField(rect, visible);
			rect.y += step;
			EditorGUI.PropertyField(rect, pause);
			rect.y += step;

			bool overrides = timeScale.floatValue >= 0f;
			EditorGUI.BeginChangeCheck();
			bool newOverrides = EditorGUI.Toggle(rect, OverrideLabel, overrides);
			if (EditorGUI.EndChangeCheck() && newOverrides != overrides) {
				// Off writes the negative sentinel; on sets 0.
				timeScale.floatValue = newOverrides ? 0f : -1f;
			}

			if (newOverrides) {
				rect.y += step;
				EditorGUI.indentLevel++;
				EditorGUI.BeginChangeCheck();
				float value = EditorGUI.FloatField(rect, TimeScaleLabel, Mathf.Max(0f, timeScale.floatValue));
				if (EditorGUI.EndChangeCheck()) {
					timeScale.floatValue = Mathf.Max(0f, value);
				}
				EditorGUI.indentLevel--;
			}

			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			float step = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			// cursorLockMode, cursorVisible, pauseGame, override toggle.
			int lines = 4;
			// Conditional time scale field.
			if (property.FindPropertyRelative("timeScale").floatValue >= 0f) lines += 1;
			return lines * step - EditorGUIUtility.standardVerticalSpacing;
		}
	}
}
