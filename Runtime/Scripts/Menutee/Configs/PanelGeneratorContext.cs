using System;
using System.Collections.Generic;

namespace Menutee {
	public class PanelGeneratorContext {
		private readonly List<PanelGenerator> _generators = new List<PanelGenerator>();

		public void AddPanelGenerator(PanelGenerator generator) {
			_generators.Add(generator);
		}

		public void AddPanelGenerator(Predicate<string> matches,
				Func<string, PanelGeneratorContext, PanelConfig> build) {
			_generators.Add(new PanelGenerator(matches, build));
		}

		public IReadOnlyList<PanelGenerator> ScopedGenerators {
			get { return _generators; }
		}
	}
}
