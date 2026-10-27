import * as blades from "./pages";
import * as locales from "./locales";
import { defineAppModule } from "@vc-shell/framework";

// legacy: workspace.EnvironmentsCompareState (list → environment settings / comparison)
export default defineAppModule({
  blades,
  locales,
});

export * from "./pages";
