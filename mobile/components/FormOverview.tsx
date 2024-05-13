import { useMemo } from "react";
import { FormStatus, mapFormStateStatus } from "../services/form.parser";
import { useTranslation } from "react-i18next";
import Card from "./Card";
import { Typography } from "./Typography";
import { XStack, YStack } from "tamagui";
import { FormStateToTextMapper } from "./FormCard";
import CircularProgress from "./CircularProgress";
import Button from "./Button";

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
            {t("form_overview.answered_questions")}:{" "}
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
        {formStatus === FormStatus.NOT_STARTED
          ? t("form_overview.start_form")
          : t("form_overview.resume")}
      </Button>
    </Card>
  );
};

export default FormOverview;
