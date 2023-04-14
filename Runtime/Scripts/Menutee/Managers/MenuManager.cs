using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using System.Collections;

namespace Menutee {
	public class MenuManager : MonoBehaviour, IMenu {

		private bool _active;

		[Tooltip("The canvas element. If this is not set, this script will try to get the canvas from the game object it is on.")]
		public Canvas Canvas;

		[HideInInspector]
		public PanelManager[] Panels;
		public MenuConfig MenuConfig;

		protected Stack<string> _panelStack = new Stack<string>();
		protected Stack<GameObject> _selectedStack = new Stack<GameObject>();
		private GameObject _activeDefaultInput;

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
			return MenuConfig.MenuAttributes;
		}

		/// <summary>
		/// Delays actions in the helper for a frame, since some things *COUGH* audio mixers *COUGH*
		/// don't handle being set in awake properly. Useful for calling things from creation callbacks
		/// for objects.
		/// </summary>
		/// <param name="doer">Thing to do after a frame.</param>
		public void RunGenericActionAfterFrame(Action doer) {
			StartCoroutine(GenericCoroutine(doer));
        }

		private IEnumerator GenericCoroutine(Action doer) {
			yield return null;
			doer?.Invoke();
		}

		public void SetMenuUp(bool newUp) {
			// Menu is closed.
			if (!newUp) {
				_cachedSelection = null;
				_panelStack.Clear();
			}
			_active = newUp;
			SetMenuIsUp(newUp, _active ? MenuConfig.MainPanelKey : null);
		}

		public void SetMenuOnTop(bool newOnTop) {
			if (!newOnTop) {
				_cachedSelection = EventSystem.current.currentSelectedGameObject;
				EventSystem.current.SetSelectedGameObject(null);
			} else if(_cachedSelection != null) {
				EventSystem.current.SetSelectedGameObject(_cachedSelection);
			}
			SetOnTop(newOnTop);
		}

		protected virtual void SetMenuIsUp(bool isUp, string newKey) {
			Canvas.enabled = isUp;
			ActivatePanel(newKey, true);
		}

		protected virtual void SetOnTop(bool isOnTop) {
			_activeManager?.SetPanelActive(isOnTop);
			Canvas.enabled = isOnTop;
		}

		void ToggleMenu() {
			if (!MenuConfig.Toggleable || _disabled) {
				return;
			}
			MenuStack.Shared.ToggleMenu(this);
		}

		protected virtual void DoToggle() {
			MenuStack.Shared.ToggleMenu(this);
		}

		protected virtual void ActivatePanel(PanelManager oldPanel, PanelManager newPanel, bool fromPush) {
			EnablePanel(newPanel, fromPush);
			DisableOtherPanels(newPanel);
		}

		protected void EnablePanel(PanelManager panel, bool fromPush) {
			EventSystem.current.SetSelectedGameObject(null);
			string oldKey = _activeKey;
			_activeKey = panel != null ? panel.Key : null;

			PanelManager active = panel;
			active?.SetPanelActive(true);

			if (active != null) {
				// Cache the default so that we have something to default to if
				// nothing is selected.
				_activeDefaultInput = active.DefaultInput;

				// If pushing or something is wrong with the selected stack, use the default.
				if (fromPush || _selectedStack.Count == 0 || _selectedStack.Peek() == null) {
					if (_activeDefaultInput != null) {
						EventSystem.current.SetSelectedGameObject(_activeDefaultInput);
					}
				} 
				// Otherwise, restore the previous selection.
				else {
					GameObject selected = _selectedStack.Pop();
					EventSystem.current.SetSelectedGameObject(selected);
                }
			} else {
				_activeDefaultInput = null;
			}
			_activeManager = active;

			if (oldKey != _activeKey) {
				foreach (System.Action<string, string> action in MenuConfig.PanelChangeCallbacks) {
					action.Invoke(oldKey, _activeKey);
				}
			}
		}

		protected void DisableOtherPanels(PanelManager active) {
			foreach (PanelManager manager in Panels) {
				if (active != manager) {
					manager.SetPanelActive(false);
                }
			}
		}

		private PanelManager PanelForKey(string key) {
			return Panels.Where(x => x.Key == key).FirstOrDefault();
		}

		protected void ActivatePanel(string key, bool fromPush) {
			ActivatePanel(PanelForKey(_activeKey), PanelForKey(key), fromPush);
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
				EventSystem.current.SetSelectedGameObject(_activeDefaultInput);
			}
		}

		/// <summary>
		/// Goes to a panel, bypassing the stack. Used by push and pop after
		/// modifying the stack. Only use if you know what you're doing.
		/// </summary>
		/// <param name="key">Key of the panel to go to.</param>
		/// <param name="fromPush">Whether the panel is from a push or a pop. Used for animations.</param>
		protected void GoToPanel(string key, bool fromPush) {
			ActivatePanel(key, fromPush);
		}

		public void PushPanel(string key) {
			foreach (PanelManager panel in Panels) {
				if (panel.Key == key) {
					_selectedStack.Push(EventSystem.current.currentSelectedGameObject);
					_panelStack.Push(key);
					GoToPanel(key, true);
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
				GoToPanel(MenuConfig.MainPanelKey, false);
			} else {
				GoToPanel(_panelStack.Last(), false);
			}
		}

		/// <summary>
		/// Pops the current panel if not at root, otherwise hides the menu.
		/// </summary>
		public void PopPanelOrExitMenu() {
			if (IsAtRoot()) {
				ExitMenu();
            } else {
				PopPanel();
            }
        }

		public void ShowMenu() {
			if (_active) return;
			ToggleMenu();
        }

		public void ExitMenu() {
			if (!_active) return;
			ToggleMenu();
		}

		public void ExitGame() {
			Application.Quit();
		}
	}
}