import React from "react";
import { Notification } from "../services/api/get-notifications.api";
import { YStack } from "tamagui";
import { Typography } from "./Typography";
import { useTranslation } from "react-i18next";
import Card from "./Card";

const NotificationListItem = ({ notification }: { notification: Notification }) => {
  const { t, i18n } = useTranslation("inbox");

  const sentAt = new Date(notification.sentAt);
  const isToday = sentAt.toDateString() === new Date().toDateString();

  return (
    <YStack gap="$sm" marginVertical="$xxs">
      <Typography textAlign="center" color="$gray6">
        {isToday
          ? `${t("today")}, ` +
            sentAt.toLocaleTimeString([], {
              hour: "2-digit",
              minute: "2-digit",
            })
          : sentAt.toLocaleString(i18n.language, {
              month: "short",
              day: "2-digit",
              year: "numeric",
              hour: "2-digit",
              minute: "2-digit",
            })}
      </Typography>
      <Card>
        <YStack gap="$xxs">
          <Typography preset="body2">{notification.title}</Typography>
          <Typography lineHeight={24}>{notification.body}</Typography>
        </YStack>
      </Card>
    </YStack>
  );
};

export default NotificationListItem;
