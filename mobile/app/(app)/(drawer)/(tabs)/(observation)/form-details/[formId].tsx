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
import { Card, XStack, YStack } from "tamagui";
import CircularProgress from "../../../../../../components/CircularProgress";
import Button from "../../../../../../components/Button";
import { useMemo } from "react";
import { ListView } from "../../../../../../components/ListView";
import CardFooter from "../../../../../../components/CardFooter";
import Badge from "../../../../../../components/Badge";

enum FormStatus {
  NOT_STARTED = "Not Started",
  IN_PROGRESS = "In progress",
  COMPLETED = "Completed",
}

interface FormOverviewProps {
  completedAnswers: number;
  numberOfQuestions: number;
}

const FormOverview = ({ completedAnswers, numberOfQuestions }: FormOverviewProps) => {
  const formStatus = useMemo(() => {
    if (completedAnswers === 0) return FormStatus.NOT_STARTED;
    if (completedAnswers < numberOfQuestions) return FormStatus.IN_PROGRESS;
    if (completedAnswers === numberOfQuestions) return FormStatus.COMPLETED;
  }, [completedAnswers, numberOfQuestions]);

  return (
    <Card padding="$md">
      {/* //TODO: translations */}
      <Typography preset="body1" fontWeight="700">
        Form overview
      </Typography>
      <XStack alignItems="center" justifyContent="space-between">
        <YStack gap="$sm">
          <Typography fontWeight="500" color="$gray5">
            Form status: <Typography fontWeight="700">{formStatus}</Typography>
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
      <Button preset="outlined" marginTop="$md" disabled={completedAnswers === numberOfQuestions}>
        {formStatus === FormStatus.NOT_STARTED ? "Start form" : "Resume form"}
      </Button>
    </Card>
  );
};

interface FormQuestionListItemProps {
  index: number;
  numberOfQuestions: number;
  status: string;
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
        <Badge status="Not started">{status}</Badge>
      </XStack>
      <Typography preset="body2">{question}</Typography>
    </YStack>
    <CardFooter text="No attached notes"></CardFooter>
  </Card>
);

const FormDetails = () => {
  const { formId, language } = useLocalSearchParams();
  console.log("language", language);

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

  const numberOfQuestions = useMemo(() => {
    const form = allForms?.forms.find((form) => form.id === formId);
    return form ? form.questions.length : 0;
  }, [allForms]);

  const numberOfAnswers = useMemo(() => {
    const submission = formSubmissions?.submissions.find(
      (submission) => submission.formId === formId,
    );
    return submission ? submission.answers.length : 0;
  }, [allForms]);

  const questions = useMemo(() => {
    const form = allForms?.forms.find((form) => form.id === formId);
    return form?.questions.map((q) => ({
      status: "Not Answered",
      question: q.text[language as string],
      id: q.id,
    }));
  }, [allForms]);

  const onQuestionItemClick = (questionId: string) => {
    console.log("press on id", questionId);
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
        title={`${formId}`}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />
      <YStack paddingTop={28} gap="$xl" paddingHorizontal="$md">
        <FormOverview completedAnswers={numberOfAnswers} numberOfQuestions={numberOfQuestions} />
        <YStack gap="$xxs">
          <Typography preset="body1" fontWeight="700" gap="$xxs">
            Questions
          </Typography>
          <YStack height={600}>
            <ListView<Pick<FormQuestionListItemProps, "question" | "status"> & { id: string }>
              data={questions}
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
        </YStack>
      </YStack>
    </Screen>
  );
};

export default FormDetails;
