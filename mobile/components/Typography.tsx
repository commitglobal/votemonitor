import React from "react";
import { StyleProp, TextStyle } from "react-native";
import { Text, styled, TextProps as TamaguiTextProps } from "tamagui";

type PresetType = "default" | "heading" | "subheading" | "body1" | "body2" | "helper";

export interface TextProps extends TamaguiTextProps {
  /**
   * Style overrides
   */
  style?: StyleProp<TextStyle>;
  /**
   * One of the different types of text presets.
   */
  preset?: PresetType;
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
  const { children, style: $styleOverride, ...rest } = props;
  const presetType: PresetType = props.preset ?? "default";

  const StyledText = styled(Text, {
    name: "StyledText",
    color: "$gray9",
    fontWeight: "400",
    lineHeight: 20,
    fontSize: 14,
    maxFontSizeMultiplier: 1.2,
    variants: {
      presets: {
        default: {},
        heading: { fontSize: 24, lineHeight: 32, maxFontSizeMultiplier: 1 },
        subheading: { fontSize: 20, lineHeight: 26, fontWeight: "700", maxFontSizeMultiplier: 1 },
        body1: { fontSize: 16 },
        body2: { fontSize: 16, fontWeight: "500" },
        helper: { fontSize: 12, lineHeight: 14, fontWeight: "700" },
      },
    } as const,
  });

  return (
    <StyledText presets={presetType} style={$styleOverride} {...rest}>
      {children}
    </StyledText>
  );
}
