import React from "react";
import { XStack, XStackProps } from "tamagui";
import { Icon } from "./Icon";
import { Typography } from "./Typography";

interface AddAttachmentProps extends XStackProps {
  label: string;
}

const AddAttachment = (props: AddAttachmentProps) => {
  const { label, ...rest } = props;
  return (
    <XStack
      pressStyle={{ opacity: 0.5 }}
      {...rest}
      alignSelf="flex-start"
      alignItems="center"
      paddingRight="$lg"
      paddingVertical="$sm"
    >
      <Icon icon="attachment" />
      <Typography color="$purple5" marginLeft="$xs">
        {label}
      </Typography>
    </XStack>
  );
};

export default AddAttachment;
