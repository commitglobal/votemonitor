import React from "react";
import { View, styled } from "tamagui";
import { Typography } from "./Typography";

export enum FormStatus {
  NOT_STARTED = "not started",
  IN_PROGRESS = "in progress",
  COMPLETED = "completed",
  DANGER = "danger",
}

enum Presets {
  DEFAULT = "default",
  SUCCESS = "success",
  WARNING = "warning",
  DANGER = "danger",
}

export enum FormProgress {
  NOT_STARTED = "Not started",
  IN_PROGRESS = "In progress",
  COMPLETED = "Completed",
  DANGER = "Danger",
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

  const StyledView = styled(View, {
    name: "StyledView",
    paddingHorizontal: "$xs",
    paddingVertical: 2,
    borderRadius: 28,
    backgroundColor: "$gray2",
    alignItems: "center",
    justifyContent: "center",
    variants: {
      presets: {
        default: {},
        success: { backgroundColor: "$green2" },
        warning: { backgroundColor: "$yellow3" },
        danger: { backgroundColor: "$red1" },
      },
    } as const,
  });

  const presetType =
    status === FormStatus.COMPLETED
      ? Presets.SUCCESS
      : status === FormStatus.IN_PROGRESS
        ? Presets.WARNING
        : status === FormStatus.DANGER
          ? Presets.DANGER
          : Presets.DEFAULT;

  const textColor =
    presetType === Presets.SUCCESS
      ? "$green9"
      : presetType === Presets.WARNING
        ? "$yellow7"
        : presetType === Presets.DANGER
          ? "$red10"
          : "$gray10";

  const text =
    status === FormStatus.COMPLETED
      ? FormProgress.COMPLETED
      : status === FormStatus.IN_PROGRESS
        ? FormProgress.IN_PROGRESS
        : status === FormStatus.DANGER
          ? FormProgress.DANGER
          : FormProgress.NOT_STARTED;

  return (
    <StyledView presets={presetType}>
      <Typography preset="body2" color={textColor}>
        {text}
      </Typography>
    </StyledView>
  );
};

export default Badge;
