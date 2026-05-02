using System;

namespace Menutee {
	public class PanelGenerator {
		public readonly Predicate<string> Matches;
		public readonly Func<string, PanelGeneratorContext, PanelConfig> Build;

		public PanelGenerator(Predicate<string> matches,
				Func<string, PanelGeneratorContext, PanelConfig> build) {
			Matches = matches;
			Build = build;
		}
	}
}
