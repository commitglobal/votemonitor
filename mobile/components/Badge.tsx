import React from "react";
import { View, styled } from "tamagui";
import { Typography } from "./Typography";

enum Presets {
  DEFAULT = "default",
  SUCCESS = "success",
  WARNING = "warning",
  DANGER = "danger",
}

enum FormProgress {
  NOT_STARTED = "Not started",
  IN_PROGRESS = "In progress",
  COMPLETED = "Completed",
}

export interface BadgeProps {
  status: string;
}

/**
 * Badge component which supports 4 initial presets: default, succes, warning and danger
 * @param {BadgeProps} props - The props for the `Badge` component.
 * @returns {JSX.Element} The rendered `Badge` component.
 */
const Badge = (props: BadgeProps): JSX.Element => {
  const { status } = props;

  // TODO @madalinazanficu: memoize everything please
  // TODO @madalinazanficu: use strong typed values for props
  const text =
    status === "completed"
      ? FormProgress.COMPLETED
      : status === "in progress"
        ? FormProgress.IN_PROGRESS
        : FormProgress.NOT_STARTED;

  const presetType =
    status === "completed"
      ? Presets.SUCCESS
      : status === "in progress"
        ? Presets.WARNING
        : status === "danger"
          ? Presets.DANGER
          : Presets.DEFAULT;

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
    presetType === Presets.SUCCESS
      ? "$green9"
      : presetType === Presets.WARNING
        ? "$yellow7"
        : presetType === Presets.DANGER
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

export default Badge;
