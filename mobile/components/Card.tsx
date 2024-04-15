import React from "react";
import { StyleProp, ViewStyle } from "react-native";
import { styled, Card as TamaguiCard, CardProps as TamaguiCardProps } from "tamagui";

export interface CardProps extends TamaguiCardProps {
  style?: StyleProp<ViewStyle>;
  children?: React.ReactNode;
}

const StyledCard = styled(TamaguiCard, {
  padding: "$md",
  borderRadius: 3,
  backgroundColor: "white",
  shadowColor: "$gray13",
  shadowOffset: { width: 0, height: 1 },
  shadowRadius: 3,
  shadowOpacity: 0.07,
  elevation: 1,
  pressStyle: {
    opacity: 0.5,
  },
});

const Card = (props: CardProps): JSX.Element => {
  const { children, style: $styleOverride, ...rest } = props;

  return (
    <StyledCard style={$styleOverride} {...rest}>
      {children}
    </StyledCard>
  );
};

export default Card;
