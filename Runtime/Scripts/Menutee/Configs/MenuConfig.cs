using System.Collections.Generic;
using UnityEngine;

namespace Menutee {
	public class MenuConfig {
		public readonly bool Toggleable;
		public readonly bool StartsOpen;
		public readonly bool MenuPausesGame;
		public readonly string MainPanelKey;
		public readonly PaletteConfig PaletteConfig;
		public readonly PanelConfig[] PanelConfigs;
		public List<System.Action<string, string>> PanelChangeCallbacks;

		public readonly Color NormalColor;

		public MenuConfig(bool toggleable, bool startsOpen, bool menuPausesGame, string mainPanelKey, PaletteConfig paletteConfig, PanelConfig[] panelConfigs, List<System.Action<string, string>> panelChangeCallbacks = null) {
			Toggleable = toggleable;
			StartsOpen = startsOpen;
			MenuPausesGame = menuPausesGame;
			MainPanelKey = mainPanelKey;
			PaletteConfig = paletteConfig;
			PanelConfigs = panelConfigs;
			PanelChangeCallbacks = panelChangeCallbacks ?? new List<System.Action<string, string>>();
		}

		public class Builder {
			private bool _toggleable;
			private bool _startsOpen;
			private bool _menuPausesGame;
			private string _mainPanelKey = null;
			private PaletteConfig _paletteConfig;
			private List<PanelConfig> _panelConfigs = new List<PanelConfig>();
			private List<System.Action<string, string>> _panelChangeCallbacks = new List<System.Action<string, string>>();

			public Builder(bool toggleableAndStartsClosed, bool menuPausesGame, PaletteConfig paletteConfig) {
				_toggleable = toggleableAndStartsClosed;
				_startsOpen = !toggleableAndStartsClosed;
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

			public MenuConfig Build() {
				if (_mainPanelKey == null) {
					_mainPanelKey = _panelConfigs[0].Key;
				}
				return new MenuConfig(_toggleable, _startsOpen, _menuPausesGame, _mainPanelKey, _paletteConfig, _panelConfigs.ToArray(), _panelChangeCallbacks);
			}
		}
	}
}