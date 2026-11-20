import { Typography } from "../../../components/Typography";
import { Controller, useForm } from "react-hook-form";
import { View, XStack, YStack } from "tamagui";
import {
  usePollingStationInformation,
  usePollingStationInformationForm,
} from "../../../services/queries.service";
import { useUserData } from "../../../contexts/user/UserContext.provider";
import { ApiFormQuestion } from "../../../services/interfaces/question.type";
import { Screen } from "../../../components/Screen";
import { ScreenScrollView } from "../../../components/ScreenScrollView";
import CheckboxInput from "../../../components/Inputs/CheckboxInput";
import {
  ApiFormAnswer,
  FormQuestionAnswerTypeMapping,
} from "../../../services/interfaces/answer.type";
import { useEffect, useMemo, useRef, useState } from "react";
import { router, useLocalSearchParams } from "expo-router";
import FormInput from "../../../components/FormInputs/FormInput";
import DateFormInput from "../../../components/FormInputs/DateFormInput";
import RadioFormInput from "../../../components/FormInputs/RadioFormInput";
import RatingFormInput from "../../../components/FormInputs/RatingFormInput";
import FormElement from "../../../components/FormInputs/FormElement";
import { Icon } from "../../../components/Icon";
import { useTranslation } from "react-i18next";
import Header from "../../../components/Header";
import {
  mapAPIAnswersToFormAnswers,
  mapAPIQuestionsToFormQuestions,
  mapFormSubmissionDataToAPIFormSubmissionAnswer,
  setFormDefaultValues as answerToFormValue,
  withAnswerTimestamps,
} from "../../../services/form.parser";
import Button from "../../../components/Button";
import { useBottomInset } from "../../../hooks/useBottomInset";
import { useMutatePollingStationGeneralData } from "../../../services/mutations/psi-general.mutation";
import OptionsSheet from "../../../components/OptionsSheet";
import { BackHandler, Keyboard, RefreshControl, ScrollView } from "react-native";
import UnansweredWarning from "../../../components/UnansweredWarning";
import { getUnansweredQuestions } from "../../../services/unanswered-questions";
import WarningDialog from "../../../components/WarningDialog";
import { useNetInfoContext } from "../../../contexts/net-info-banner/NetInfoContext";
import ChangeLanguageDialog from "../../../components/ChangeLanguageDialog";
import { setFormLanguagePreference } from "../../../common/language.preferences";
type SearchParamsType = {
  language: string;
  // the question to scroll to (the unanswered warning on the polling station card leads there)
  questionId?: string;
};
const PollingStationQuestionnaire = () => {
  const { t, i18n } = useTranslation("polling_station_information_form");
  const bottomInset = useBottomInset();
  const { isOnline } = useNetInfoContext();

  const [openContextualMenu, setOpenContextualMenu] = useState(false);
  const [clearingForm, setClearingForm] = useState(false);
  const [showWarningDialog, setShowWarningDialog] = useState(false);
  const [isChangeLanguageModalOpen, setIsChangeLanguageModalOpen] = useState<boolean>(false);
  const { activeElectionRound, selectedPollingStation } = useUserData();
  const { language, questionId: questionIdToShow } = useLocalSearchParams<SearchParamsType>();
  const scrollRef = useRef<ScrollView>(null);
  const hasScrolledRef = useRef(false);

  const {
    data: formStructure,
    refetch: refetchFormStructure,
    isRefetching: isRefetchingFormStructure,
  } = usePollingStationInformationForm(activeElectionRound?.id);

  const {
    data: formData,
    refetch: refetchFormData,
    isRefetching: isRefetchingFormData,
  } = usePollingStationInformation(
    activeElectionRound?.id,
    selectedPollingStation?.pollingStationId,
  );

  const isRefetching = useMemo(
    () => isRefetchingFormStructure || isRefetchingFormData,
    [isRefetchingFormStructure, isRefetchingFormData],
  );

  const handleRefetch = () => {
    refetchFormStructure();
    refetchFormData();
  };

  const questions: Record<string, ApiFormQuestion> = useMemo(
    () => mapAPIQuestionsToFormQuestions(formStructure?.questions),
    [formStructure],
  );
  const answers: Record<string, ApiFormAnswer> = useMemo(
    () => mapAPIAnswersToFormAnswers(formData?.answers),
    [formData],
  );

  const { mutate } = useMutatePollingStationGeneralData({
    electionRoundId: activeElectionRound?.id,
    pollingStationId: selectedPollingStation?.pollingStationId,
    scopeId: `PS_General_${activeElectionRound?.id}_${selectedPollingStation?.pollingStationId}`,
  });

  const currentLanguage = useMemo(() => {
    const activeLanguage = language
      ? language.toLocaleUpperCase()
      : i18n.language.toLocaleUpperCase();

    if (
      formStructure &&
      formStructure?.defaultLanguage &&
      !formStructure?.languages.find((el) => el === activeLanguage)
    ) {
      return formStructure.defaultLanguage;
    }
    return activeLanguage;
  }, [formStructure]);

  const onSetCompletion = (completion: boolean) => {
    if (activeElectionRound?.id && selectedPollingStation?.pollingStationId) {
      mutate({
        electionRoundId: activeElectionRound?.id,
        pollingStationId: selectedPollingStation?.pollingStationId,
        isCompleted: completion,
        lastUpdatedAt: new Date().toISOString(),
      });
      router.back();
    }
  };

  const onSubmit = (formData: Record<string, string | string[] | Record<string, any>>) => {
    const updatedAnswers: ApiFormAnswer[] = Object.keys(formData)
      .map((questionId: string) => {
        const question: ApiFormQuestion = questions[questionId];

        if (!question) return undefined;

        const answer = mapFormSubmissionDataToAPIFormSubmissionAnswer(
          questionId,
          question.$questionType,
          formData[questionId] as Parameters<
            typeof mapFormSubmissionDataToAPIFormSubmissionAnswer
          >[2],
        );
        // a multi select question with nothing picked has no answer
        if (answer?.$answerType === "multiSelectAnswer" && !answer.selection.length) {
          return undefined;
        }
        return answer;
      })
      .map((answer) => answer && withAnswerTimestamps(answer, answers[answer.questionId]))
      .filter(Boolean) as ApiFormAnswer[];

    // Answers to questions this version can't show are kept as they are, not dropped on save.
    const keptAnswers = Object.values(answers).filter((answer) => {
      const question = questions[answer.questionId];
      return !!question && !(question.$questionType in FormQuestionAnswerTypeMapping);
    });

    if (activeElectionRound?.id && selectedPollingStation?.pollingStationId) {
      mutate({
        electionRoundId: activeElectionRound?.id,
        pollingStationId: selectedPollingStation?.pollingStationId,
        answers: [...updatedAnswers, ...keptAnswers],
        lastUpdatedAt: new Date().toISOString(),
      });
      router.back();
    }
  };

  const resetFormValues = () => {
    const formFields: Record<string, any> = Object.keys(questions).reduce(
      (acc: Record<string, any>, questionId: string) => {
        acc[questionId] = "";
        return acc;
      },
      {},
    );

    reset(formFields);

    if (openContextualMenu) {
      setOpenContextualMenu(false);
    }
    setClearingForm(false);
  };

  const setFormDefaultValues = () => {
    const formFields: Record<string, any> = Object.keys(answers).reduce(
      (acc: Record<string, any>, questionId: string) => ({
        ...acc,
        ...answerToFormValue(questionId, answers[questionId]),
      }),
      {},
    );

    return formFields;
  };

  const {
    control,
    handleSubmit,
    reset,
    watch,
    formState: { errors, isDirty },
  } = useForm({
    defaultValues: setFormDefaultValues(),
  });

  // unanswered as filled in on screen, so a warning goes away as soon as its question is answered
  const formValues = watch();
  const unansweredQuestionIds = useMemo(() => {
    const liveAnswers = Object.fromEntries(
      (formStructure?.questions ?? []).map((question) => [
        question.id,
        mapFormSubmissionDataToAPIFormSubmissionAnswer(
          question.id,
          question.$questionType,
          formValues[question.id] as Parameters<
            typeof mapFormSubmissionDataToAPIFormSubmissionAnswer
          >[2],
        ),
      ]),
    );
    return new Set(
      getUnansweredQuestions(formStructure?.questions, liveAnswers).map((question) => question.id),
    );
  }, [formStructure, JSON.stringify(formValues)]);

  const onQuestionLayout = (questionId: string, y: number) => {
    if (questionId !== questionIdToShow || hasScrolledRef.current) return;
    hasScrolledRef.current = true;
    scrollRef.current?.scrollTo({ y, animated: true });
  };

  const onBackPress = () => {
    // if the user changed one of the answers in the meantime -> show condirmation modal
    if (isDirty) {
      Keyboard.dismiss();
      setShowWarningDialog(true);
    } else {
      router.back();
    }
  };

  useEffect(() => {
    const backHandler = BackHandler.addEventListener("hardwareBackPress", () => {
      onBackPress();
      return true;
    });

    return () => backHandler.remove();
  }, [isDirty]);

  const onConfirmFormLanguage = (formId: string, language: string) => {
    setFormLanguagePreference({ formId, language });

    router.replace(`/polling-station-questionnaire?language=${language}`);
    setIsChangeLanguageModalOpen(false);
  };

  const renderQuestion = (question: ApiFormQuestion) => {
    const _label = `${question.code}. ${question.text[currentLanguage]}`;
    const _helper = question.helptext?.[currentLanguage] || "";
    if (question.$questionType === "numberQuestion") {
      return (
        <Controller
          key={question.id}
          name={question.id}
          control={control}
          rules={{
            maxLength: {
              value: 10,
              message: t("form.max", { value: 10 }),
            },
          }}
          render={({ field: { onChange, value } }) => (
            <FormInput
              title={question.text[currentLanguage]}
              placeholder={question.helptext?.[currentLanguage] || ""}
              type="numeric"
              value={value}
              onChangeText={onChange}
              error={errors[question.id]?.message as string}
            />
          )}
        />
      );
    }

    if (question.$questionType === "textQuestion") {
      return (
        <Controller
          key={question.id}
          name={question.id}
          control={control}
          rules={{
            maxLength: { value: 1024, message: t("form.max", { value: 1024 }) },
          }}
          render={({ field: { onChange, value } }) => (
            <YStack>
              <FormInput
                type="textarea"
                title={question.text[currentLanguage]}
                placeholder={question.helptext?.[currentLanguage] || ""}
                onChangeText={onChange}
                value={value}
                error={errors[question.id]?.message as string}
              />
            </YStack>
          )}
        />
      );
    }

    if (
      question.$questionType === "dateQuestion" ||
      question.$questionType === "timeQuestion" ||
      question.$questionType === "dateTimeQuestion"
    ) {
      return (
        <Controller
          key={question.id}
          name={question.id}
          control={control}
          render={({ field: { onChange, value } }) => (
            <YStack>
              <DateFormInput
                mode={
                  question.$questionType === "dateQuestion"
                    ? "date"
                    : question.$questionType === "timeQuestion"
                      ? "time"
                      : "datetime"
                }
                title={question.text[currentLanguage]}
                onChange={onChange}
                value={value}
                placeholder={question.helptext?.[currentLanguage] || ""}
              />
            </YStack>
          )}
        />
      );
    }

    if (question.$questionType === "singleSelectQuestion") {
      return (
        <Controller
          key={question.id}
          name={question.id}
          control={control}
          render={({ field: { onChange, value = { radioValue: "", textValue: null } } }) => {
            return (
              <YStack>
                <RadioFormInput
                  options={question.options.map((option) => ({
                    id: question.id + option.id,
                    label: option.text[currentLanguage],
                    value: option.id,
                  }))}
                  title={question.text[currentLanguage]}
                  value={value.radioValue}
                  onValueChange={(radioValue) =>
                    onChange({ ...value, radioValue, textValue: null })
                  }
                />
                {/* if the selected option is "other", show textArea */}
                {question.options.map((option) => {
                  if (value.radioValue === option.id && option.isFreeText) {
                    return (
                      <FormInput
                        type="textarea"
                        marginTop="$md"
                        key={question.id + option.id}
                        value={value.textValue}
                        placeholder={t("form.placeholder")}
                        onChangeText={(textValue) => onChange({ ...value, textValue })}
                        maxLength={1024}
                        helper={t("form.max", {
                          value: 1024,
                        })}
                      />
                    );
                  }
                  return undefined;
                })}
              </YStack>
            );
          }}
        />
      );
    }

    if (question.$questionType === "ratingQuestion") {
      return (
        <Controller
          key={question.id}
          name={question.id}
          control={control}
          render={({ field: { onChange, value } }) => (
            <YStack>
              <RatingFormInput
                id={question.id}
                type="single"
                title={question.text[currentLanguage]}
                value={value}
                scale={question.scale}
                onValueChange={onChange}
              />
            </YStack>
          )}
        />
      );
    }

    if (question.$questionType === "multiSelectQuestion") {
      return (
        <FormElement
          key={question.id}
          title={`${question.code}. ${question.text[currentLanguage]}`}
        >
          <Controller
            key={question.id}
            name={question.id}
            control={control}
            render={({ field: { onChange, value } }) => {
              const selections: Record<string, { optionId: string; text: string }> = value || {};

              return (
                <>
                  {question.options.map((option) => (
                    <YStack key={question.id + option.id}>
                      <CheckboxInput
                        marginBottom="$md"
                        id={question.id + option.id}
                        label={option.text[currentLanguage]}
                        checked={selections[option.id]?.optionId === option.id}
                        onCheckedChange={(checked) => {
                          if (checked) {
                            return onChange({
                              ...selections,
                              [option.id]: { optionId: option.id, text: null },
                            });
                          } else {
                            const { [option.id]: _toRemove, ...rest } = selections;
                            return onChange(rest);
                          }
                        }}
                      />
                      {selections[option.id]?.optionId === option.id && option.isFreeText && (
                        <FormInput
                          type="textarea"
                          marginTop="$md"
                          value={selections[option.id]?.text}
                          placeholder={t("form.placeholder")}
                          maxLength={1024}
                          helper={t("form.max", {
                            value: 1024,
                          })}
                          onChangeText={(textValue) => {
                            selections[option.id] = {
                              optionId: option.id,
                              text: textValue,
                            };
                            onChange(selections);
                          }}
                        />
                      )}
                    </YStack>
                  ))}
                </>
              );
            }}
          ></Controller>
        </FormElement>
      );
    }

    // A question type this version doesn't know: say so instead of an empty gap.
    return (
      <FormElement key={question.id} title={question.text?.[currentLanguage] ?? ""}>
        <Typography color="$gray5">
          {t("form.unsupported_question", { ns: "polling_station_form_wizard" })}
        </Typography>
      </FormElement>
    );
  };

  return (
    <>
      <Screen
        preset="fixed"
        backgroundColor="white"
        contentContainerStyle={{ flex: 1 }}
        footer={
          <XStack
            backgroundColor="white"
            padding="$xs"
            justifyContent="center"
            paddingBottom={bottomInset + 10}
            elevation={2}
          >
            <Button flex={1} onPress={handleSubmit(onSubmit)}>
              {t("submit")}
            </Button>
          </XStack>
        }
      >
        <Header
          title={t("title")}
          titleColor="white"
          barStyle="light-content"
          leftIcon={<Icon icon="chevronLeft" color="white" />}
          rightIcon={<Icon icon="dotsVertical" color="white" />}
          onLeftPress={onBackPress}
          onRightPress={() => {
            Keyboard.dismiss();
            setOpenContextualMenu(true);
          }}
        />

        <ScreenScrollView
          ref={scrollRef}
          contentContainerStyle={{ flexGrow: 1 }}
          keyboardShouldPersistTaps="handled"
          bounces={isOnline}
          refreshControl={<RefreshControl refreshing={isRefetching} onRefresh={handleRefetch} />}
        >
          <YStack padding="$md" gap="$lg" flex={1}>
            {formStructure?.questions.map((question: ApiFormQuestion) => (
              <YStack
                key={question.id}
                gap="$xs"
                onLayout={(event) => onQuestionLayout(question.id, event.nativeEvent.layout.y)}
              >
                {unansweredQuestionIds.has(question.id) && (
                  <UnansweredWarning label={t("questions.not_answered", { ns: "form_overview" })} />
                )}
                {renderQuestion(question)}
              </YStack>
            ))}
          </YStack>
        </ScreenScrollView>
        {openContextualMenu && (
          <OptionsSheet open setOpen={setOpenContextualMenu}>
            <OptionSheetContent
              onClear={() => {
                setOpenContextualMenu(false);
                setClearingForm(true);
              }}
              onChangeLanguagePress={() => {
                setOpenContextualMenu(false);
                setIsChangeLanguageModalOpen(true);
              }}
              onSetCompletion={onSetCompletion}
              isFullyAnswered={Object.values(answers).length === Object.values(questions).length}
              isCompleted={formData?.isCompleted || false}
            />
          </OptionsSheet>
        )}

        {clearingForm && (
          <WarningDialog
            title={t("warning_modal.title")}
            description={t("warning_modal.description")}
            action={resetFormValues}
            onCancel={() => setClearingForm(false)}
            actionBtnText={t("warning_modal.actions.clear")}
            cancelBtnText={t("warning_modal.actions.cancel")}
          />
        )}

        {showWarningDialog && (
          <WarningDialog
            theme="info"
            title={t("unsaved_changes_dialog.title")}
            description={t("unsaved_changes_dialog.description")}
            action={handleSubmit(onSubmit)}
            onCancel={() => {
              // close dialog and go back without saving changes
              setShowWarningDialog(false);
              router.back();
            }}
            actionBtnText={t("unsaved_changes_dialog.actions.save")}
            cancelBtnText={t("unsaved_changes_dialog.actions.discard")}
          />
        )}

        {isChangeLanguageModalOpen && formStructure?.languages && (
          <ChangeLanguageDialog
            currentLanguage={currentLanguage}
            formId={formStructure?.id as string}
            languages={formStructure.languages}
            onCancel={setIsChangeLanguageModalOpen.bind(null, false)}
            onSelectLanguage={onConfirmFormLanguage}
          />
        )}
      </Screen>
    </>
  );
};

