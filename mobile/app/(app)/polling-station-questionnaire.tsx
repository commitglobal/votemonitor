import { ViewStyle } from "react-native";
import { Typography } from "../../components/Typography";
import { Controller, useForm } from "react-hook-form";
import WizardFormInput from "../../components/WizardFormInputs/WizardFormInput";
import { Button, CheckedState, YStack } from "tamagui";
import {
  pollingStationsKeys,
  usePollingStationInformation,
  usePollingStationInformationForm,
} from "../../services/queries.service";
import { useUserData } from "../../contexts/user/UserContext.provider";
import { ApiFormQuestion } from "../../services/interfaces/question.type";
import WizardDateFormInput from "../../components/WizardFormInputs/WizardDateFormInput";
import WizardRadioFormInput from "../../components/WizardFormInputs/WizardRadioFormInput";
import { Screen } from "../../components/Screen";
import CheckboxInput from "../../components/Inputs/CheckboxInput";
import WizardFormElement from "../../components/WizardFormInputs/WizardFormElement";
import {
  ApiFormAnswer,
  FormQuestionAnswerTypeMapping,
} from "../../services/interfaces/answer.type";
import { useMemo } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import {
  PollingStationInformationAPIPayload,
  PollingStationInformationAPIResponse,
  upsertPollingStationGeneralInformation,
} from "../../services/definitions.api";
import { router } from "expo-router";
import WizardRatingFormInput from "../../components/WizardFormInputs/WizardRatingFormInput";

