import * as Crypto from "expo-crypto";
import { router, useLocalSearchParams } from "expo-router";
import { useMemo, useState } from "react";
import { useTranslation } from "react-i18next";
import { RefreshControl } from "react-native";
import { ScrollView, Spinner, useWindowDimensions, YStack } from "tamagui";
import Button from "../../../../../../../components/Button";
import Card from "../../../../../../../components/Card";
import FormSubmissionListItem, {
  FormSubmissionDetails,
} from "../../../../../../../components/FormSubmissionListItem";
import Header from "../../../../../../../components/Header";
import { Icon } from "../../../../../../../components/Icon";
import { ListView } from "../../../../../../../components/ListView";
import { Screen } from "../../../../../../../components/Screen";
import { Typography } from "../../../../../../../components/Typography";
import { useNetInfoContext } from "../../../../../../../contexts/net-info-banner/NetInfoContext";
import { useUserData } from "../../../../../../../contexts/user/UserContext.provider";
import { mapFormToFormSubmissionListItem } from "../../../../../../../services/form.parser";
import { useFormSubmissionsByFormId } from "../../../../../../../services/queries/form-submissions.query";
import { useFormById } from "../../../../../../../services/queries/forms.query";

const ESTIMATED_ITEM_SIZE = 100;

type SearchParamsType = {
  formId: string;
  language: string;
};

const MultiSubmissionFormDetails = () => {
  const { t } = useTranslation(["form_overview", "common"]);
  const { formId, language } = useLocalSearchParams<SearchParamsType>();
  const { isOnline } = useNetInfoContext();

  if (!formId || !language) {
    return <Typography>MultiSubmissionFormDetails Incorrect page params</Typography>;
  }

  const { activeElectionRound, selectedPollingStation } = useUserData();
  const [isRefreshing, setIsRefreshing] = useState(false);
  const { width } = useWindowDimensions();

  const {
    data: currentForm,
    isLoading: isLoadingCurrentForm,
    error: currentFormError,
    refetch: refetchCurrentForm,
  } = useFormById(activeElectionRound?.id, formId);

  const {
    data: formSubmissions,
    isLoading: isLoadingSubmissions,
    error: submissionsError,
    refetch: refetchSubmissions,
  } = useFormSubmissionsByFormId(
    activeElectionRound?.id,
    selectedPollingStation?.pollingStationId,
    formId,
  );

  const handleRefetch = () => {
    setIsRefreshing(true);
    Promise.all([refetchCurrentForm(), refetchSubmissions()]).then(() => {
      setIsRefreshing(false);
    });
  };

  const formsSubmissionListItems = useMemo(() => {
    if (currentForm) {
      return mapFormToFormSubmissionListItem(currentForm, formSubmissions);
    }

    return [];
  }, [currentForm, formSubmissions]);

  const onFormSubmissionClick = (submissionId: string) => {
    router.push(`/form-submission-details/${submissionId}?formId=${formId}&language=${language}`);
  };

  const { formTitle } = useMemo(() => {
    return {
      formTitle: `${currentForm?.code} - ${currentForm?.name[language]} (${language})`,
      languages: currentForm?.languages,
    };
  }, [currentForm]);

  const onAddFormSubmission = () => {
    // find first unanswered question
    // do not navigate if the form has no questions or not found
    if (!currentForm || currentForm.questions.length === 0) return;

    const newSubmissionId = Crypto.randomUUID();
    return router.push(
      `/form-submission-details/${newSubmissionId}?formId=${formId}&language=${language}`,
    );
  };

  const navigateToReviewBeforeDeleteSubmission = (
    submissionId: string,
    submissionNumber: number,
  ) => {
    return router.push(
      `/delete-submission/${submissionId}?formId=${formId}&language=${language}&submissionNumber=${submissionNumber}`,
    );
  };

  if (isLoadingCurrentForm || isLoadingSubmissions) {
    return (
      <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
        <Header
          title={`${formId}`}
          titleColor="white"
          barStyle="light-content"
          leftIcon={<Icon icon="chevronLeft" color="white" />}
          onLeftPress={() => router.back()}
        />
        <YStack justifyContent="center" alignItems="center" flex={1}>
          <Spinner size="large" color="$purple5" />
        </YStack>
      </Screen>
    );
  }

  if (currentFormError || submissionsError) {
    return (
      <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
        <Header
          title={`${formId}`}
          titleColor="white"
          barStyle="light-content"
          leftIcon={<Icon icon="chevronLeft" color="white" />}
          onLeftPress={() => router.back()}
        />
        <ScrollView
          contentContainerStyle={{ flex: 1, alignItems: "center" }}
          paddingVertical="$xxl"
          showsVerticalScrollIndicator={false}
          bounces={isOnline}
          refreshControl={<RefreshControl refreshing={isRefreshing} onRefresh={handleRefetch} />}
        >
          <Typography>{t("error")}</Typography>
        </ScrollView>
      </Screen>
    );
  }

  return (
    <Screen preset="fixed" contentContainerStyle={{ flexGrow: 1 }}>
      <Header
        title={`${formTitle}`}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />
      <YStack paddingTop={28} gap="$xl" paddingHorizontal="$md" flex={1}>
        <ListView<FormSubmissionDetails>
          data={formsSubmissionListItems}
          ListHeaderComponent={
            <YStack gap="$xl" paddingBottom="$xxs">
              <Card padding="$md">
                <Button preset="outlined" marginTop="$md" onPress={onAddFormSubmission}>
                  {t("overview.add_submission")}
                </Button>
              </Card>

              <Typography preset="body1" fontWeight="700" gap="$xxs">
                {t("overview.submissions_list")}
              </Typography>
            </YStack>
          }
          showsVerticalScrollIndicator={false}
          renderItem={({ item, index }) => {
            return (
              <FormSubmissionListItem
                key={index}
                {...item}
                onClick={() => onFormSubmissionClick(item.id)}
                onDeleteSubmission={() =>
                  navigateToReviewBeforeDeleteSubmission(item.id, item.submissionNumber)
                }
              />
            );
          }}
          estimatedItemSize={ESTIMATED_ITEM_SIZE}
          estimatedListSize={{ height: ESTIMATED_ITEM_SIZE * 5, width: width - 32 }}
          refreshControl={<RefreshControl refreshing={isRefreshing} onRefresh={handleRefetch} />}
        />
      </YStack>
    </Screen>
  );
};

export default MultiSubmissionFormDetails;
