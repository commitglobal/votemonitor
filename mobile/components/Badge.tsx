import React from "react";
import { View, ViewStyle, TextStyle } from "react-native";
import { useTheme, Variable } from "tamagui";
import { UseThemeResult } from "@tamagui/web/src/hooks/useTheme";
import { tokens } from "../theme/tokens";
import { Typography } from "./Typography";

type Presets = "default" | "success" | "warning" | "danger";

export interface BadgeProps {
  /**
   * One of the different types of badge presets.
   */
  preset?: Presets;
}

/**
 * Badge component which supports 4 initial presets: default, succes, warning and danger
 * @param {ButtonProps} props - The props for the `Buttom` component.
 * @returns {JSX.Element} The rendered `Button` component.
 */
export function Badge(props: BadgeProps): JSX.Element {
  const theme = useTheme();

  const presetType: Presets = props.preset ?? "default";
  const style = $presets(theme, tokens.space)[presetType];
  const text = getTextByPresetType(presetType);

  return (
    <View style={style}>
      <Typography preset="body1" style={style.childTextStyle}>
        {text}
      </Typography>
    </View>
  );
}

/* 
  Extract text based on the presetType
*/
const getTextByPresetType = (presetType: Presets): string => {
  switch (presetType) {
    case "success":
      return "Completed";
    case "warning":
      return "In progress";
    case "danger":
      return "Danger";
    default:
      return "Not started";
  }
};

/*
  This type incoroporates ViewStyle for parent component
  and TextStyle for its child component.
*/
type ViewAndChildStyle = ViewStyle & {
  childTextStyle?: TextStyle;
};

const $presets = (
  theme: UseThemeResult,
  spacing: { [key: string]: Variable<number> }
) => {
  const $baseStyle: ViewAndChildStyle = {
    paddingHorizontal: spacing.xs.val,
    paddingVertical: spacing.xxxs.val,
    borderRadius: 28,
    backgroundColor: theme.$gray2?.val,
    alignItems: "center",

    childTextStyle: {
      color: theme.$gray9?.val,
      fontWeight: "500",
    },
  };

  return {
    default: $baseStyle,

    success: {
      ...$baseStyle,
      backgroundColor: theme.$green1?.val,

      childTextStyle: {
        ...$baseStyle.childTextStyle,
        color: theme.$green11?.val,
      },
    } as ViewAndChildStyle,

    danger: {
      ...$baseStyle,
      backgroundColor: theme.$red1?.val,

      childTextStyle: {
        ...$baseStyle.childTextStyle,
        color: theme.$red10?.val,
      },
    } as ViewAndChildStyle,

    warning: {
      ...$baseStyle,
      backgroundColor: theme.$yellow13?.val,

      childTextStyle: {
        ...$baseStyle.childTextStyle,
        color: theme.$yellow14?.val,
      },
    } as ViewAndChildStyle,
  };
};
