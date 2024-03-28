import { config } from "@tamagui/config/v3";
import { createFont } from "tamagui";

const dmSansFace = {
  normal: { normal: "DMSansRegular" },
  bold: { normal: "DMSansBold" },
  unset: { normal: "DMSansRegular" },
  500: { normal: "DMSansRegular" },
  600: { normal: "DMSans" },
  700: { normal: "DMSansBold" },
};

const robotoFace = {
  normal: { normal: "RobotoRegular" },
  bold: { normal: "RobotoBold" },
  unset: { normal: "RobotoRegular" },
  500: { normal: "RobotoRegular" },
  600: { normal: "Roboto" },
  700: { normal: "RobotoBold" },
};

const Roboto = createFont({
  family: "Roboto",
  size: config.fonts.heading.size,
  lineHeight: config.fonts.heading.lineHeight,
  weight: config.fonts.heading.weight,
  letterSpacing: config.fonts.heading.letterSpacing,
  face: robotoFace,
});

const DMSans = createFont({
  family: "DMSans",
  size: config.fonts.body.size,
  lineHeight: config.fonts.body.lineHeight,
  weight: config.fonts.body.weight,
  letterSpacing: config.fonts.body.letterSpacing,
  face: dmSansFace,
});

export const fonts = { body: DMSans, heading: DMSans, tabs: Roboto };
