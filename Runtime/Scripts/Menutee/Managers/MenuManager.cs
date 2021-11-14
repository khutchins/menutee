using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

namespace Menutee {
	public class MenuManager : MonoBehaviour, IMenu {

		private bool _active;

		[Tooltip("The canvas element. If this is not set, this script will try to get the canvas from the game object it is on.")]
		public Canvas Canvas;

		[HideInInspector]
		public PanelManager[] Panels;
		public MenuConfig MenuConfig;

		protected Stack<string> _panelStack = new Stack<string>();
		private UIElementManager _activeDefaultInput;

		public MenuInputMediator InputMediator;

		private bool _disabled;
		private string _activeKey;
		private PanelManager _activeManager;
		private GameObject _cachedSelection;

		private void Awake() {
			if (Canvas == null) {
				Canvas = GetComponent<Canvas>();
			}
		}

		private void Start() {
			SetMenuUp(false);

			// This needs to be checked on Start to guarantee
			// that MenuStack has had time to register its singleton.
			if (MenuStack.Shared == null) {
				Debug.LogError("No MenuStack in scene. Menu disabled.");
				_disabled = true;
				return;
			} else if (InputMediator == null) {
				Debug.LogError("No input mediator set on MenuManager. Menu disabled.");
				_disabled = true;
				return;
			} else if (Canvas == null) {
				Debug.LogError("No canvas element set or found in MenuManager. Menu disabled.");
			}

			if (MenuConfig.StartsOpen) {
				MenuStack.Shared.PushAndShowMenu(this);
			}
		}

		public MenuAttributes GetMenuAttributes() {
			return MenuConfig.MenuPausesGame ? MenuAttributes.StandardPauseMenu() : MenuAttributes.StandardNonPauseMenu();
		}

		public void SetMenuUp(bool newUp) {
			// Menu is closed.
			if (!newUp) {
				_cachedSelection = null;
				_panelStack.Clear();
			}
			_active = newUp;
			Canvas.enabled = newUp;
			ActivatePanel(_active ? MenuConfig.MainPanelKey : null);
		}

		public void SetMenuOnTop(bool newOnTop) {
			if (!newOnTop) {
				_cachedSelection = EventSystem.current.currentSelectedGameObject;
				EventSystem.current.SetSelectedGameObject(null);
			} else if(_cachedSelection != null) {
				EventSystem.current.SetSelectedGameObject(_cachedSelection);
			}
			_activeManager?.SetPanelActive(newOnTop);
			Canvas.enabled = newOnTop;
		}

		void ToggleMenu() {
			if (!MenuConfig.Toggleable || _disabled) {
				return;
			}
			MenuStack.Shared.ToggleMenu(this);
		}

		private void ActivatePanel(string key) {
			ActivateMenu(key, MenuConfig.PanelConfigs.Where(p => p.Key == key).FirstOrDefault());
		}

		private void ActivateMenu(string key, PanelConfig config) {
			PanelManager active = null;
			EventSystem.current.SetSelectedGameObject(null);
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
					EventSystem.current.SetSelectedGameObject(_activeDefaultInput.SelectableObject);
				}
			} else {
				_activeDefaultInput = null;
			}

			if (oldKey != _activeKey) {
				foreach (System.Action<string, string> action in MenuConfig.PanelChangeCallbacks) {
					action.Invoke(oldKey, _activeKey);
				}
			}
			_activeManager = active;
		}

		private void Update() {
			if (_disabled) return;
			if (InputMediator.MenuToggleDown()) {
				ToggleMenu();
			} else if (InputMediator.UICancelDown()) {
				if (!IsAtRoot()) {
					PopPanel();
				}
			}
			if (_activeDefaultInput != null && EventSystem.current.currentSelectedGameObject == null && (Mathf.Abs(InputMediator.UIX()) > 0.1 || Mathf.Abs(InputMediator.UIY()) > 0.1)) {
				EventSystem.current.SetSelectedGameObject(_activeDefaultInput.SelectableObject);
			}
		}

		/// <summary>
		/// Goes to a panel, bypassing the stack. Used by push and pop after
		/// modifying the stack. Only use if you know what you're doing.
		/// </summary>
		/// <param name="key">Key of the panel to go to.</param>
		protected void GoToPanel(string key) {
			ActivatePanel(key);
		}

		public void PushPanel(string key) {
			foreach (PanelManager panel in Panels) {
				if (panel.Key == key) {
					_panelStack.Push(key);
					GoToPanel(key);
					return;
				}
			}
			Debug.LogErrorFormat("Cannot push panel {0}! Not in Panels array.", key);
		}

		private bool IsAtRoot() {
			return _panelStack.Count == 0;
		}

		public void PopPanel() {
			if (_panelStack.Count > 0) {
				_panelStack.Pop();
			}
			if (_panelStack.Count == 0) {
				GoToPanel(MenuConfig.MainPanelKey);
			} else {
				GoToPanel(_panelStack.Last());
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