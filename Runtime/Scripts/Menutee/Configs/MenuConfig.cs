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
        public readonly PaletteConfig PaletteConfig;
        public readonly PanelConfig[] PanelConfigs;
        public readonly PanelGenerator[] PanelGenerators;
        public List<System.Action<string, string>> PanelChangeCallbacks;

        public readonly Color NormalColor;

        public enum SelectMode {
            Always,
            Contextual,
            Never,
        }

        public enum RestorationMode {
            Always,
            OnInput,
            Never,
        }

        MenuConfig(bool toggleable, bool startsOpen, bool menuPausesGame, string mainPanelKey, PaletteConfig paletteConfig,
                PanelConfig[] panelConfigs, PanelGenerator[] panelGenerators = null,
                List<System.Action<string, string>> panelChangeCallbacks = null,
                MenuAttributes? menuAttributesOverride = null, SelectMode selectMode = SelectMode.Contextual,
                RestorationMode restorationMode = RestorationMode.Always) {
            Toggleable = toggleable;
            StartsOpen = startsOpen;
            MenuPausesGame = menuPausesGame;
            MainPanelKey = mainPanelKey;
            PaletteConfig = paletteConfig;
            PanelConfigs = panelConfigs;
            PanelGenerators = panelGenerators ?? new PanelGenerator[0];
            DefaultSelectMode = selectMode;
            SelectionRestorationMode = restorationMode;
            MenuAttributes = menuAttributesOverride.HasValue ? menuAttributesOverride.Value
                : (menuPausesGame ? MenuAttributes.StandardPauseMenu() : MenuAttributes.StandardNonPauseMenu());
            PanelChangeCallbacks = panelChangeCallbacks ?? new List<System.Action<string, string>>();
        }

        public class Builder {
            private bool _toggleable;
            private bool _startsOpen;
            private bool _menuPausesGame;
            private SelectMode _selectMode;
            private RestorationMode _selectionRestorationMode;
            private string _mainPanelKey = null;
            private MenuAttributes? _menuAttributesOverride = null;
            private PaletteConfig _paletteConfig;
            private List<PanelConfig> _panelConfigs = new List<PanelConfig>();
            private List<PanelGenerator> _panelGenerators = new List<PanelGenerator>();
            private List<System.Action<string, string>> _panelChangeCallbacks = new List<System.Action<string, string>>();

            public Builder(bool toggleableAndStartsClosed, bool menuPausesGame, PaletteConfig paletteConfig) {
                _toggleable = toggleableAndStartsClosed;
                _startsOpen = !toggleableAndStartsClosed;
                _selectMode = SelectMode.Contextual;
                _menuPausesGame = menuPausesGame;
                _paletteConfig = paletteConfig;
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
            /// Sets the conditions under which an input will be reselcted if none are selected.
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
                    _panelChangeCallbacks, _menuAttributesOverride, _selectMode);
            }
        }
    }
}