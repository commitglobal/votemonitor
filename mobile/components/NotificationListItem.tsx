import React from "react";
import { Notification } from "../services/api/notifications/notifications-get.api";
import { useWindowDimensions, YStack } from "tamagui";
import { Typography } from "./Typography";
import { useTranslation } from "react-i18next";
import Card from "./Card";
import { useDateTimeFormat } from "../contexts/date-time-format/DateTimeFormatContext.provider";
import RenderHtml from "react-native-render-html";

const NotificationListItem = ({ notification }: { notification: Notification }) => {
  const { t } = useTranslation("inbox");
  const { formatDate, formatTime } = useDateTimeFormat();

  const sentAt = new Date(notification.sentAt);
  const isToday = sentAt.toDateString() === new Date().toDateString();

  const { width } = useWindowDimensions();

  return (
    <YStack gap="$sm" marginVertical="$xxs">
      <Typography textAlign="center" color="$gray6">
        {isToday
          ? `${t("today")}, ${formatTime(sentAt)}`
          : `${formatDate(sentAt)}, ${formatTime(sentAt)}`}
      </Typography>
      <Card
        borderTopStartRadius={20}
        borderTopEndRadius={20}
        borderBottomRightRadius={20}
        shadowOpacity={0}
        elevation={0}
        pressStyle={{ opacity: 1 }}
      >
        <YStack gap="$xxs">
          <Typography preset="body2">{notification.title}</Typography>

          <RenderHtml
            source={{ html: notification.body }}
            contentWidth={width - 32}
            // @ts-ignore
            tagsStyles={tagsStyles}
          />
        </YStack>
      </Card>
    </YStack>
  );
};

export default NotificationListItem;

const tagsStyles = {
  body: {
    color: "hsl(240, 5%, 34%)",
  },
  p: {
    margin: 0,
    marginBottom: 8,
  },
  h1: {
    fontSize: 24,
    marginVertical: 16,
  },
  a: {
    color: "hsl(272, 56%, 45%)",
    fontWeight: "700",
    textDecoration: "underline",
    textDecorationColor: "hsl(272, 56%, 45%)",
  },
};
