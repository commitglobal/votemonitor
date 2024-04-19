import { router, useLocalSearchParams } from "expo-router";
import { Screen } from "../../../../../../components/Screen";
import Header from "../../../../../../components/Header";
import { Icon } from "../../../../../../components/Icon";
import {
  useElectionRoundAllForms,
  useFormSubmissions,
} from "../../../../../../services/queries.service";
import { useUserData } from "../../../../../../contexts/user/UserContext.provider";
import { Typography } from "../../../../../../components/Typography";
import { Card, Sheet, XStack, YStack } from "tamagui";
import CircularProgress from "../../../../../../components/CircularProgress";
import Button from "../../../../../../components/Button";
import { useMemo, useState } from "react";
import { ListView } from "../../../../../../components/ListView";
import CardFooter from "../../../../../../components/CardFooter";
import Badge from "../../../../../../components/Badge";
import {
  FormStatus,
  mapAPIAnswersToFormAnswers,
  mapFormStateStatus,
} from "../../../../../../services/form.parser";
import { ApiFormAnswer } from "../../../../../../services/interfaces/answer.type";
import { Dimensions } from "react-native";
import { useTranslation } from "react-i18next";
import { FormStateToTextMapper } from "../../../../../../components/FormCard";
import { useSafeAreaInsets } from "react-native-safe-area-context";

interface FormOverviewProps {
  completedAnswers: number;
  numberOfQuestions: number;
  onFormActionClick: () => void;
}

const FormOverview = ({
  completedAnswers,
  numberOfQuestions,
  onFormActionClick,
}: FormOverviewProps) => {
  const formStatus = useMemo(
    () => mapFormStateStatus(completedAnswers, numberOfQuestions),
    [completedAnswers, numberOfQuestions],
  );
  const { t } = useTranslation("form_overview");

  return (
    <Card padding="$md">
      {/* //TODO: translations */}
      <Typography preset="body1" fontWeight="700">
        {t("form_overview.title")}
      </Typography>
      <XStack alignItems="center" justifyContent="space-between">
        <YStack gap="$sm">
          <Typography fontWeight="500" color="$gray5">
            {t("form_overview.status")}:{" "}
            <Typography fontWeight="700">{FormStateToTextMapper[formStatus]}</Typography>
          </Typography>
          <Typography fontWeight="500" color="$gray5">
            Answered questions:{" "}
            <Typography fontWeight="700">
              {completedAnswers}/{numberOfQuestions}
            </Typography>
          </Typography>
        </YStack>
        {/* TODO: This doesn't look good */}
        <CircularProgress progress={(completedAnswers / numberOfQuestions) * 100} size={98} />
      </XStack>
      <Button
        preset="outlined"
        marginTop="$md"
        disabled={completedAnswers === numberOfQuestions}
        onPress={onFormActionClick}
      >
        {formStatus === FormStatus.NOT_STARTED ? "Start form" : "Resume form"}
      </Button>
    </Card>
  );
};

enum QuestionStatus {
  ANSWERED = "answered",
  NOT_ANSWERED = "not answered",
}

const QuestionStatusToTextWrapper = {
  [QuestionStatus.ANSWERED]: "Answered",
  [QuestionStatus.NOT_ANSWERED]: "Not Answered",
};
interface FormQuestionListItemProps {
  index: number;
  numberOfQuestions: number;
  status: QuestionStatus;
  question: string;
  onClick: () => void;
}

const FormQuestionListItem = ({
  index,
  numberOfQuestions,
  status,
  question,
  onClick,
}: FormQuestionListItemProps) => (
  <Card gap="$md" padding="$md" marginBottom="$xxs" onPress={onClick}>
    <YStack gap="$xxs">
      <XStack justifyContent="space-between">
        <Typography color="$gray5">{`${index}/${numberOfQuestions}`}</Typography>
        <Badge status={status}>{QuestionStatusToTextWrapper[status]}</Badge>
      </XStack>
      <Typography preset="body2">{question}</Typography>
    </YStack>
    <CardFooter text="No attached notes"></CardFooter>
  </Card>
);

