import React from "react";
import { XStack } from "tamagui";
import { Typography } from "./Typography";

export const NotificationsBadge = ({
  noOfUnreadNotifications,
}: {
  noOfUnreadNotifications: number;
}) => {
  return (
    <XStack
      position="absolute"
      top={-5}
      right={-5}
      width={15}
      height={15}
      backgroundColor="red"
      borderRadius={50}
      justifyContent="center"
      alignItems="center"
    >
      <Typography
        fontSize={noOfUnreadNotifications && noOfUnreadNotifications >= 100 ? 8 : 10}
        allowFontScaling={false}
        color="white"
        textAlign="center"
        fontWeight="700"
        lineHeight={14}
        maxWidth={15}
        numberOfLines={1}
        ellipsizeMode="tail"
      >
        {noOfUnreadNotifications || "0"}
      </Typography>
    </XStack>
  );
};
