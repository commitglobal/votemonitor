import React from "react";
import { YStack, XStack } from "tamagui";
import { Typography } from "./Typography";
import Card from "./Card";
import { ICitizenReportingForm } from "../services/api/citizen/get-citizen-reporting-forms";
import { Icon } from "./Icon";

export const IssueCard = ({
  form,
  onClick,
}: {
  form: ICitizenReportingForm;
  onClick?: () => void;
}) => {
  return (
    <Card onPress={onClick}>
      <XStack gap="$md">
        {/* icon */}
        <XStack
          justifyContent="center"
          alignItems="center"
          borderWidth={1}
          borderColor="$purple5"
          padding="$md"
          borderRadius={12}
          alignSelf="flex-start"
        >
          <Icon icon="publicResourcesProblems" size={40} />
        </XStack>

        {/* text */}
        <YStack flex={1} gap="$xxs" justifyContent="space-between">
          <Typography preset="body2" color="$gray9">
            {form.name}
          </Typography>

          {/* footer */}
          <XStack flex={1}>
            <Typography preset="helper" color="$gray5" fontWeight="400" flex={1} numberOfLines={2}>
              {form.description}
            </Typography>

            <Icon
              icon="chevronRight"
              size={24}
              color="$purple5"
              alignSelf="flex-end"
              paddingLeft="$xxs"
            />
          </XStack>
        </YStack>
      </XStack>
    </Card>
  );
};
