# Changelog

## [6.0.0]

### Breaking
* `OptionSelectManager` now operates on an `OperatorSelect` `Selectable` instead of the prior conglomeration. You can make a new one for your prefab with `Create/UI/Menutee/Option Select`.
* `MenuManager`'s animation hooks are no longer overridable: `SetMenuIsUp`, `SetOnTop`, and `ActivatePanel(PanelManager, PanelManager, bool)` are now `private` (was `protected virtual`), and `DoToggle` was removed. Menus that animated by subclassing `MenuManager` should transition to the new transition system (see `MenuWithAnimationSample`).
* Removed `OptionSelectRefConfig.Builder.SetToggleManager`. It was a copy-paste of the toggle handler onto the non-toggle int-ref option select. Use `SetOptionSelectedHandler`, or `OptionSelectToggleRefConfig` for an actual toggle.
* Removed the obsolete `HighlightTextWhenSelected` component. Use `MirrorSelectable` instead.
* Renamed `MenuConfig.PaletteConfig` to `DefaultPaletteConfig`.
* Renamed `SetToggleManager` to `SetToggleHandler` on option select toggle components.
* Renamed `MenuConfig.Builder.SetSelectedRestorationMode` to `SetSelectionRestorationMode`, matching the `MenuConfig.SelectionRestorationMode` field.
* `ExitGame()` removed from `MenuManager`.

### Deprecated
* `MenuConfig.Builder(bool, bool, PaletteConfig)`. Use `MenuConfig.Builder(bool, bool)` with `SetDefaultPaletteConfig` (or `SetDefaultPaletteReference`) instead.

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