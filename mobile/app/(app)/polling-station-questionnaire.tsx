import { Typography } from "../../components/Typography";
import { Controller, useForm } from "react-hook-form";
import { View, XStack, YStack } from "tamagui";
import {
  usePollingStationInformation,
  usePollingStationInformationForm,
} from "../../services/queries.service";
import { useUserData } from "../../contexts/user/UserContext.provider";
import { ApiFormQuestion } from "../../services/interfaces/question.type";
import { Screen } from "../../components/Screen";
import CheckboxInput from "../../components/Inputs/CheckboxInput";
import {
  ApiFormAnswer,
  FormQuestionAnswerTypeMapping,
} from "../../services/interfaces/answer.type";
import { useEffect, useMemo, useState } from "react";
import { router } from "expo-router";
import FormInput from "../../components/FormInputs/FormInput";
import DateFormInput from "../../components/FormInputs/DateFormInput";
import RadioFormInput from "../../components/FormInputs/RadioFormInput";
import RatingFormInput from "../../components/FormInputs/RatingFormInput";
import FormElement from "../../components/FormInputs/FormElement";
import { Icon } from "../../components/Icon";
import { useTranslation } from "react-i18next";
import Header from "../../components/Header";
import {
  mapAPIAnswersToFormAnswers,
  mapAPIQuestionsToFormQuestions,
} from "../../services/form.parser";
import Button from "../../components/Button";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { useMutatePollingStationGeneralData } from "../../services/mutations/psi-general.mutation";
import OptionsSheet from "../../components/OptionsSheet";
import { Keyboard } from "react-native";
import WarningDialog from "../../components/WarningDialog";
import { KeyboardAwareScrollView } from "react-native-keyboard-aware-scroll-view";

