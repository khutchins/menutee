# Changelog

## [6.0.0]

### Breaking
* `OptionSelectManager` now operates on an `OperatorSelect` `Selectable` instead of the prior conglomeration. You can make a new one for your prefab with `Create/UI/Menutee/Option Select`.
* `MenuManager`'s animation hooks are no longer overridable: `SetMenuIsUp`, `SetOnTop`, and `ActivatePanel(PanelManager, PanelManager, bool)` are now `private` (was `protected virtual`), and `DoToggle` was removed. Menus that animated by subclassing `MenuManager` should transition to the new transition system (see `MenuWithAnimationSample`).
* Removed `OptionSelectRefConfig.Builder.SetToggleManager`. It was a copy-paste of the toggle handler onto the non-toggle int-ref option select. Use `SetOptionSelectedHandler`, or `OptionSelectToggleRefConfig` for an actual toggle.
* Removed the deprecated `HighlightTextWhenSelected` component. Use `MirrorSelectable` instead.
* Renamed `MenuConfig.PaletteConfig` to `DefaultPaletteConfig`.
* Renamed `SetToggleManager` to `SetToggleHandler` on option select toggle components.
* `ExitGame()` removed from `MenuManager`.
* `PanelConfig`'s constructor is now private. Build panels through `PanelConfig.Builder`.
* `MenuGenerator` is now lean: the prefabs slots and the `PaletteConfig` field moved off it (it was making assumptions about the shape you'd want to use), and the public `PanelDictionary`/`PanelObjectDictionary` were removed, as they were vestigial. If your generator subclass referenced those prefab slots, change its base class from `MenuGenerator` to `StarterMenuGenerator` to fix the compile errors. Expectation moving forward is to declare the ones you want.
* `MenuHook` now manages interactability through a CanvasGroup, defaulting to `InteractabilityMode.Auto`, which grabs or adds a CanvasGroup on the hook's object at runtime and makes the menu non-interactable whenever it isn't the top menu. Previously menus underneath the stack still received input. Set `Interactability Mode` to `None` to restore the old behavior.
* `MenuHook`'s `CursorLockMode`, `CursorVisible`, `PausesGame`, and `TimeScale` fields were consolidated into a single serialized `MenuAttributes Attributes` field. Existing MenuHooks will reset these values to defaults, so re-check the Menu Attributes on any MenuHook you've customized. Code that read those fields directly should switch to `hook.Attributes.cursorLockMode` (etc.).

### Deprecated
* `MenuConfig.Builder(bool, bool, PaletteConfig)`. Use `MenuConfig.Builder(bool, bool)` with `SetDefaultPaletteConfig` (or `SetDefaultPaletteReference`) instead.
* `MenuConfig.Builder.SetSelectedRestorationMode`. Renamed to `SetSelectionRestorationMode`, matching the `MenuConfig.SelectionRestorationMode` field. The old name forwards to it for now.
* `PanelConfig.Builder.SetCustomNavigation(Action<List<Selectable>>)`. Use the `Action<PanelManager, List<Selectable>>` overload, which also exposes the PanelManager for reading non-generated selectables. The old overload forwards to it.
* `MenuConfig.AddPanelChangeCallback(string,string)` is deprecated. Calls should be switched to the PanelManager version.

### Fixed
* `OptionSelectMappedConfig<T>.Builder.Build()` now returns an actual `OptionSelectMappedConfig<T>` (carrying its `Options` array) instead of a plain `OptionSelectConfig`.
* `OptionSelectConfig` builders' `SetHidesArrowIfLastOption` is now applied to the generated `OptionSelect` instead of being ignored.

### Added
* `MenuConfig.Builder.SetDefaultPaletteConfig` / `SetDefaultPaletteReference` set a menu-level default palette. A reference re-themes every element that uses the default whenever it changes. Element palette precedence is: object reference > object config > menu default reference > menu default config.
* `PanelObjectConfig.Builder.SetPaletteReferenceOverride` binds an element to a `PaletteConfigReference`, so it re-themes live whenever the referenced palette changes (and applies the reference's current value on creation). Previously the `PaletteReference` field existed but was never settable through the builder or read during generation.
* `OptionSelect`: a Selectable subclass that behaves like the combination of features I had worked before.
* `GameObject > UI > Menutee > Option Select (Horizontal/Vertical)` create-menu entries that drop the shipped OptionSelect prefabs into the scene (creating a Canvas/EventSystem if needed).
* Default arrow sprites (left/right/up/down) and OptionSelect prefabs shipped in the package.
* Configurable menu animation system replacing the old subclassing approach. `IMenuVisibilityTransition` (menu show/hide) and `IPanelTransition` (panel push/pop) are assigned via the `MenuConfig`/`PanelConfig` builders, with per-panel transitions overriding a menu-level default.
* Added menu transitions: `FadeMenuTransition`, `SlideMenuTransition`
* Added panel transitions: `FadePanelTransition`, `SlidePanelTransition`, and `StaggeredSlidePanelTransition`.
* Panel objects no longer require a key to be specified.
* `MenuConfig.AddPanelChangeCallback(PanelManager,PanelManager)`.
* Focus tracking. The menu now resolves which single element is "focused" - the most recently selected or highlighted candidate, available at the menu, panel, and element level. Generated elements participate automatically.
  * Per-object: `PanelObjectConfig.Builder.AddFocusChangedCallback(Action<ElementFocusChange>)` fires as an element gains/loses focus; `AddInteractionStateCallback(Action<InteractionStateChange>)` reports its raw local transitions.
  * Per-panel: `PanelConfig.Builder.AddFocusedElementChangedCallback(Action<FocusChange>)`.
  * Menu-wide: `MenuConfig.Builder.AddFocusedElementChangedCallback(Action<FocusChange>)`.
  * `PanelManager.RegisterFocusSource` / `RegisterFocusSources` / `UnregisterFocusSource` opt external selectables (e.g. ones baked into a panel prefab) into the system.
* Navigation-state queries on `MenuManager`: `IsAtRoot()` (whether the active panel can be popped) and `GetPanelPath()` (root-to-active panel keys, for breadcrumbs). `PanelManager.Selectables` now exposes the panel's generated selectables, so an `OnDisplayCallback` can rebuild navigation (e.g. to conditionally shot back button if not at root). The `MenuWithCustomBack` sample uses these to share one back-button prefab across all panels.
* `MirrorSelectable.SelfManagedPalette`: when enabled, the component keeps its own serialized `Palette` and ignores palettes applied during generation. Lets part of a composite element (e.g. a shared background) stay on a fixed palette while the rest receives the element's palette.
* `MenuHook.InteractabilityMode` (`Auto`/`Reference`/`None`) controls whether the menu uses a CanvasGroup's `interactable`/`blocksRaycasts` to disable input when it isn't on top. `Auto` grabs or adds a CanvasGroup at runtime, `Reference` uses one you assign, and `None` opts out.
* Custom inspector for `MenuHook` that hides fields that don't currently apply (e.g. `Input Mediator` only shows when restore-on-input is enabled, `CanvasGroup` only in `Reference` mode).
* `MenuAttributes` now has a property drawer that presents the time scale as an "Override Time Scale" toggle instead of the negative-value sentinel, and it's `[System.Serializable]` so it can be edited directly in the inspector.

## [5.2.1]

### Changed
* Component prefabs include UniformInteraction by default.
* Component prefabs had a pass to make them more generally robust.

### Fixed
* OptionSelectManager now handles null palette config correctly.
* DropdownManager no longer uses null propagation on ScriptableObject.
* GeneralMenu sample no longer throws due to missing verify.
* GeneralMenu no longer has deprecation warning on refreshRatio usage.

[5.2.1]: https://github.com/khutchins/Menutee/releases/tag/v5.2.1