import React from "react";
import { XStack, XStackProps } from "tamagui";
import { Icon } from "./Icon";
import { Typography } from "./Typography";

const AddAttachment = ({ ...rest }: XStackProps) => {
  return (
    <XStack alignItems="center" pressStyle={{ opacity: 0.5 }} {...rest}>
      <Icon icon="attachment" />
      <Typography color="$purple5" marginLeft="$xs">
        Add Note, Photo or Video
      </Typography>
    </XStack>
  );
};

export default AddAttachment;
