using System.Collections.Generic;
using UnityEngine;

namespace Menutee {
    public class MenuConfig {
        public readonly bool Toggleable;
        public readonly bool StartsOpen;
        public readonly bool MenuPausesGame;
        public readonly RestorationMode SelectionRestorationMode;
        public readonly SelectMode DefaultSelectMode;
        public readonly string MainPanelKey;
        public readonly MenuAttributes MenuAttributes;
        /// <summary>
        /// Menu-level default palette, applied to any element that doesn't specify its
        /// own palette. Overridden by <see cref="DefaultPaletteReference"/> if set.
        /// </summary>
        public readonly PaletteConfig DefaultPaletteConfig;
        /// <summary>
        /// Menu-level default palette reference. Elements that don't specify their own
        /// palette re-theme live when this reference changes. Takes precedence over
        /// <see cref="DefaultPaletteConfig"/>.
        /// </summary>
        public readonly PaletteConfigReference DefaultPaletteReference;
        public readonly PanelConfig[] PanelConfigs;
        public readonly PanelGenerator[] PanelGenerators;
        public readonly List<System.Action<string, string>> PanelChangeCallbacks;
        /// <summary>
        /// Animation for the menu showing (opening). Null means an instant open.
        /// See <see cref="IMenuVisibilityTransition"/>.
        /// </summary>
        public readonly IMenuVisibilityTransition MenuInTransition;
        /// <summary>
        /// Animation for the menu hiding (closing). Null means an instant close.
        /// See <see cref="IMenuVisibilityTransition"/>.
        /// </summary>
        public readonly IMenuVisibilityTransition MenuOutTransition;
        /// <summary>
        /// Fallback animation for panel changes, used by any panel that does not
        /// set its own <see cref="PanelConfig.Transition"/>. Null means instant.
        /// See <see cref="IPanelTransition"/>.
        /// </summary>
        public readonly IPanelTransition DefaultPanelTransition;
        /// <summary>How the root panel's enter composes with the menu opening. See <see cref="MenuPanelSequence"/>.</summary>
        public readonly MenuPanelSequence MenuOpenSequence;
        /// <summary>How the active panel's exit composes with the menu closing. See <see cref="MenuPanelSequence"/>.</summary>
        public readonly MenuPanelSequence MenuCloseSequence;

        /// <summary>
        /// Controls whether an element is selected by default when a menu is entered
        /// or returned to. Without a default selection, the user must move the
        /// controller/keyboard to select anything.
        /// </summary>
        public enum SelectMode {
            /// <summary>Always select an element by default.</summary>
            Always,
            /// <summary>Select an element only if the last input was not the mouse.</summary>
            Contextual,
            /// <summary>Never select an element by default.</summary>
            Never,
        }

        /// <summary>
        /// Controls whether a selection is automatically restored after it is lost
        /// (e.g. the selected element is destroyed or deselected).
        /// </summary>
        public enum RestorationMode {
            /// <summary>Instantly restore a selection whenever none is selected.</summary>
            Always,
            /// <summary>Restore a selection on the next directional (next/previous) input.</summary>
            OnInput,
            /// <summary>Never automatically restore a selection.</summary>
            Never,
        }

        MenuConfig(bool toggleable, bool startsOpen, bool menuPausesGame, string mainPanelKey, PaletteConfig paletteConfig,
                PanelConfig[] panelConfigs, PanelGenerator[] panelGenerators = null,
                List<System.Action<string, string>> panelChangeCallbacks = null,
                MenuAttributes? menuAttributesOverride = null, SelectMode selectMode = SelectMode.Contextual,
                RestorationMode restorationMode = RestorationMode.Always,
                IMenuVisibilityTransition menuInTransition = null, IMenuVisibilityTransition menuOutTransition = null,
                IPanelTransition defaultPanelTransition = null,
                MenuPanelSequence menuOpenSequence = MenuPanelSequence.None,
                MenuPanelSequence menuCloseSequence = MenuPanelSequence.None,
                PaletteConfigReference defaultPaletteReference = null) {
            Toggleable = toggleable;
            StartsOpen = startsOpen;
            MenuPausesGame = menuPausesGame;
            MainPanelKey = mainPanelKey;
            DefaultPaletteConfig = paletteConfig;
            DefaultPaletteReference = defaultPaletteReference;
            PanelConfigs = panelConfigs;
            PanelGenerators = panelGenerators ?? new PanelGenerator[0];
            DefaultSelectMode = selectMode;
            SelectionRestorationMode = restorationMode;
            MenuAttributes = menuAttributesOverride.HasValue ? menuAttributesOverride.Value
                : (menuPausesGame ? MenuAttributes.StandardPauseMenu() : MenuAttributes.StandardNonPauseMenu());
            PanelChangeCallbacks = panelChangeCallbacks ?? new List<System.Action<string, string>>();
            MenuInTransition = menuInTransition;
            MenuOutTransition = menuOutTransition;
            DefaultPanelTransition = defaultPanelTransition;
            MenuOpenSequence = menuOpenSequence;
            MenuCloseSequence = menuCloseSequence;
        }