const FormDetails = () => {
  const { formId, language } = useLocalSearchParams();
  const { activeElectionRound, selectedPollingStation } = useUserData();
  const [optionSheetOpen, setOptionSheetOpen] = useState(false);
  const insets = useSafeAreaInsets();

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

  const answers: Record<string, ApiFormAnswer> = useMemo(() => {
    const formSubmission = formSubmissions?.submissions.find(
      (sub) => sub.formId === (formId as string),
    );
    return mapAPIAnswersToFormAnswers(formSubmission?.answers);
  }, [formSubmissions]);

  const { questions, numberOfAnswers } = useMemo(() => {
    const form = allForms?.forms.find((form) => form.id === formId);
    const submission = formSubmissions?.submissions.find(
      (submission) => submission.formId === formId,
    );
    return {
      questions: form?.questions.map((q) => ({
        status: answers[q.id] ? QuestionStatus.ANSWERED : QuestionStatus.NOT_ANSWERED,
        question: q.text[language as string],
        id: q.id,
      })),
      numberOfAnswers: submission ? submission.answers.length : 0,
    };
  }, [allForms, formSubmissions]);

  const { numberOfQuestions, formTitle } = useMemo(() => {
    const form = allForms?.forms.find((form) => form.id === formId);
    return {
      numberOfQuestions: form ? form.questions.length : 0,
      formTitle: `${form?.code} - ${form?.name[language as string]} (${language as string})`,
    };
  }, [allForms]);

  const onQuestionItemClick = (questionId: string) => {
    router.push(`/form-questionnaire/${questionId}?formId=${formId}&language=${language}`);
  };

  const onFormOverviewActionClick = () => {
    // find first unanswered question
    const form = allForms?.forms.find((form) => form.id === formId);
    // do not navigate if the form has no questions or not found
    if (!form || form.questions.length === 0) return;
    // get the first unanswered question
    const lastQ = questions?.find((q) => !answers[q.id]);
    // if all questions are answered get the last question
    const lastQId = lastQ?.id || form?.questions[form.questions.length - 1].id;
    return router.push(`/form-questionnaire/${lastQId}?formId=${formId}&language=${language}`);
  };

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
        rightIcon={<Icon icon="dotsVertical" color="white" />}
        onRightPress={() => setOptionSheetOpen(true)}
      />
      <YStack
        paddingTop={28}
        gap="$xl"
        paddingHorizontal="$md"
        height={Dimensions.get("screen").height - 120}
      >
        <ListView<Pick<FormQuestionListItemProps, "question" | "status"> & { id: string }>
          data={questions}
          ListHeaderComponent={
            <YStack gap="$xl" paddingBottom="$xxs">
              <FormOverview
                completedAnswers={numberOfAnswers}
                numberOfQuestions={numberOfQuestions}
                onFormActionClick={onFormOverviewActionClick}
              />
              <Typography preset="body1" fontWeight="700" gap="$xxs">
                Questions
              </Typography>
            </YStack>
          }
          showsVerticalScrollIndicator={false}
          bounces={false}
          renderItem={({ item, index }) => {
            return (
              <FormQuestionListItem
                key={index}
                {...item}
                index={index + 1}
                numberOfQuestions={numberOfQuestions}
                onClick={onQuestionItemClick.bind(null, item.id)}
              />
            );
          }}
          estimatedItemSize={100}
        />
      </YStack>
      <Sheet
        open={optionSheetOpen}
        modal
        native
        onOpenChange={setOptionSheetOpen}
        snapPointsMode="fit"
        dismissOnSnapToBottom
        zIndex={100_000}
      >
        <Sheet.Overlay />
        <Sheet.Frame borderTopLeftRadius={28} borderTopRightRadius={28}>
          <Icon paddingVertical="$md" alignSelf="center" icon="dragHandle"></Icon>
          <YStack padding="$md" paddingBottom={16 + insets.bottom} gap="$lg">
            <Typography preset="body1">Change language</Typography>
            <Typography preset="body1">Clear form (delete all answers)</Typography>
          </YStack>
        </Sheet.Frame>
      </Sheet>
    </Screen>
  );
};

export default FormDetails;
