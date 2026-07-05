using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Menutee {
	public class MenuHook : MonoBehaviour, IMenu {

		public enum SelectedBehavior {
			None,
			Restore,
			SetDefault
		}

		public enum CanvasGroupMode {
			// Grabs an existing CanvasGroup on this object, or adds one at runtime
			// if there isn't one. Best for menus that don't have a CanvasGroup wired
			// up ahead of time, including ones generated dynamically at runtime.
			Auto,
			// Drives the CanvasGroup assigned to the CanvasGroup field.
			Reference,
			// Doesn't manage interactability through a CanvasGroup.
			None
		}

		[Header("General")]
		[Tooltip("The canvas element. If this is set, this script will automatically hide/show the canvas when appropriate.")]
		public Canvas Canvas;
		[Tooltip("How the menu controls interactability (blocking and receiving input) when it isn't the top menu. Auto grabs or adds a CanvasGroup on this object at runtime, Reference uses the one you assign, and None disables the behavior. The CanvasGroup should be on the root of the menu so it covers everything.")]
		public CanvasGroupMode InteractabilityMode = CanvasGroupMode.Auto;
		[Tooltip("CanvasGroup used to toggle interactability. Only used when Interactability Mode is set to Reference.")]
		public CanvasGroup CanvasGroup;
		[Tooltip("Whether or not this canvas should be shown by default.")]
		public bool ShowOnStart = false;
		[Tooltip("If this is true, it will hide the canvas except when it is not on top.")]
		public bool HideIfNotOnTop = false;

		[Header("Menu Attributes")]
		public MenuAttributes Attributes = new MenuAttributes {
			cursorLockMode = CursorLockMode.None,
			cursorVisible = true,
			pauseGame = true,
			timeScale = 0
		};

		[Header("Selected Behavior")]
		[Tooltip("Whether the default selected game object should be selected when the menu is pushed.")]
		public bool UseDefaultOnPush;
		[Tooltip("Behavior to use when the menu is popped back to.")]
		public SelectedBehavior BehaviorOnPop = SelectedBehavior.Restore;
		[Tooltip("Whether or not the selected game object should be cleared when this menu is not on top or disappears.")]
		public bool ClearSelectedOnNotOnTop = false;
		[Tooltip("Whether or not the default selected game object should be restored if no objects are selected, a direction is pressed, and this menu is on top.")]
		public bool RestoreDefaultOnInputIfNoneSelected = false;
		[Tooltip("GameObject to default to when using the \"SetDefault\" behavior.")]
		public GameObject DefaultSelectedGameObject;
		[Tooltip("Input mediator to use to detect input if restore default on input is enabled.")]
		public MenuInputMediator InputMediator;

		[Header("Hooks")]
		[Tooltip("If this is true, OnMenuClose callbacks will be called on startup if it doesn't start shown.")]
		public bool CallCloseCallbackOnStart = true;
		[Tooltip("If this is true, OnMenuNotTop callbacks will be called on startup if it doesn't start shown.")]
		public bool CallNotTopCallbackOnStart = true;
		[Tooltip("Callbacks for when the menu is shown.")]
		public UnityEvent OnMenuOpen;
		[Tooltip("Callbacks for when the menu is hidden.")]
		public UnityEvent OnMenuClose;
		[Tooltip("Callbacks for when the menu has become the top menu on the stack.")]
		public UnityEvent OnMenuTop;
		[Tooltip("Callbacks for when the menu is no longer the top menu on the stack.")]
		public UnityEvent OnMenuNotTop;

		private GameObject _cachedSelection;
		private bool _isOnTop = false;
		private float _lastTimeOnTop;
		private float _lastTimeOpen;
		private CanvasGroup _canvasGroup;

		void Awake() {
			if (Canvas != null) Canvas.enabled = false;
			_canvasGroup = ResolveCanvasGroup();
			// Start non-interactable; a push will make it interactable via SetMenuOnTop.
			SetInteractable(false);
		}

		private CanvasGroup ResolveCanvasGroup() {
			switch (InteractabilityMode) {
				case CanvasGroupMode.Auto:
					CanvasGroup group = GetComponent<CanvasGroup>();
					if (group == null) group = gameObject.AddComponent<CanvasGroup>();
					return group;
				case CanvasGroupMode.Reference:
					return CanvasGroup;
				case CanvasGroupMode.None:
				default:
					return null;
			}
		}

		private void SetInteractable(bool interactable) {
			if (_canvasGroup == null) return;
			_canvasGroup.interactable = interactable;
			_canvasGroup.blocksRaycasts = interactable;
		}

		void Start() {
			if (ShowOnStart) {
				PushMenu();
			} else {
				if (CallNotTopCallbackOnStart) OnMenuNotTop?.Invoke();
				if (CallCloseCallbackOnStart) OnMenuClose?.Invoke();
			}
		}

		public MenuAttributes GetMenuAttributes() {
			return Attributes;
		}

		/// <summary>
		/// True if the menu became up on this frame. Useful for not accidentally 
		/// double triggering on button down across menus.
		/// </summary>
		public bool IsNewlyUp {
			get => _lastTimeOpen == Time.unscaledTime;
        }

		/// <summary>
		/// True if the menu became on top this frame. Useful for not accidentally 
		/// double triggering on button down across menus.
		/// </summary>
		public bool IsNewlyOnTop {
			get => _lastTimeOnTop == Time.unscaledTime;
		}

		public void SetMenuUp(bool newUp) {
			if (Canvas != null) Canvas.enabled = newUp;
			if (newUp) {
				_lastTimeOpen = Time.unscaledTime;
				OnMenuOpen?.Invoke();

				if (UseDefaultOnPush) {
					SetSelectedGameObjectFromBehavior(SelectedBehavior.SetDefault);
				}
			} else {
				OnMenuClose?.Invoke();
				_cachedSelection = null;
			}
		}

		public void SetMenuOnTop(bool newOnTop) {
			if (HideIfNotOnTop) Canvas.enabled = newOnTop;
			SetInteractable(newOnTop);
			if (newOnTop) {
				_lastTimeOnTop = Time.unscaledTime;
				OnMenuTop?.Invoke();
			} else OnMenuNotTop?.Invoke();

			_isOnTop = newOnTop;
			if (!newOnTop) {
				_cachedSelection = EventSystem.current.currentSelectedGameObject;
				if (ClearSelectedOnNotOnTop) {
					EventSystem.current.SetSelectedGameObject(null);
				}
			} else {
				// If cached selection is not null, it means that this is from a pop, not a push.
				if (_cachedSelection != null) {
					SetSelectedGameObjectFromBehavior(BehaviorOnPop);
				}
			}
		}

		private void SetSelectedGameObjectFromBehavior(SelectedBehavior behavior) {
			switch (behavior) {
				case SelectedBehavior.Restore:
					EventSystem.current.SetSelectedGameObject(_cachedSelection);
					break;
				case SelectedBehavior.SetDefault:
					EventSystem.current.SetSelectedGameObject(DefaultSelectedGameObject);
					break;
				case SelectedBehavior.None:
				default:
					break;
			}
		}

		public void PushMenu() {
			MenuStack.Shared.PushAndShowMenu(this);
		}

		public void PushOtherMenu(MenuHook other) {
			MenuStack.Shared.PushAndShowMenu(other);
		}

		public void PushOtherMenu(MenuManager other) {
			MenuStack.Shared.PushAndShowMenu(other);
		}

		public void PopMenu() {
			MenuStack.Shared.PopAndCloseMenu(this);
		}

		void Update() {
			if (_isOnTop && RestoreDefaultOnInputIfNoneSelected 
				&& EventSystem.current.currentSelectedGameObject == null 
				&& (Mathf.Abs(InputMediator.UIX()) > 0.1 || Mathf.Abs(InputMediator.UIY()) > 0.1)) {
				EventSystem.current.SetSelectedGameObject(DefaultSelectedGameObject);
			}
		}

		public IEnumerator WaitForClose() {
			yield return new AwaitMenuClose(this).WaitForClose();
        }

		private class AwaitMenuClose {
			public bool Finished { get => _finished; }
			public IEnumerator WaitForClose() {
				while (!Finished) yield return null;
            }

			MenuHook _menu;
			UnityAction _action;
			bool _finished;

			public AwaitMenuClose(MenuHook menu) {
				this._menu = menu;
				_action = new UnityAction(() => {
					_finished = true;
					_menu.OnMenuClose.RemoveListener(_action);
				});
				_menu.OnMenuClose.AddListener(_action);
			}
		}
	}
}