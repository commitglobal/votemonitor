import React, { useMemo } from "react";
import { YStack, XStack } from "tamagui";
import { Typography } from "./Typography";
import Card from "./Card";
import { Icon } from "./Icon";
import { FormAPIModel } from "../services/definitions.api";
import { useTranslation } from "react-i18next";
import { SvgXml } from "react-native-svg";

export const IssueCard = ({ form, onClick }: { form: FormAPIModel; onClick?: () => void }) => {
  const { i18n } = useTranslation();

  const currentLanguage = useMemo(
    () => i18n.language.toLocaleUpperCase() || Object.keys(form?.name)[0],
    [i18n.language, form],
  );

  const formName = useMemo(() => {
    return form.name[currentLanguage] || form.name[Object.keys(form?.name)[0]];
  }, [form, currentLanguage]);

  const formDescription = useMemo(() => {
    return form.description[currentLanguage] || form.description[Object.keys(form?.description)[0]];
  }, [form, currentLanguage]);

  return (
    <Card onPress={onClick}>
      <XStack gap="$md">
        {/* icon */}
        {form.icon ? <SvgXml xml={form.icon} /> : <Icon icon="publicResourcesProblems" />}

        {/* text */}
        <YStack flex={1} gap="$xxs" justifyContent="space-between">
          <Typography preset="body2" color="$gray9">
            {formName}
          </Typography>

          {/* footer */}
          <XStack flex={1}>
            <Typography preset="helper" color="$gray5" fontWeight="400" flex={1} numberOfLines={2}>
              {formDescription}
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