        public class Builder {
            private bool _toggleable;
            private bool _startsOpen;
            private bool _menuPausesGame;
            private SelectMode _selectMode = SelectMode.Contextual;
            private RestorationMode _selectionRestorationMode = RestorationMode.Always;
            private string _mainPanelKey = null;
            private MenuAttributes? _menuAttributesOverride = null;
            private PaletteConfig _paletteConfig;
            private PaletteConfigReference _paletteReference;
            private List<PanelConfig> _panelConfigs = new List<PanelConfig>();
            private List<PanelGenerator> _panelGenerators = new List<PanelGenerator>();
            private List<System.Action<string, string>> _panelChangeCallbacks = new List<System.Action<string, string>>();
            private IMenuVisibilityTransition _menuInTransition;
            private IMenuVisibilityTransition _menuOutTransition;
            private IPanelTransition _defaultPanelTransition;
            private MenuPanelSequence _menuOpenSequence = MenuPanelSequence.None;
            private MenuPanelSequence _menuCloseSequence = MenuPanelSequence.None;

            public Builder(bool toggleableAndStartsClosed, bool menuPausesGame) {
                _toggleable = toggleableAndStartsClosed;
                _startsOpen = !toggleableAndStartsClosed;
                _menuPausesGame = menuPausesGame;
            }

            [System.Obsolete("Set the palette via SetDefaultPaletteConfig instead of the constructor: new Builder(toggleableAndStartsClosed, menuPausesGame).SetDefaultPaletteConfig(palette).", false)]
            public Builder(bool toggleableAndStartsClosed, bool menuPausesGame, PaletteConfig paletteConfig)
                    : this(toggleableAndStartsClosed, menuPausesGame) {
                _paletteConfig = paletteConfig;
            }

            /// <summary>
            /// Sets the menu-level default palette, applied to any element that doesn't
            /// specify its own palette (config or reference). Overridden by
            /// <see cref="SetDefaultPaletteReference"/> if that is also set.
            /// </summary>
            public Builder SetDefaultPaletteConfig(PaletteConfig paletteConfig) {
                _paletteConfig = paletteConfig;
                return this;
            }

            /// <summary>
            /// Sets a menu-level default palette reference. Elements that don't specify
            /// their own palette re-theme live whenever this reference changes. Takes
            /// precedence over <see cref="SetDefaultPaletteConfig"/>.
            /// </summary>
            public Builder SetDefaultPaletteReference(PaletteConfigReference paletteReference) {
                _paletteReference = paletteReference;
                return this;
            }

            public Builder SetStartsOpen(bool startsOpen) {
                _startsOpen = startsOpen;
                return this;
            }

            public Builder SetToggleable(bool toggleable) {
                _toggleable = toggleable;
                return this;
            }

            /// <summary>
            /// Sets whether or not an item will be selected by default. If an element is
            /// not selected by default, it will require controller or keyboard up/down
            /// movement to select.
            /// Always: Always select an element by default when entering a menu or returning to it.
            /// Never: Never select an element by default.
            /// Contextual: Select element if last input type was not mouse.
            /// </summary>
            public Builder SetDefaultSelectMode(SelectMode selectMode) {
                _selectMode = selectMode;
                return this;
            }

            /// <summary>
            /// Sets the conditions under which an input will be reselected if none are selected.
            /// Always: Instantly restores a selection if no options are selected.
            /// OnInput: Restores a selection when an input that would select the next/previous input is entered.
            /// Never: Never automatically reselect an element.
            /// </summary>
            public Builder SetSelectedRestorationMode(RestorationMode mode) {
                _selectionRestorationMode = mode;
                return this;
            }

            public Builder AddPanelConfig(PanelConfig config, bool mainPanel = false) {
                _panelConfigs.Add(config);
                if (mainPanel) {
                    _mainPanelKey = config.Key;
                }
                return this;
            }

            public Builder AddPanelConfig(PanelConfig.Builder configBuilder, bool mainPanel = false) {
                PanelConfig config = configBuilder.Build();
                _panelConfigs.Add(config);
                if (mainPanel) {
                    _mainPanelKey = config.Key;
                }
                return this;
            }