const PollingStationQuestionnaire = () => {
  const queryClient = useQueryClient();

  const { activeElectionRound, selectedPollingStation } = useUserData();

  const { data: formStructure } = usePollingStationInformationForm(activeElectionRound?.id);
  const { data: formData } = usePollingStationInformation(
    activeElectionRound?.id,
    selectedPollingStation?.pollingStationId,
  );

  const questions: Record<string, ApiFormQuestion> = useMemo(
    () =>
      formStructure?.questions.reduce(
        (acc: Record<string, ApiFormQuestion>, curr: ApiFormQuestion) => {
          acc[curr.id] = curr;

          return acc;
        },
        {},
      ) || {},
    [formStructure],
  );
  const answers: Record<string, ApiFormAnswer> = useMemo(
    () =>
      formData?.answers.reduce((acc: Record<string, ApiFormAnswer>, curr: ApiFormAnswer) => {
        acc[curr.questionId] = curr;

        return acc;
      }, {}) || {},
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
    mutationKey: ["upsertPollingStationGeneralInformation"],
    mutationFn: async (payload: PollingStationInformationAPIPayload) => {
      return upsertPollingStationGeneralInformation(payload);
    },
    onMutate: async (payload: PollingStationInformationAPIPayload) => {
      // Cancel any outgoing refetches
      // (so they don't overwrite our optimistic update)
      await queryClient.cancelQueries({ queryKey: [pollingStationInformationQK] });

      // Snapshot the previous value
      const previousData = queryClient.getQueryData<PollingStationInformationAPIResponse>(
        pollingStationInformationQK,
      );

      // Optimistically update to the new value
      if (previousData && payload?.answers) {
        // TODO: improve this
        queryClient.setQueryData<PollingStationInformationAPIResponse>(
          pollingStationInformationQK,
          {
            ...previousData,
            answers: payload.answers,
          },
        );
      }

      // Return a context object with the snapshotted value
      return { previousData };
    },
    onError: (err, newData, context) => {
      console.log(err);
      queryClient.setQueryData([pollingStationInformationQK], context?.previousData);
    },
    onSettled: () => {
      // TODO: we want to keep the mutation in pending until the refetch is done?
      return queryClient.invalidateQueries({ queryKey: [pollingStationInformationQK] });
    },
  });

  const onSubmit = (formData: Record<string, string | string[]>) => {
    console.log(formData);

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
                optionId: formData[questionId],
                // text: '' //TODO: handle free text
              },
            } as ApiFormAnswer;
          case "multiSelectAnswer": {
            const selections: string[] = formData[questionId] as string[];
            return {
              $answerType: "multiSelectAnswer",
              questionId,
              selection: selections.map((optionId) => ({
                optionId,
                // text: '' //TODO: handle free text
              })),
            } as ApiFormAnswer;
          }
          default:
            return undefined;
        }
      })
      .filter(Boolean) as ApiFormAnswer[];

    console.log("answers", JSON.stringify(answers));

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

  const handleCheckboxChange = (name: string, value: string, checked: CheckedState) => {
    // Get the current state of missingMaterials from the form
    const currentValues = getValues(name) || [];

    // Update the state based on whether the checkbox was checked or unchecked
    const updated = checked
      ? [...currentValues, value] // Add to the array if checked
      : currentValues.filter((v: any) => v !== value); // Remove from the array if unchecked

    // Update the missingMaterials state
    setValue(name, updated, { shouldValidate: true });
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
            acc[questionId] = answer.selection.optionId;
            break;
          case "multiSelectAnswer":
            acc[questionId] = answer.selection.map((o) => o.optionId);
            break;
          default:
            break;
        }
        return acc;
      },
      {},
    );

    console.log("Reseting values to ", formFields);
    return formFields;
  };

  const {
    control,
    handleSubmit,
    // formState: { errors },
    getValues,
    setValue,
  } = useForm({
    defaultValues: setFormDefaultValues(),
  });

  return (
    <Screen preset="scroll" contentContainerStyle={$containerStyle}>
      <Typography>This is the polling station questionnaire</Typography>
      {formStructure?.questions.map((question: ApiFormQuestion) => {
        const label = `${question.code}. ${question.text.EN}`;
        const helper = question.helptext.EN;

        if (question.$questionType === "numberQuestion") {
          return (
            <Controller
              key={question.id}
              name={question.id}
              control={control}
              render={({ field: { onChange, value } }) => (
                <YStack>
                  <WizardFormInput
                    type="numeric"
                    label={label}
                    helper={helper}
                    onChangeText={onChange}
                    value={value}
                  />
                </YStack>
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
                  <WizardFormInput
                    type="text"
                    label={label}
                    helper={helper}
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
                  <WizardDateFormInput
                    label={label}
                    helper={helper}
                    onChange={onChange}
                    value={value}
                  />
                </YStack>
              )}
            />
          );
        }

        if (question.$questionType === "singleSelectQuestion") {
          return (
            // TODO: need to handle free text option
            <Controller
              key={question.id}
              name={question.id}
              control={control}
              render={({ field: { onChange, value } }) => (
                <YStack>
                  <WizardRadioFormInput
                    options={question.options.map((option) => ({
                      id: option.id,
                      label: option.text.EN,
                      value: option.id,
                    }))}
                    label={label}
                    value={value}
                    onValueChange={onChange}
                  />
                </YStack>
              )}
            />
          );
        }

        if (question.$questionType === "ratingQuestion") {
          return (
            // TODO: need to handle free text option
            <Controller
              key={question.id}
              name={question.id}
              control={control}
              render={({ field: { onChange, value } }) => (
                <YStack>
                  <WizardRatingFormInput
                    id={question.id}
                    type="single"
                    label={label}
                    value={value}
                    onValueChange={onChange}
                  />
                </YStack>
              )}
            />
          );
        }

        if (question.$questionType === "multiSelectQuestion") {
          return (
            <WizardFormElement key={question.id} label={`${question.code}. ${question.text.EN}`}>
              {question.options.map((option) => (
                <Controller
                  key={option.id}
                  name={question.id} // TODO: maybe more?
                  control={control}
                  render={() => (
                    <YStack>
                      <CheckboxInput
                        marginBottom="$md"
                        id={option.id}
                        label={option.text.EN}
                        checked={!!getValues(question.id)?.includes(option.id)}
                        onCheckedChange={(checked) =>
                          handleCheckboxChange(question.id, option.id, checked)
                        }
                      />
                    </YStack>
                  )}
                />
              ))}
            </WizardFormElement>
          );
        }

        return <Typography key={question.id}></Typography>;
      })}
      <Button onPress={handleSubmit(onSubmit)}>Submit answer</Button>
    </Screen>
  );
};

const $containerStyle: ViewStyle = {
  padding: 10,
};

export default PollingStationQuestionnaire;
