import React, { useState } from "react";
import { XStack, XStackProps } from "tamagui";
import { Icon } from "./Icon";
import { Typography } from "./Typography";

interface CardFooterProps extends XStackProps {
  text: string;
  action: () => void;
}

const CardFooter: React.FC<CardFooterProps> = ({ text, action, ...rest }) => {
  // const [isPressed, setIsPressed] = useState(false);

  return (
    <XStack
      alignItems="center"
      justifyContent="space-between"
      width="100%"
      onPress={action}
      //TODO: do we handle pressed state?
      // onPressIn={() => setIsPressed(true)}
      // onPressOut={() => setIsPressed(false)}
      // opacity={isPressed ? 0.5 : 1}
      {...rest}
    >
      <Typography
        color="$gray5"
        size="sm"
        style={{ fontWeight: "500", width: "80%" }}
      >
        {text}
      </Typography>
      <Icon icon="chevronRight" color="$purple5" />
    </XStack>
  );
};

export default CardFooter;
