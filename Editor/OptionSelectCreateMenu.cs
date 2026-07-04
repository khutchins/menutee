using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Menutee.EditorTools {
	public static class OptionSelectCreateMenu {
		private const string HorizontalPrefabPath = "Runtime/UI/Prefabs/OptionSelect_Horizontal.prefab";
		private const string VerticalPrefabPath = "Runtime/UI/Prefabs/OptionSelect_Vertical.prefab";

		[MenuItem("GameObject/UI/Menutee/Option Select (Horizontal)", false, 10)]
		static void CreateHorizontal(MenuCommand cmd) => Create(cmd, HorizontalPrefabPath);

		[MenuItem("GameObject/UI/Menutee/Option Select (Vertical)", false, 11)]
		static void CreateVertical(MenuCommand cmd) => Create(cmd, VerticalPrefabPath);

		static void Create(MenuCommand cmd, string prefabRelativePath) {
			string path = $"{PackageRoot}/{prefabRelativePath}";
			var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
			if (prefab == null) {
				Debug.LogError($"Menutee: OptionSelect prefab not found at {path}.");
				return;
			}

			var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
			go.name = "OptionSelect";
			PrefabUtility.UnpackPrefabInstance(go, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

			PlaceInUI(go, cmd.context as GameObject);
			Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
			Selection.activeGameObject = go;
			EditorSceneManager.MarkSceneDirty(go.scene);
		}

		static string _packageRoot;
		static string PackageRoot {
			get {
				if (string.IsNullOrEmpty(_packageRoot)) {
					var info = UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(OptionSelectCreateMenu).Assembly);
					_packageRoot = info != null ? info.assetPath : "Packages/com.khutchins.menutee";
				}
				return _packageRoot;
			}
		}
		static void PlaceInUI(GameObject go, GameObject context) {
			GameObject parent = context;
			if (parent == null || parent.GetComponentInParent<Canvas>() == null) {
				parent = GetOrCreateCanvas().gameObject;
			}

			GameObjectUtility.SetParentAndAlign(go, parent);
			if (go.transform is RectTransform rt) {
				rt.anchoredPosition = Vector2.zero;
			}

			if (FindAny<EventSystem>() == null) {
				var es = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
				Undo.RegisterCreatedObjectUndo(es, "Create EventSystem");
			}
		}

		static Canvas GetOrCreateCanvas() {
			Canvas existing = FindAny<Canvas>();
			if (existing != null) return existing;

			var canvasGo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
			canvasGo.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
			canvasGo.layer = LayerMask.NameToLayer("UI");
			Undo.RegisterCreatedObjectUndo(canvasGo, "Create Canvas");
			return canvasGo.GetComponent<Canvas>();
		}

		static T FindAny<T>() where T : Object {
#if UNITY_2023_1_OR_NEWER
			return Object.FindFirstObjectByType<T>();
#else
			return Object.FindObjectOfType<T>();
#endif
		}
	}
}
