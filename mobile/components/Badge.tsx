import React, { useMemo } from "react";
import { View, ViewProps, styled } from "tamagui";
import { Typography } from "./Typography";
import { TextStyle } from "react-native";

export enum Status {
  NOT_STARTED = "not started",
  IN_PROGRESS = "in progress",
  COMPLETED = "completed",
  NOT_ANSWERED = "not answered",
  ANSWERED = "answered",
  MARKED_AS_COMPLETED = "markedAsCompleted",
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

  // TODO @madalinazanficu: use strong typed values for props

  const StyledView = useMemo(
    () =>
      styled(View, {
        name: "StyledView",
        paddingHorizontal: "$xs",
        paddingVertical: 2,
        borderRadius: 28,
        borderWidth: 1,
        borderColor: "$purple5",
        backgroundColor: "$purple2",
        alignItems: "center",
        justifyContent: "center",
        variants: {
          presets: {
            default: {},
            success: { backgroundColor: "$green1", borderColor: "$green6" },
            warning: { backgroundColor: "$yellow3", borderColor: "$yellow7" },
            danger: { backgroundColor: "$red1", borderColor: "$red12" },
          },
        } as const,
      }),
    [],
  );

  const presetType = useMemo(
    () =>
      status === Status.COMPLETED ||
      status === Status.ANSWERED ||
      status === Status.MARKED_AS_COMPLETED
        ? Presets.SUCCESS
        : status === Status.IN_PROGRESS
          ? Presets.WARNING
          : Presets.DEFAULT,
    [status],
  );

  const textColor = useMemo(
    () =>
      presetType === Presets.SUCCESS
        ? "$green6"
        : presetType === Presets.WARNING
          ? "$yellow7"
          : "$purple5",
    [presetType],
  );

  return (
    <StyledView presets={presetType} {...rest}>
      <Typography
        preset="body2"
        color={textColor}
        style={{ ...textStyle }}
        allowFontScaling={false}
      >
        {props.children}
      </Typography>
    </StyledView>
  );
};

export default Badge;
