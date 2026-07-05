using UnityEditor;
using UnityEngine;

namespace Menutee.EditorTools {
	[CustomEditor(typeof(MenuHook))]
	[CanEditMultipleObjects]
	public class MenuHookEditor : Editor {
		public override void OnInspectorGUI() {
			serializedObject.Update();

			SerializedProperty mode = serializedObject.FindProperty(nameof(MenuHook.InteractabilityMode));
			SerializedProperty canvas = serializedObject.FindProperty(nameof(MenuHook.Canvas));
			SerializedProperty useDefaultOnPush = serializedObject.FindProperty(nameof(MenuHook.UseDefaultOnPush));
			SerializedProperty behaviorOnPop = serializedObject.FindProperty(nameof(MenuHook.BehaviorOnPop));
			SerializedProperty restoreDefaultOnInput = serializedObject.FindProperty(nameof(MenuHook.RestoreDefaultOnInputIfNoneSelected));

			SerializedProperty prop = serializedObject.GetIterator();
			bool enterChildren = true;
			while (prop.NextVisible(enterChildren)) {
				enterChildren = false;

				if (prop.name == "m_Script") {
					using (new EditorGUI.DisabledScope(true)) {
						EditorGUILayout.PropertyField(prop);
					}
					continue;
				}

				if (!ShouldShow(prop, mode, canvas, useDefaultOnPush, behaviorOnPop, restoreDefaultOnInput)) {
					continue;
				}

				EditorGUILayout.PropertyField(prop, true);
			}

			serializedObject.ApplyModifiedProperties();
		}

		// Hides fields that don't apply given the current settings.
		static bool ShouldShow(SerializedProperty prop, SerializedProperty mode, SerializedProperty canvas,
				SerializedProperty useDefaultOnPush, SerializedProperty behaviorOnPop, SerializedProperty restoreDefaultOnInput) {
			if (prop.name == nameof(MenuHook.CanvasGroup)) {
				return mode.hasMultipleDifferentValues
					|| (MenuHook.CanvasGroupMode)mode.enumValueIndex == MenuHook.CanvasGroupMode.Reference;
			}
			if (prop.name == nameof(MenuHook.HideIfNotOnTop)) {
				return canvas.hasMultipleDifferentValues || canvas.objectReferenceValue != null;
			}
			if (prop.name == nameof(MenuHook.DefaultSelectedGameObject)) {
				return useDefaultOnPush.hasMultipleDifferentValues || useDefaultOnPush.boolValue
					|| behaviorOnPop.hasMultipleDifferentValues
					|| (MenuHook.SelectedBehavior)behaviorOnPop.enumValueIndex == MenuHook.SelectedBehavior.SetDefault
					|| restoreDefaultOnInput.hasMultipleDifferentValues || restoreDefaultOnInput.boolValue;
			}
			if (prop.name == nameof(MenuHook.InputMediator)) {
				return restoreDefaultOnInput.hasMultipleDifferentValues || restoreDefaultOnInput.boolValue;
			}
			return true;
		}
	}
}
