import React from "react";
import { View, styled } from "tamagui";
import { Typography } from "./Typography";

export enum Status {
  NOT_STARTED = "not started",
  IN_PROGRESS = "in progress",
  COMPLETED = "completed",
  NOT_ANSWERED = "not answered",
  ANSWERED = "answered",
}

enum Presets {
  DEFAULT = "default",
  SUCCESS = "success",
  WARNING = "warning",
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
    status === Status.COMPLETED || status === Status.ANSWERED
      ? Presets.SUCCESS
      : status === Status.IN_PROGRESS
        ? Presets.WARNING
        : Presets.DEFAULT;

  const textColor =
    presetType === Presets.SUCCESS
      ? "$green9"
      : presetType === Presets.WARNING
        ? "$yellow7"
        : "$gray10";

  const text =
    status === Status.COMPLETED
      ? Status.COMPLETED.charAt(0).toUpperCase() + Status.COMPLETED.slice(1)
      : status === Status.ANSWERED
        ? Status.ANSWERED.charAt(0).toUpperCase() + Status.ANSWERED.slice(1)
        : status === Status.NOT_ANSWERED
          ? Status.NOT_ANSWERED.split(" ")
              .map((word) => word.charAt(0).toUpperCase() + word.slice(1))
              .join(" ")
          : status === Status.IN_PROGRESS
            ? Status.IN_PROGRESS.charAt(0).toUpperCase() + Status.IN_PROGRESS.slice(1)
            : Status.NOT_STARTED.charAt(0).toUpperCase() + Status.NOT_STARTED.slice(1);

  return (
    <StyledView presets={presetType}>
      <Typography preset="body2" color={textColor}>
        {text}
      </Typography>
    </StyledView>
  );
};

export default Badge;
