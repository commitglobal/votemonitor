import React from "react";
import { View, ViewProps, styled } from "tamagui";
import { Typography } from "./Typography";
import { TextStyle } from "react-native";

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

export interface BadgeProps extends ViewProps {
  status: string;
  children: string;
  textStyle?: TextStyle;
}

/**
 * Badge component which supports 4 initial presets: default, succes, warning and danger
 * @param {BadgeProps} props - The props for the `Badge` component.
 * @returns {JSX.Element} The rendered `Badge` component.
 */
const Badge = (props: BadgeProps): JSX.Element => {
  const { status, textStyle, ...rest } = props;

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

  return (
    <StyledView presets={presetType} {...rest}>
      <Typography preset="body2" color={textColor} style={{ ...textStyle }}>
        {props.children}
      </Typography>
    </StyledView>
  );
};

export default Badge;
