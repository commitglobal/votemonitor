import React from "react";
import { StyleProp, TextProps as RNTextProps, TextStyle } from "react-native";
import { SizableText } from "tamagui";

type Sizes = keyof typeof $sizeStyles;
type Presets = keyof typeof $presets;

export interface TextProps extends RNTextProps {
  style?: StyleProp<TextStyle>;
  /**
   * One of the different types of text presets.
   */
  preset?: Presets;
  /**
   * Text size modifier.
   */
  size?: Sizes;
  /**
   * Set number of lines to display.
   */
  numberOfLines?: number;
  /**
   * Text color modifier.
   */
  color?: string;
  /**
   * Children components.
   */
  children?: React.ReactNode;
}

/**
 * For your text displaying needs.
 * This component is a HOC over the Tamagui one
 * @param {TextProps} props - The props for the `Text` component.
 * @returns {JSX.Element} The rendered `Text` component.
 */
export function Typography(props: TextProps) {
  const {
    size,
    children,
    style: $styleOverride,
    numberOfLines = 1,
    ...rest
  } = props;

  const preset: Presets = props.preset ?? "default";
  const $styles: StyleProp<TextStyle> = [
    $presets[preset],
    size && $sizeStyles[size],
    $styleOverride,
  ];

  return (
    <SizableText
      color="$gray9"
      style={$styles}
      numberOfLines={numberOfLines}
      {...rest}
    >
      {children}
    </SizableText>
  );
}

const $sizeStyles = {
  xl: { fontSize: 24, lineHeight: 32, fontWeight: "400" } satisfies TextStyle,
  lg: { fontSize: 20, lineHeight: 26, fontWeight: "700" } satisfies TextStyle,
  md: { fontSize: 16, lineHeight: 20, fontWeight: "500" } satisfies TextStyle,
  sm: { fontSize: 14, lineHeight: 20, fontWeight: "400" } satisfies TextStyle,
  xs: { fontSize: 12, lineHeight: 14, fontWeight: "700" } satisfies TextStyle,
};

const $baseStyle: StyleProp<TextStyle> = $sizeStyles.sm;

const $presets = {
  default: $baseStyle,

  heading: { ...$baseStyle, ...$sizeStyles.xl } as StyleProp<TextStyle>,

  subheading: { ...$baseStyle, ...$sizeStyles.lg } as StyleProp<TextStyle>,

  body1: { ...$baseStyle, ...$sizeStyles.md } as StyleProp<TextStyle>,
  body2: {
    ...$baseStyle,
    ...$sizeStyles.md,
    fontWeight: "700",
  } as StyleProp<TextStyle>,

  label: $baseStyle as StyleProp<TextStyle>,

  helper: { ...$baseStyle, ...$sizeStyles.xs } as StyleProp<TextStyle>,
};
