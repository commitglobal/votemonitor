import React from "react";
import { XStack, XStackProps } from "tamagui";
import { Icon } from "./Icon";
import { Typography } from "./Typography";

interface CardFooterProps extends XStackProps {
  text: string;
}

const CardFooter: React.FC<CardFooterProps> = ({ text, ...rest }) => {
  return (
    <XStack alignItems="center" justifyContent="space-between" width="100%" {...rest}>
      <Typography color="$gray5" fontWeight="500" width="80%">
        {text}
      </Typography>
      <Icon icon="chevronRight" color="$purple5" />
    </XStack>
  );
};

export default CardFooter;
