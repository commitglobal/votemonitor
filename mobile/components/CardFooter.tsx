import React from "react";
import { XStack, Text } from "tamagui";
import { Icon } from "./Icon";
import { Typography } from "./Typography";

const CardFooter = ({
  text,
  action,
  ...rest
}: {
  text: string;
  action: () => void;
}) => {
  return (
    <XStack
      alignItems="center"
      justifyContent="space-between"
      width="100%"
      onPress={action}
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
