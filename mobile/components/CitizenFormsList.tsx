import React from "react";
import { Spinner, XStack, YStack } from "tamagui";
import { ListView } from "./ListView";
import { FormAPIModel } from "../services/definitions.api";
import { IssueCard } from "./IssueCard";
import { RefreshControl } from "react-native";
import { Typography } from "./Typography";
import { useTranslation } from "react-i18next";
import { Icon } from "./Icon";
import { LoadingContent } from "./ListContent";
import Button from "./Button";

interface CitizenFormsListProps {
  forms: FormAPIModel[];
  isLoading: boolean;
  isError: boolean;
  isRefetching: boolean;
  refetch: () => void;
  onFormPress: (form: FormAPIModel) => void;
}

const ESTIMATED_ITEM_SIZE = 115;

export const CitizenFormsList = ({
  forms,
  isLoading,
  isError,
  isRefetching,
  refetch,
  onFormPress,
}: CitizenFormsListProps) => {
  const { t } = useTranslation("citizen_report_issue");

  if (isLoading) {
    return <LoadingContent />;
  }

  if (isError) {
    return <ErrorContent onRefetch={refetch} isRefetching={isRefetching} />;
  }

  return (
    <YStack flex={1}>
      <ListView<FormAPIModel>
        data={forms}
        contentContainerStyle={{
          paddingHorizontal: 16,
          paddingBottom: 24,
        }}
        showsVerticalScrollIndicator={false}
        ListHeaderComponent={
          <Typography marginTop="$lg" marginBottom="$md">
            {t("description")}
          </Typography>
        }
        estimatedItemSize={ESTIMATED_ITEM_SIZE}
        ListEmptyComponent={CitizenFormsListEmptyContent}
        renderItem={({ item }) => (
          <IssueCard key={item.id} form={item} onClick={() => onFormPress(item)} />
        )}
        ItemSeparatorComponent={() => <YStack height={16} width="100%" />}
        refreshControl={<RefreshControl refreshing={isRefetching} onRefresh={refetch} />}
      />
    </YStack>
  );
};

const CitizenFormsListEmptyContent = () => {
  const { t } = useTranslation("citizen_report_issue");

  return (
    <XStack gap="$md" alignItems="center">
      <Icon icon="form" />
      <Typography color="$gray5" flex={1}>
        {t("empty_forms")}
      </Typography>
    </XStack>
  );
};

const ErrorContent = ({
  onRefetch,
  isRefetching,
}: {
  onRefetch: () => void;
  isRefetching: boolean;
}) => {
  const { t } = useTranslation("citizen_report_issue");

  return (
    <YStack flex={1} paddingVertical="$lg" paddingHorizontal="$lg" gap="$lg">
      <XStack gap="$md">
        {isRefetching ? (
          <Spinner color="$purple5" size="large" />
        ) : (
          <Icon icon="warning" color="$purple5" size={36} />
        )}

        <Typography flex={1}>{t("error_message")}</Typography>
      </XStack>

      <Button onPress={onRefetch} disabled={isRefetching} marginTop="$lg">
        {t("retry")}
      </Button>
    </YStack>
  );
};