const PollingStationQuestionnaire = () => {
  const { t, i18n } = useTranslation("polling_station_information_form");
  const [currentLanguage, setCurrentLanguage] = useState(i18n.language.toLocaleUpperCase());
  const [openContextualMenu, setOpenContextualMenu] = useState(false);
  const [clearingForm, setClearingForm] = useState(false);
  const insets = useSafeAreaInsets();

  const { activeElectionRound, selectedPollingStation } = useUserData();

  const { data: formStructure } = usePollingStationInformationForm(activeElectionRound?.id);
  const { data: formData } = usePollingStationInformation(
    activeElectionRound?.id,
    selectedPollingStation?.pollingStationId,
  );

  const questions: Record<string, ApiFormQuestion> = useMemo(
    () => mapAPIQuestionsToFormQuestions(formStructure?.questions),
    [formStructure],
  );
  const answers: Record<string, ApiFormAnswer> = useMemo(
    () => mapAPIAnswersToFormAnswers(formData?.answers),
    [formData],
  );


  useEffect(() => {
    if (formStructure && formStructure?.defaultLanguage && !formStructure?.languages.find(el => el === currentLanguage)) {
      setCurrentLanguage(formStructure?.defaultLanguage);
    }
  }, [formStructure])

  const { mutate } = useMutatePollingStationGeneralData({
    electionRoundId: activeElectionRound?.id,
    pollingStationId: selectedPollingStation?.pollingStationId,
    scopeId: `PS_General_${activeElectionRound?.id}_${selectedPollingStation?.pollingStationId}_answers`,
  });

  const onSubmit = (formData: Record<string, string | string[] | Record<string, any>>) => {
    const answers: ApiFormAnswer[] = Object.keys(formData)
      .map((questionId: string) => {
        const question: ApiFormQuestion = questions[questionId];

        if (!question) return undefined;
        if (!formData[questionId]) return undefined;

        switch (FormQuestionAnswerTypeMapping[question.$questionType]) {
          case "numberAnswer":
            return {
              $answerType: "numberAnswer",
              questionId,
              value: +formData[questionId],
            } as ApiFormAnswer;
          case "ratingAnswer":
            return {
              $answerType: "ratingAnswer",
              questionId,
              value: +formData[questionId],
            } as ApiFormAnswer;
          case "textAnswer":
            return {
              $answerType: "textAnswer",
              questionId,
              text: formData[questionId],
            } as ApiFormAnswer;
          case "dateAnswer":
            return {
              $answerType: "dateAnswer",
              questionId,
              date: new Date(formData[questionId] as string).toISOString(),
            } as ApiFormAnswer;
          case "singleSelectAnswer":
            return {
              $answerType: "singleSelectAnswer",
              questionId,
              selection: {
                optionId: (formData[questionId] as Record<string, string>).radioValue,
                text: (formData[questionId] as Record<string, string>).textValue,
              },
            } as ApiFormAnswer;
          case "multiSelectAnswer": {
            const selections: Record<string, { optionId: string; text: string }> = formData[
              questionId
            ] as Record<string, { optionId: string; text: string }>;
            const mappedSelections = Object.values(selections).map((selection) => ({
              optionId: selection.optionId,
              text: selection.text,
            }));
            return mappedSelections?.length
              ? ({
                  $answerType: "multiSelectAnswer",
                  questionId,
                  selection: mappedSelections,
                } as ApiFormAnswer)
              : undefined;
          }
          default:
            return undefined;
        }
      })
      .filter(Boolean) as ApiFormAnswer[];

    if (activeElectionRound?.id && selectedPollingStation?.pollingStationId) {
      // TODO: How we get rid of so many undefined validations? If we press the button and we don't have the data, is equaly bad
      mutate({
        electionRoundId: activeElectionRound?.id,
        pollingStationId: selectedPollingStation?.pollingStationId,
        answers,
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
      (acc: Record<string, any>, questionId: string) => {
        const answer = answers[questionId];
        switch (answer.$answerType) {
          case "textAnswer":
            acc[questionId] = answer.text;
            break;
          case "numberAnswer":
          case "ratingAnswer":
            acc[questionId] = answer?.value?.toString() ?? "";
            break;
          case "dateAnswer":
            acc[questionId] = answer.date ? new Date(answer.date) : "";
            break;
          case "singleSelectAnswer":
            acc[questionId] = {
              radioValue: answer.selection.optionId,
              textValue: answer.selection.text,
            };
            break;
          case "multiSelectAnswer":
            acc[questionId] = answer.selection.reduce((acc: Record<string, any>, curr) => {
              acc[curr.optionId] = { optionId: curr.optionId, text: curr.text };
              return acc;
            }, {});
            break;
          default:
            break;
        }
        return acc;
      },
      {},
    );

    return formFields;
  };

  const {
    control,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm({
    defaultValues: setFormDefaultValues(),
  });

  return (
    <>
      <Screen
        preset="scroll"
        backgroundColor="white"
        ScrollViewProps={{
          stickyHeaderIndices: [0],
          bounces: false,
        }}
      >
        <Header
          title={t("title")}
          titleColor="white"
          barStyle="light-content"
          leftIcon={<Icon icon="chevronLeft" color="white" />}
          rightIcon={<Icon icon="dotsVertical" color="white" />}
          onLeftPress={() => router.back()}
          onRightPress={() => {
            Keyboard.dismiss();
            setOpenContextualMenu(true);
          }}
        />
        <KeyboardAwareScrollView
          contentContainerStyle={{ flexGrow: 1 }}
          keyboardShouldPersistTaps="handled"
        >
          <YStack padding="$md" gap="$lg" flex={1}>
            {formStructure?.questions.map((question: ApiFormQuestion) => {
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

              if (question.$questionType === "dateQuestion") {
                return (
                  <Controller
                    key={question.id}
                    name={question.id}
                    control={control}
                    render={({ field: { onChange, value } }) => (
                      <YStack>
                        <DateFormInput
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
                    render={({
                      field: { onChange, value = { radioValue: "", textValue: null } },
                    }) => {
                      return (
                        <YStack>
                          <RadioFormInput
                            options={question.options.map((option) => ({
                              id: option.id,
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
                                  key={option.id}
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
                        const selections: Record<string, { optionId: string; text: string }> =
                          value || {};

                        return (
                          <>
                            {question.options.map((option) => (
                              <YStack key={option.id}>
                                <CheckboxInput
                                  marginBottom="$md"
                                  id={option.id}
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
                                {selections[option.id]?.optionId === option.id &&
                                  option.isFreeText && (
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

              return <Typography key={question.id}></Typography>;
            })}
          </YStack>
        </KeyboardAwareScrollView>
        <OptionsSheet open={openContextualMenu} setOpen={setOpenContextualMenu}>
          <OptionSheetContent
            onClear={() => {
              setOpenContextualMenu(false);
              setClearingForm(true);
            }}
          />
        </OptionsSheet>
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
      </Screen>

      <XStack
        backgroundColor="white"
        padding="$xs"
        justifyContent="center"
        paddingBottom={insets.bottom + 10}
        elevation={2}
      >
        <Button flex={1} onPress={handleSubmit(onSubmit)}>
          {t("submit")}
        </Button>
      </XStack>
    </>
  );
};

const OptionSheetContent = ({ onClear }: { onClear: () => void }) => {
  const { t } = useTranslation("polling_station_information_form");

  return (
    <View paddingVertical="$xxs" paddingHorizontal="$sm">
      <Typography preset="body1" color="$gray7" lineHeight={24} onPress={onClear}>
        {t("menu.clear")}
      </Typography>
    </View>
  );
};

export default PollingStationQuestionnaire;
