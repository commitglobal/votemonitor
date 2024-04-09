import { createTokens } from "tamagui";
import { config } from "@tamagui/config/v3";

// the comments above each key in the tokens object specify where these apply by defaul (we won't have to specify $space.sm, only $sm and it will know where to look)

export const tokens = createTokens({
  // width, height, minWidth, minHeight, maxWidth, maxHeight
  size: {
    ...config.tokens.size,
  },
  // zIndex
  zIndex: {
    ...config.tokens.zIndex,
  },
  // borderRadius, borderTopLeftRadius, borderTopRightRadius, borderBottomLeftRadius, borderBottomRightRadius
  radius: {
    ...config.tokens.radius,
  },
  // All properties not matched by the above: padding, margin, etc
  space: {
    ...config.tokens.space,
    true: 16, // mandatory default spacing
    xxxs: 4,
    xxs: 8,
    xs: 10,
    sm: 12,
    md: 16,
    lg: 24,
    xl: 32,
    xxl: 48,
    xxxl: 58,
  },
  color: {},
});
