import { config } from "@tamagui/config/v3";
import { createFont } from "tamagui";

const dmSansFace = {
  normal: { normal: "DMSans-Regular" },
  bold: { normal: "DMSans-Bold" },
  unset: { normal: "DMSans-Regular" },
  500: { normal: "DMSans-Regular" },
  600: { normal: "DMSans-Medium" },
  700: { normal: "DMSans-Bold" },
};

const robotoFace = {
  normal: { normal: "Roboto-Regular" },
  bold: { normal: "Roboto-Bold" },
  unset: { normal: "Roboto-Regular" },
  500: { normal: "Roboto-Regular" },
  600: { normal: "Roboto-Medium" },
  700: { normal: "Roboto-Bold" },
};

const headingFont = createFont({
  family: "Roboto",
  size: config.fonts.heading.size,
  lineHeight: config.fonts.heading.lineHeight,
  weight: config.fonts.heading.weight,
  letterSpacing: config.fonts.heading.letterSpacing,
  face: robotoFace,
});

const bodyFont = createFont({
  family: "DMSans",
  size: config.fonts.body.size,
  lineHeight: config.fonts.body.lineHeight,
  weight: config.fonts.body.weight,
  letterSpacing: config.fonts.body.letterSpacing,
  face: dmSansFace,
});

export const fonts = { heading: headingFont, body: bodyFont };
