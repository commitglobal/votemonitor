import { config } from "@tamagui/config/v3";

import { createTamagui } from "tamagui";
import { themes } from "./theme/themes";
import { fonts } from "./theme/fonts";

export const tamaguiConfig = createTamagui({
  ...config,
  themes,
  fonts,
});

export default tamaguiConfig;

export type Conf = typeof tamaguiConfig;

declare module "tamagui" {
  interface TamaguiCustomConfig extends Conf {}
}
