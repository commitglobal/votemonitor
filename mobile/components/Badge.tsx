import React from "react";
import { View, styled } from "tamagui";
import { Typography } from "./Typography";

export type Presets = "default" | "success" | "warning" | "danger";

export interface BadgeProps {
  /**
   * One of the different types of badge presets.
   */
  preset?: Presets;
}

/**
 * Badge component which supports 4 initial presets: default, succes, warning and danger
 * @param {BadgeProps} props - The props for the `Badge` component.
 * @returns {JSX.Element} The rendered `Badge` component.
 */
const Badge = (props: BadgeProps): JSX.Element => {
  const presetType: Presets = props.preset ?? "default";
  const text = getTextByPresetType(presetType);

  const StyledView = styled(View, {
    name: "StyledView",
    paddingHorizontal: "$xs",
    paddingVertical: 2,
    borderRadius: 28,
    backgroundColor: "$gray2",
    alignItems: "center",
    variants: {
      presets: {
        default: {},
        success: { backgroundColor: "$green2" },
        warning: { backgroundColor: "$yellow3" },
        danger: { backgroundColor: "$red1" },
      },
    } as const,
  });

  const textColor =
    presetType === "success"
      ? "$green9"
      : presetType === "warning"
      ? "$yellow7"
      : presetType === "danger"
      ? "$red10"
      : "$gray10";

  return (
    <StyledView presets={presetType}>
      <Typography
        preset="body1"
        style={{ fontSize: 16, lineHeight: 20, fontWeight: "500" }}
        color={textColor}
      >
        {text}
      </Typography>
    </StyledView>
  );
};

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

export default Badge;
