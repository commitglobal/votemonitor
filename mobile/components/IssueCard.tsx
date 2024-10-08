import React from "react";
import { YStack, XStack } from "tamagui";
import { Typography } from "./Typography";
import Card from "./Card";
import { Icon } from "./Icon";
import { FormAPIModel } from "../services/definitions.api";
import i18n from "../common/config/i18n";

export const IssueCard = ({ form, onClick }: { form: FormAPIModel; onClick?: () => void }) => {
  const currentLanguage =
    i18n.language.toLocaleUpperCase() || form.name[Object.keys(form?.name)[0]];

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
            {form.name[currentLanguage]}
          </Typography>

          {/* footer */}
          <XStack flex={1}>
            <Typography preset="helper" color="$gray5" fontWeight="400" flex={1} numberOfLines={2}>
              {form.description.currentLanguage}
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
