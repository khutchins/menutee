using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Linq;

namespace Menutee {
	public class MenuManager : MonoBehaviour, IMenu {

		private bool _active;

		[Tooltip("Background element.")]
		public GameObject BG;
		[HideInInspector]
		public PanelManager[] Panels;
		public MenuConfig MenuConfig;

		protected Stack<string> _panelStack = new Stack<string>();

		public EventSystem EventSystem;
		private UIElementManager _activeDefaultInput;

		public MenuInputMediator InputMediator;

		private bool _disabled;
		private string _activeKey;

		private void Start() {
			SetMenuUp(false);
			if (MenuStack.Shared == null) {
				Debug.LogError("No MenuStack in scene. Menu disabled.");
				_disabled = true;
				return;
			} else if (InputMediator == null) {
				Debug.LogError("No input mediator set on MenuManager. Menu disabled.");
				_disabled = true;
				return;
			}

			// Read Closeable in Start so that other scripts
			// can set it in Awake.
			if (!MenuConfig.Closeable) {
				MenuStack.Shared.PushAndShowMenu(this);
			}
		}

		public MenuAttributes GetMenuAttributes() {
			return MenuConfig.MenuPausesGame ? MenuAttributes.StandardPauseMenu() : MenuAttributes.StandardNonPauseMenu();
		}

		public void SetMenuUp(bool newUp) {
			_active = newUp;
			BG.SetActive(_active);
			ActivateMenu(_active ? MenuConfig.MainPanelKey : null);
		}

		public void SetMenuOnTop(bool newOnTop) {
		}

		void ToggleMenu() {
			if (!MenuConfig.Closeable || _disabled) {
				return;
			}
			MenuStack.Shared.ToggleMenu(this);
		}

		private void ActivateMenu(string key) {
			ActivateMenu(key, MenuConfig.PanelConfigs.Where(p => p.Key == key).FirstOrDefault());
		}

		private void ActivateMenu(string key, PanelConfig config) {
			PanelManager active = null;
			EventSystem.SetSelectedGameObject(null);
			string oldKey = _activeKey;

			if (key == null) {
				_activeKey = null;
			}
			foreach (PanelManager manager in Panels) {
				manager.SetPanelActive(key == manager.Key);
				if (key == manager.Key) {
					active = manager;
					_activeKey = key;
				}
			}
			if (active != null) {
				_activeDefaultInput = active.DefaultInput;
				if (_activeDefaultInput != null && _activeDefaultInput.SelectableObject != null) {
					EventSystem.SetSelectedGameObject(_activeDefaultInput.SelectableObject);
				}
			} else {
				_activeDefaultInput = null;
			}

			if (oldKey != _activeKey) {
				foreach (System.Action<string, string> action in MenuConfig.PanelChangeCallbacks) {
					action.Invoke(oldKey, _activeKey);
				}
			}
		}

		private void Update() {
			if (_disabled) return;
			if (InputMediator.PauseDown()) {
				ToggleMenu();
			} else if (InputMediator.UICancelDown()) {
				if (!IsAtRoot()) {
					PopMenu();
				}
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.F5)) {
				ScreenCapture.CaptureScreenshot("Screenshot" + System.DateTime.Now.ToString("__yyyy-MM-dd-HH-mm-ss") + ".png", 2);
			}
			if (_activeDefaultInput != null && EventSystem.currentSelectedGameObject == null && (Mathf.Abs(InputMediator.UIX()) > 0.1 || Mathf.Abs(InputMediator.UIY()) > 0.1)) {
				EventSystem.SetSelectedGameObject(_activeDefaultInput.SelectableObject);
			}
		}

		/// <summary>
		/// Goes to a menu, bypassing the stack. Used by push and pop after
		/// modifying the stack. Only use if you know what you're doing.
		/// </summary>
		/// <param name="key">Key of the menu to go to.</param>
		protected void GoToMenu(string key) {
			ActivateMenu(key);
		}

		public void PushMenu(string key) {
			foreach (PanelManager panel in Panels) {
				if (panel.Key == key) {
					_panelStack.Push(key);
					GoToMenu(key);
					return;
				}
			}
			Debug.LogError("Cannot push menu " + key + "! Not in Panels array.");
		}

		private bool IsAtRoot() {
			return _panelStack.Count == 0;
		}

		public void PopMenu() {
			if (_panelStack.Count > 0) {
				_panelStack.Pop();
			}
			if (_panelStack.Count == 0) {
				GoToMenu(MenuConfig.MainPanelKey);
			} else {
				GoToMenu(_panelStack.Last());
			}
		}

		public void ExitMenu() {
			if (!_active)
				return;
			ToggleMenu();
		}

		public void ExitGame() {
			Application.Quit();
		}
	}
}