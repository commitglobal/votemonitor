import React from "react";
import { StyleProp, ViewStyle } from "react-native";
import {
  Card as TamaguiCard,
  CardProps as TamaguiCardProps,
  useTheme,
} from "tamagui";
import { tokens } from "../theme/tokens";

export interface CardProps extends TamaguiCardProps {
  style?: StyleProp<ViewStyle>;
  children?: React.ReactNode;
}

export function Card(props: CardProps): JSX.Element {
  const theme = useTheme();
  const { children, style, ...rest } = props;

  const $defaultStyling: ViewStyle = {
    padding: tokens.space.md.val,
    borderRadius: 3,
    shadowColor: theme.gray13?.val,
    shadowOffset: { width: 0, height: 1 },
    shadowRadius: 3,
    shadowOpacity: 0.07,
    elevation: 1,
  };

  return (
    <TamaguiCard
      style={[$defaultStyling, style]}
      backgroundColor="white"
      {...rest}
    >
      {children}
    </TamaguiCard>
  );
}