const OptionSheetContent = ({
  onClear,
  onSetCompletion,
  onChangeLanguagePress,
  isCompleted,
  isFullyAnswered,
}: {
  onClear: () => void;
  onSetCompletion: (completion: boolean) => void;
  onChangeLanguagePress: () => void;
  isCompleted: boolean;
  isFullyAnswered: boolean;
}) => {
  const { t } = useTranslation(["polling_station_information_form", "common"]);

  const disableMarkAsDone = useMemo(
    () => isFullyAnswered && !isCompleted,
    [isFullyAnswered, isCompleted],
  );

  return (
    <View paddingVertical="$xxs" paddingHorizontal="$sm" gap="$xxs">
      <Typography
        preset="body1"
        color={disableMarkAsDone ? "$gray3" : "$gray9"}
        paddingVertical="$xs"
        lineHeight={24}
        onPress={() => {
          onSetCompletion(!isCompleted);
        }}
        disabled={disableMarkAsDone}
      >
        {!isCompleted
          ? t("forms.mark_as_done", { ns: "common" })
          : t("forms.mark_as_in_progress", { ns: "common" })}
      </Typography>
      <Typography preset="body1" paddingVertical="$xs" onPress={onChangeLanguagePress}>
        {t("menu.change_language")}
      </Typography>
      <Typography preset="body1" color="$red10" paddingVertical="$xs" onPress={onClear}>
        {t("menu.clear")}
      </Typography>
    </View>
  );
};

export default PollingStationQuestionnaire;
