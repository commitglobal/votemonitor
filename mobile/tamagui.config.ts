import { config } from "@tamagui/config/v3";
import { themes } from "./theme/themes";
import { createTamagui } from "tamagui";
import { fonts } from "./theme/fonts";
import { tokens } from "./theme/tokens";

export const tamaguiConfig = createTamagui({
  ...config,
  themes,
  fonts,
  tokens,
});

export default tamaguiConfig;

export type Conf = typeof tamaguiConfig;

declare module "tamagui" {
  interface TamaguiCustomConfig extends Conf {}
}
