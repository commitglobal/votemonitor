import { ReactNode, useMemo } from "react";
import { SubmissionStatus, mapSubmissionStateStatus } from "../services/form.parser";
import { useTranslation } from "react-i18next";
import Card from "./Card";
import { Typography } from "./Typography";
import { XStack, YStack, useWindowDimensions } from "tamagui";
import CircularProgress from "./CircularProgress";
import Button from "./Button";
import { SubmissionStateToTextMapper } from "../common/utils/form-submissions";
import UnansweredWarning from "./UnansweredWarning";

interface FormOverviewProps {
  completedAnswers: number;
  numberOfQuestions: number;
  isCompleted: boolean;
  onFormActionClick: () => void;
  /** the form description, under the heading */
  description?: ReactNode;
  /** unanswered questions (see unanswered-questions): a warning that leads to the first one */
  unansweredCount?: number;
  onUnansweredPress?: () => void;
}

const FormOverview = ({
  completedAnswers,
  numberOfQuestions,
  isCompleted,
  onFormActionClick,
  description,
  unansweredCount = 0,
  onUnansweredPress,
}: FormOverviewProps) => {
  const formStatus = useMemo(
    () => mapSubmissionStateStatus(completedAnswers, numberOfQuestions, isCompleted),
    [completedAnswers, numberOfQuestions, isCompleted],
  );

  const { t } = useTranslation(["form_overview", "common"]);
  const { width } = useWindowDimensions();

  return (
    <Card padding="$md">
      <Typography preset="body1" fontWeight="700">
        {t("overview.heading")}
      </Typography>
      {description}
      <XStack alignItems="center">
        <YStack gap="$sm" flex={1}>
          <Typography fontWeight="500" color="$gray5">
            {t("overview.status")}:{" "}
            <Typography fontWeight="700">
              {t(SubmissionStateToTextMapper[formStatus], { ns: "common" })}
            </Typography>
          </Typography>
          <Typography fontWeight="500" color="$gray5">
            {t("overview.answered_questions")}:{" "}
            <Typography fontWeight="700">
              {completedAnswers}/{numberOfQuestions}
            </Typography>
          </Typography>
        </YStack>
        {/* TODO: This doesn't look good */}
        <YStack>
          <CircularProgress
            progress={(completedAnswers / numberOfQuestions) * 100}
            size={width > 400 ? 98 : 90}
          />
        </YStack>
      </XStack>

      {unansweredCount > 0 && (
        <UnansweredWarning count={unansweredCount} onPress={onUnansweredPress} marginTop="$md" />
      )}

      <Button
        preset="outlined"
        marginTop="$lg"
        disabled={completedAnswers === numberOfQuestions}
        onPress={onFormActionClick}
      >
        {formStatus === SubmissionStatus.NOT_STARTED
          ? t("overview.start_form")
          : t("overview.resume")}
      </Button>
    </Card>
  );
};

export default FormOverview;
