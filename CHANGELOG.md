# Changelog

## [6.0.0]

### Breaking
* `OptionSelectManager` now operates on an `OperatorSelect` `Selectable` instead of the prior conglomeration. You can make a new one for your prefab with `Create/UI/Menutee/Option Select`.

### Added
* `OptionSelect`: a Selectable subclass that behaves like the combination of features I had worked before.
* `GameObject > UI > Menutee > Option Select (Horizontal/Vertical)` create-menu entries that drop the shipped OptionSelect prefabs into the scene (creating a Canvas/EventSystem if needed).
* Default arrow sprites (left/right/up/down) and OptionSelect prefabs shipped in the package.

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