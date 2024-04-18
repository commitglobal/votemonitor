import { router, useLocalSearchParams } from "expo-router";
import { Screen } from "../../../components/Screen";
import Header from "../../../components/Header";
import { Icon } from "../../../components/Icon";
import { useElectionRoundAllForms, useFormSubmissions } from "../../../services/queries.service";
import { Typography } from "../../../components/Typography";
import { XStack, YStack } from "tamagui";
import LinearProgress from "../../../components/LinearProgress";
import { useMemo } from "react";
import { useUserData } from "../../../contexts/user/UserContext.provider";
import { ApiFormQuestion } from "../../../services/interfaces/question.type";

const FormQuestionnaire = () => {
  const { questionId, formId, language } = useLocalSearchParams();
  console.log("language", language);
  console.log("formId", formId);
  console.log("questionId", questionId);

  const { activeElectionRound, selectedPollingStation } = useUserData();

  const {
    data: allForms,
    isLoading: isLoadingForms,
    error: formsError,
  } = useElectionRoundAllForms(activeElectionRound?.id);

  const {
    data: formSubmissions,
    isLoading: isLoadingAnswers,
    error: answersError,
  } = useFormSubmissions(activeElectionRound?.id, selectedPollingStation?.pollingStationId);
  console.log("formSubmissions", formSubmissions);

  const currentForm = useMemo(() => {
    const form = allForms?.forms.find((form) => form.id === formId);
    return form;
  }, [allForms]);

  const activeQuestion: { question: ApiFormQuestion; index: number } | null = useMemo(() => {
    const qIndex = currentForm?.questions.findIndex((qt) => qt.id === questionId);
    return qIndex !== undefined && qIndex >= 0
      ? { question: currentForm?.questions[qIndex] as ApiFormQuestion, index: qIndex + 1 }
      : null;
  }, [questionId]);

  const formTitle = useMemo(() => {
    return currentForm?.name[(language as string) || currentForm.defaultLanguage];
  }, [currentForm]);

  if (isLoadingForms || isLoadingAnswers) {
    return <Typography>Loading</Typography>;
  }

  if (formsError || answersError) {
    return <Typography>Form Error</Typography>;
  }

  return (
    <Screen
      preset="scroll"
      ScrollViewProps={{
        stickyHeaderIndices: [0],
        bounces: false,
        showsVerticalScrollIndicator: false,
      }}
    >
      <Header
        title={`${formTitle}`}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />
      <YStack padding="$md">
        <YStack gap="$xxs">
          <XStack justifyContent="space-between">
            <Typography>Some text here</Typography>
            <Typography justifyContent="space-between">{`${activeQuestion?.index}/${currentForm?.questions.length}`}</Typography>
          </XStack>
          <LinearProgress
            current={activeQuestion?.index || 0}
            total={currentForm?.questions.length || 0}
          />
          <XStack justifyContent="flex-end">
            <Typography color="$red10" onPress={() => console.log("press me to clear")}>
              Clear answer
            </Typography>
          </XStack>
        </YStack>
      </YStack>
    </Screen>
  );
};

export default FormQuestionnaire;
