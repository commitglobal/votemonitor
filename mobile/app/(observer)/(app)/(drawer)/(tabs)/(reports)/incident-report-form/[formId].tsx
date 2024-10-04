import * as Crypto from "expo-crypto";
import { router, useLocalSearchParams } from "expo-router";
import { createRef, useMemo, useRef, useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { Keyboard, RefreshControl } from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { ScrollView, View, XStack, YStack } from "tamagui";
import DateFormInput from "../../../../../../../components/FormInputs/DateFormInput";
import FormElement from "../../../../../../../components/FormInputs/FormElement";
import FormInput from "../../../../../../../components/FormInputs/FormInput";
import RadioFormInput from "../../../../../../../components/FormInputs/RadioFormInput";
import RatingFormInput from "../../../../../../../components/FormInputs/RatingFormInput";
import Header from "../../../../../../../components/Header";
import { Icon } from "../../../../../../../components/Icon";
import CheckboxInput from "../../../../../../../components/Inputs/CheckboxInput";
import OptionsSheet from "../../../../../../../components/OptionsSheet";
import { Screen } from "../../../../../../../components/Screen";
import { Typography } from "../../../../../../../components/Typography";
import WarningDialog from "../../../../../../../components/WarningDialog";
import { useUserData } from "../../../../../../../contexts/user/UserContext.provider";
import { scrollToTextarea } from "../../../../../../../helpers/scrollToTextarea";
import { IncidentReportLocationType } from "../../../../../../../services/api/incident-report/post-incident-report.api";
import {
  mapAPIAnswersToFormAnswers,
  mapAPIQuestionsToFormQuestions,
} from "../../../../../../../services/form.parser";
import {
  ApiFormAnswer,
  FormQuestionAnswerTypeMapping,
} from "../../../../../../../services/interfaces/answer.type";
import { ApiFormQuestion } from "../../../../../../../services/interfaces/question.type";
import { useIncidentReportMutation } from "../../../../../../../services/mutations/form-submission.mutation";
import { useFormById } from "../../../../../../../services/queries/forms.query";
import { useIncidentReportById } from "../../../../../../../services/queries/incident-reports.query";
import i18n from "../../../../../../../common/config/i18n";
import Select from "../../../../../../../components/Select";
import { PollingStationVisitVM } from "../../../../../../../common/models/polling-station.model";
import Button from "../../../../../../../components/Button";

type SearchParamsType = {
  formId: string;
  language: string;
};

const mapVisitsToSelectPollingStations = (visits: PollingStationVisitVM[] = []) => {
  const pollingStationsForSelect = visits.map((visit) => {
    return {
      id: visit.pollingStationId,
      value: visit.pollingStationId,
      label: `${visit.number} - ${visit.address}`,
    };
  });

  //   adding the 'other'
  pollingStationsForSelect.push({
    id: "other",
    value: IncidentReportLocationType.OtherLocation,
    label: i18n.t("form.polling_station_id.options.other", { ns: "incident_report" }),
  });
  return pollingStationsForSelect;
};

const IncidentReportFormDetails = () => {
  const { formId, language } = useLocalSearchParams<SearchParamsType>();

  if (!formId || !language) {
    return <Typography>Incorrect page params</Typography>;
  }
  const { activeElectionRound, visits } = useUserData();

  const { t, i18n } = useTranslation("incident_report");
  const pollingStations = useMemo(() => mapVisitsToSelectPollingStations(visits), [visits]);

  const [openContextualMenu, setOpenContextualMenu] = useState(false);
  const [clearingForm, setClearingForm] = useState(false);
  const [showWarningDialog, setShowWarningDialog] = useState(false);

  const insets = useSafeAreaInsets();

  const incidentReportId = Crypto.randomUUID();
  const {
    data: formStructure,
    refetch: refetchFormStructure,
    isRefetching: isRefetchingFormStructure,
  } = useFormById(activeElectionRound?.id, formId);

  const {
    data: formData,
    refetch: refetchFormData,
    isRefetching: isRefetchingFormData,
  } = useIncidentReportById(activeElectionRound?.id, incidentReportId);

  const isRefetching = useMemo(
    () => isRefetchingFormStructure || isRefetchingFormData,
    [isRefetchingFormStructure, isRefetchingFormData],
  );

  const handleRefetch = () => {
    refetchFormStructure();
    refetchFormData();
  };

  const currentLanguage = useMemo(() => {
    const activeLanguage = i18n.language.toLocaleUpperCase();
    if (
      formStructure &&
      formStructure?.defaultLanguage &&
      !formStructure?.languages.find((el) => el === activeLanguage)
    ) {
      return formStructure.defaultLanguage;
    }
    return activeLanguage;
  }, [formStructure]);

  const questions: Record<string, ApiFormQuestion> = useMemo(
    () => mapAPIQuestionsToFormQuestions(formStructure?.questions),
    [formStructure],
  );

  const answers: Record<string, ApiFormAnswer> = useMemo(
    () => mapAPIAnswersToFormAnswers(formData?.answers),
    [formData],
  );

  const { mutate } = useIncidentReportMutation({
    electionRoundId: activeElectionRound?.id,
    incidentReportId,
    scopeId: `IncidentReport_${activeElectionRound?.id}_${incidentReportId}_answers`,
  });

  const onSubmit = (formData: Record<string, string | string[] | Record<string, any>>) => {
    if (!visits || !activeElectionRound) {
      return;
    }

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

    let incidentReportLocationType = IncidentReportLocationType.PollingStation;
    let pollingStationId: string | undefined = formData.polling_station_id as string | undefined;
    let locationDescription: string | undefined;

    if (
      IncidentReportLocationType.OtherLocation === (pollingStationId as IncidentReportLocationType)
    ) {
      incidentReportLocationType = pollingStationId as IncidentReportLocationType;
      pollingStationId = undefined;
      locationDescription = formData.location_description as string | undefined;
    } else {
      locationDescription = undefined;
    }

    // TODO: How we get rid of so many undefined validations? If we press the button and we don't have the data, is equaly bad
    mutate({
      electionRoundId: activeElectionRound?.id,
      id: incidentReportId,
      formId,
      // TODO: add missing fields
      locationType: incidentReportLocationType,
      locationDescription,
      pollingStationId: pollingStationId as string | undefined,
      answers,
    });
    console.log("jdjdjdj");
    router.push("/(reports)");
  };

  const resetFormValues = () => {
    const formFields: Record<string, any> = Object.keys(questions).reduce(
      (acc: Record<string, any>, questionId: string) => {
        acc[questionId] = "";
        return acc;
      },
      {},
    );

    formFields.location_type = "";
    formFields.location_description = "";
    formFields.polling_station_id = "";

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
    watch,
    formState: { errors, isDirty },
  } = useForm({
    defaultValues: setFormDefaultValues(),
  });

  // ref for the scrollview
  const scrollViewRef = useRef(null);

  // create  a ref for each question we receive that has any of the types: "textQuestion", "singleSelectQuestion", "multiSelectQuestion"
  // so basically for all of the cases where we might deal with a 'textarea' that needs scrolling to
  const questionRefs = useRef(
    formStructure?.questions.map((question) => {
      if (
        question.$questionType === "textQuestion" ||
        question.$questionType === "singleSelectQuestion" ||
        question.$questionType === "multiSelectQuestion"
      ) {
        return createRef();
      }
      return null;
    }),
  );

  const locationTypeWatch = watch("polling_station_id");

  const locationDescriptionRef = createRef();

  const onBackPress = () => {
    // if the user changed one of the answers in the meantime -> show confirmation modal
    if (isDirty) {
      Keyboard.dismiss();
      setShowWarningDialog(true);
    } else {
      router.back();
    }
  };

  return (
    <>
      <Screen preset="fixed" backgroundColor="white" contentContainerStyle={{ flex: 1 }}>
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

        <ScrollView
          ref={scrollViewRef}
          contentContainerStyle={{ flexGrow: 1 }}
          keyboardShouldPersistTaps="handled"
          refreshControl={<RefreshControl refreshing={isRefetching} onRefresh={handleRefetch} />}
        >
          <YStack padding="$md" gap="$lg" flex={1}>
            <Controller
              key="polling_station_id"
              name="polling_station_id"
              control={control}
              rules={{
                required: {
                  value: true,
                  message: t("form.polling_station_id.required"),
                },
              }}
              render={({ field: { onChange, value } }) => (
                <>
                  {/* select location type */}
                  <FormElement
                    title={t("form.polling_station_id.label")}
                    error={errors.polling_station_id?.message as string}
                  >
                    <Select
                      value={value}
                      options={pollingStations}
                      placeholder={t("form.polling_station_id.placeholder")}
                      onValueChange={onChange}
                      error={errors.polling_station_id?.message as string}
                    />
                  </FormElement>
                </>
              )}
            />

            {/* location details details */}

            {locationTypeWatch === IncidentReportLocationType.OtherLocation && (
              <Controller
                key="location_description"
                name="location_description"
                control={control}
                rules={{
                  maxLength: { value: 2048, message: t("form.max", { value: 2048 }) },
                  required: {
                    value: true,
                    message: t("form.location_description.required"),
                  },
                }}
                render={({ field: { onChange, value } }) => (
                  <YStack>
                    <FormInput
                      type="textarea"
                      ref={locationDescriptionRef}
                      onFocus={() => scrollToTextarea(scrollViewRef, locationDescriptionRef)}
                      title={t("form.location_description.label")}
                      placeholder={t("form.location_description.placeholder")}
                      onChangeText={onChange}
                      value={value}
                      error={errors.location_description?.message as string}
                    />
                  </YStack>
                )}
              />
            )}

            {formStructure?.questions.map((question: ApiFormQuestion, index: number) => {
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
                          ref={questionRefs.current && questionRefs.current[index]}
                          onFocus={() =>
                            scrollToTextarea(
                              scrollViewRef,
                              questionRefs.current && questionRefs.current[index],
                            )
                          }
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
                                  ref={questionRefs.current && questionRefs.current[index]}
                                  onFocus={() =>
                                    scrollToTextarea(
                                      scrollViewRef,
                                      questionRefs.current && questionRefs.current[index],
                                    )
                                  }
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
                                      ref={questionRefs.current && questionRefs.current[index]}
                                      onFocus={() =>
                                        scrollToTextarea(
                                          scrollViewRef,
                                          questionRefs.current && questionRefs.current[index],
                                        )
                                      }
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
        </ScrollView>
        {openContextualMenu && (
          <OptionsSheet open setOpen={setOpenContextualMenu}>
            <OptionSheetContent
              onClear={() => {
                setOpenContextualMenu(false);
                setClearingForm(true);
              }}
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

export default IncidentReportFormDetails;
