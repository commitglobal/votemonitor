import { Typography } from "../../components/Typography";
import { Controller, useForm } from "react-hook-form";
import { Sheet, View, XStack, YStack } from "tamagui";
import {
  pollingStationsKeys,
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
import { useMemo, useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import {
  PollingStationInformationAPIPayload,
  PollingStationInformationAPIResponse,
  upsertPollingStationGeneralInformation,
} from "../../services/definitions.api";
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
import Input from "../../components/Inputs/Input";
import Button from "../../components/Button";
import { useSafeAreaInsets } from "react-native-safe-area-context";

const PollingStationQuestionnaire = () => {
  const queryClient = useQueryClient();
  const { t } = useTranslation("polling_station_information");
  const [openContextualMenu, setOpenContextualMenu] = useState(false);
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

  const pollingStationInformationQK = useMemo(
    () =>
      pollingStationsKeys.pollingStationInformation(
        activeElectionRound?.id,
        selectedPollingStation?.pollingStationId,
      ),
    [activeElectionRound, selectedPollingStation],
  );

  const { mutate } = useMutation({
    mutationKey: [pollingStationsKeys.mutatePollingStationGeneralData()],
    mutationFn: async (payload: PollingStationInformationAPIPayload) => {
      return upsertPollingStationGeneralInformation(payload);
    },
    onMutate: async (payload: PollingStationInformationAPIPayload) => {
      // Cancel any outgoing refetches
      // (so they don't overwrite our optimistic update)
      await queryClient.cancelQueries({ queryKey: pollingStationInformationQK });

      // Snapshot the previous value
      const previousData = queryClient.getQueryData<PollingStationInformationAPIResponse>(
        pollingStationInformationQK,
      );

      // Optimistically update to the new value
      queryClient.setQueryData<PollingStationInformationAPIResponse>(pollingStationInformationQK, {
        ...(previousData || {
          id: "-1",
          pollingStationId: payload.pollingStationId,
          arrivalTime: "",
          departureTime: "",
        }),
        answers: payload?.answers || [],
      });

      // Return a context object with the snapshotted value
      return { previousData };
    },
    onError: (err, newData, context) => {
      console.log(err);
      queryClient.setQueryData(pollingStationInformationQK, context?.previousData);
    },
    onSettled: () => {
      // TODO: we want to keep the mutation in pending until the refetch is done?
      return queryClient.invalidateQueries({ queryKey: pollingStationInformationQK });
    },
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
              value: formData[questionId],
            } as ApiFormAnswer;
          case "ratingAnswer":
            return {
              $answerType: "ratingAnswer",
              questionId,
              value: formData[questionId],
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

  const { control, handleSubmit, reset } = useForm({
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
          title={t("header.title")}
          titleColor="white"
          barStyle="light-content"
          leftIcon={<Icon icon="chevronLeft" color="white" />}
          rightIcon={<Icon icon="dotsVertical" color="white" />}
          onLeftPress={() => router.back()}
          onRightPress={() => setOpenContextualMenu(true)}
        />
        <YStack padding="$md" gap="$lg">
          {formStructure?.questions.map((question: ApiFormQuestion) => {
            const _label = `${question.code}. ${question.text.EN}`;
            const _helper = question.helptext.EN;

            if (question.$questionType === "numberQuestion") {
              return (
                <Controller
                  key={question.id}
                  name={question.id}
                  control={control}
                  render={({ field: { onChange, value } }) => (
                    <FormInput
                      title={question.text.EN}
                      placeholder={question.helptext?.EN}
                      type="numeric"
                      value={value}
                      onChangeText={onChange}
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
                  render={({ field: { onChange, value } }) => (
                    <YStack>
                      <FormInput
                        type="textarea"
                        title={question.text.EN}
                        placeholder={question.helptext?.EN}
                        onChangeText={onChange}
                        value={value}
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
                        title={question.text.EN}
                        onChange={onChange}
                        value={value}
                        placeholder={question.helptext?.EN}
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
                            label: option.text.EN,
                            value: option.id,
                          }))}
                          title={question.text.EN}
                          value={value.radioValue}
                          onValueChange={(radioValue) =>
                            onChange({ ...value, radioValue, textValue: null })
                          }
                        />
                        {/* if the selected option is "other", show textArea */}
                        {question.options.map((option) => {
                          if (value.radioValue === option.id && option.isFreeText) {
                            return (
                              <Input
                                type="textarea"
                                marginTop="$md"
                                key={option.id}
                                value={value.textValue}
                                onChangeText={(textValue) => onChange({ ...value, textValue })}
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
                        title={question.text.EN}
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
                <FormElement key={question.id} title={`${question.code}. ${question.text.EN}`}>
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
                                label={option.text.EN}
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
                                  <Input
                                    type="textarea"
                                    marginTop="$md"
                                    value={selections[option.id]?.text}
                                    placeholder="Please enter a text..."
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
        <OptionsSheet
          open={openContextualMenu}
          setOpen={setOpenContextualMenu}
          onClear={() => resetFormValues()}
        />
      </Screen>

      <XStack
        backgroundColor="white"
        padding="$xs"
        justifyContent="center"
        paddingBottom={insets.bottom + 10}
        shadowColor="$gray13"
        shadowOffset={{ width: 0, height: -5 }}
        shadowRadius={3}
        shadowOpacity={0.1}
      >
        <Button flex={1} onPress={handleSubmit(onSubmit)}>
          Submit answer
        </Button>
      </XStack>
    </>
  );
};

interface OptionsSheetProps {
  /* The current state of the sheet */
  open: boolean;

  /* Control the state of the sheet */
  setOpen: (state: boolean) => void;

  /* For future: Triggered action for pressing "Clear form" */
  onClear?: () => void;
}

export const OptionsSheet = (props: OptionsSheetProps) => {
  const { open, setOpen, onClear } = props;
  const { t } = useTranslation("bottom_sheets");
  const insets = useSafeAreaInsets();
  return (
    <Sheet
      modal
      native
      open={open}
      onOpenChange={setOpen}
      zIndex={100_000}
      snapPointsMode="fit"
      dismissOnSnapToBottom
    >
      <Sheet.Overlay />
      <Sheet.Frame
        borderTopLeftRadius={28}
        borderTopRightRadius={28}
        gap="$sm"
        paddingHorizontal="$md"
        paddingBottom="$xl"
        marginBottom={insets.bottom}
      >
        <Icon paddingVertical="$md" alignSelf="center" icon="dragHandle" />

        <View paddingVertical="$xxs" paddingHorizontal="$sm">
          <Typography preset="body1" color="$gray7" lineHeight={24} onPress={onClear}>
            {t("observations.actions.clear_form")}
          </Typography>
        </View>
      </Sheet.Frame>
    </Sheet>
  );
};

export default PollingStationQuestionnaire;