            public Builder InsertPanelConfig(PanelConfig config, int index, bool mainPanel = false) {
                _panelConfigs.Insert(index, config);
                if (mainPanel) {
                    _mainPanelKey = config.Key;
                }
                return this;
            }

            public Builder InsertPanelConfig(PanelConfig.Builder configBuilder, int index, bool mainPanel = false) {
                PanelConfig config = configBuilder.Build();
                _panelConfigs.Insert(index, config);
                if (mainPanel) {
                    _mainPanelKey = config.Key;
                }
                return this;
            }

            /// <summary>
            /// Adds a callback that will be invoked whenever the active panel changes (including to and from null).
            /// The first parameter is the old panel (null if menu wasn't active), and the second is the new panel
            /// (null if menu is going away).
            /// </summary>
            public Builder AddPanelChangeCallback(System.Action<string, string> callback) {
                _panelChangeCallbacks.Add(callback);
                return this;
            }

            /// <summary>
            /// Registers a panel generator. When a key is pushed that has no static panel,
            /// generators are checked in order (innermost panel's scoped generators first,
            /// then walking down the panel stack, then the generators registered here);
            /// the first matching generator builds the panel.
            ///
            /// The generated panel is destroyed when popped from the stack or when the
            /// menu closes — its PanelConfig.OnDisposeCallback fires at that point.
            /// </summary>
            public Builder AddPanelGenerator(PanelGenerator generator) {
                _panelGenerators.Add(generator);
                return this;
            }

            public Builder AddPanelGenerator(System.Predicate<string> matches,
                    System.Func<string, PanelGeneratorContext, PanelConfig> build) {
                return AddPanelGenerator(new PanelGenerator(matches, build));
            }

            /// <summary>
            /// Sets a single animation used for both opening and closing the menu. 
            /// Leave unset for an instant show/hide.
            /// </summary>
            public Builder SetMenuTransition(IMenuVisibilityTransition transition) {
                _menuInTransition = transition;
                _menuOutTransition = transition;
                return this;
            }

            /// <summary>
            /// Sets independent open and close animations. Either may be null for
            /// an instant show/hide in that direction.
            /// </summary>
            public Builder SetMenuTransition(IMenuVisibilityTransition inTransition, IMenuVisibilityTransition outTransition) {
                _menuInTransition = inTransition;
                _menuOutTransition = outTransition;
                return this;
            }

            /// <summary>
            /// Sets how the active panel's enter/exit composes with the menu opening
            /// and closing, applying <paramref name="sequence"/> to both directions.
            /// See <see cref="MenuPanelSequence"/>.
            /// </summary>
            public Builder SetMenuSequence(MenuPanelSequence sequence) {
                _menuOpenSequence = sequence;
                _menuCloseSequence = sequence;
                return this;
            }

            /// <summary>
            /// Sets the panel enter/exit sequencing independently for opening and
            /// closing - e.g. an Ordered entrance but a None exit.
            /// </summary>
            public Builder SetMenuSequence(MenuPanelSequence openSequence, MenuPanelSequence closeSequence) {
                _menuOpenSequence = openSequence;
                _menuCloseSequence = closeSequence;
                return this;
            }

            /// <summary>
            /// Sets the fallback panel-change animation, used by any panel that doesn't
            /// set its own via PanelConfig.Builder.SetTransition. Leave unset for
            /// instant panel changes.
            /// </summary>
            public Builder SetDefaultPanelTransition(IPanelTransition transition) {
                _defaultPanelTransition = transition;
                return this;
            }

            public Builder SetMenuAttributes(MenuAttributes? menuAttributes) {
                _menuAttributesOverride = menuAttributes;
                return this;
            }

            public MenuConfig Build() {
                if (_mainPanelKey == null && _panelConfigs.Count > 0) {
                    _mainPanelKey = _panelConfigs[0].Key;
                }
                return new MenuConfig(_toggleable, _startsOpen, _menuPausesGame, _mainPanelKey,
                    _paletteConfig, _panelConfigs.ToArray(), _panelGenerators.ToArray(),
                    _panelChangeCallbacks, _menuAttributesOverride, _selectMode, _selectionRestorationMode,
                    menuInTransition: _menuInTransition, menuOutTransition: _menuOutTransition,
                    defaultPanelTransition: _defaultPanelTransition,
                    menuOpenSequence: _menuOpenSequence, menuCloseSequence: _menuCloseSequence,
                    defaultPaletteReference: _paletteReference);
            }
        }
    }
}