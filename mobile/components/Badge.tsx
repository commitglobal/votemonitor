import React from "react";
import { View, styled } from "tamagui";
import { Typography } from "./Typography";

export type Presets = "default" | "success" | "warning" | "danger";

// enum FormProgress {
//   NOT_STARTED = "Not started",
//   IN_PROGRESS = "In progress",
//   COMPLETED = "Completed",
// }

export interface BadgeProps {
  children: string;
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
  const { children } = props;
  const presetType: Presets = props.preset ?? "default";

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
        {children}
      </Typography>
    </StyledView>
  );
};

export default Badge;
