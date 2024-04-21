import { router, useLocalSearchParams } from "expo-router";
import { Screen } from "../../../components/Screen";
import Header from "../../../components/Header";
import { Icon } from "../../../components/Icon";
import {
  pollingStationsKeys,
  useElectionRoundAllForms,
  useFormSubmissions,
} from "../../../services/queries.service";
import { Typography } from "../../../components/Typography";
import { XStack, YStack } from "tamagui";
import LinearProgress from "../../../components/LinearProgress";
import { useMemo } from "react";
import { useUserData } from "../../../contexts/user/UserContext.provider";
import { ApiFormQuestion } from "../../../services/interfaces/question.type";
import WizzardControls from "../../../components/WizzardControls";
import { ViewStyle } from "react-native";
import { Controller, useForm } from "react-hook-form";
import WizardFormInput from "../../../components/WizardFormInputs/WizardFormInput";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import {
  FormSubmissionAPIPayload,
  FormSubmissionsApiResponse,
  upsertFormSubmission,
} from "../../../services/definitions.api";
import {
  mapAPIAnswersToFormAnswers,
  mapAPIQuestionsToFormQuestions,
  mapFormSubmissionDataToAPIFormSubmissionAnswer,
  setFormDefaultValues,
} from "../../../services/form.parser";
import { ApiFormAnswer } from "../../../services/interfaces/answer.type";
import WizardDateFormInput from "../../../components/WizardFormInputs/WizardDateFormInput";
import WizardRadioFormInput from "../../../components/WizardFormInputs/WizardRadioFormInput";
import WizardFormElement from "../../../components/WizardFormInputs/WizardFormElement";
import CheckboxInput from "../../../components/Inputs/CheckboxInput";
import Input from "../../../components/Inputs/Input";
import WizardRatingFormInput from "../../../components/WizardFormInputs/WizardRatingFormInput";

const FormQuestionnaire = () => {
  const { questionId, formId, language } = useLocalSearchParams();
  const { activeElectionRound, selectedPollingStation } = useUserData();
  const queryClient = useQueryClient();

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

  const {
    control,
    handleSubmit,
    reset,
    formState: { isValid },
  } = useForm({
    defaultValues: setFormDefaultValues(questionId as string, answers[questionId as string]) as any,
  });

  const formSubmissionsQK = useMemo(
    () =>
      pollingStationsKeys.formSubmissions(
        activeElectionRound?.id,
        selectedPollingStation?.pollingStationId as string,
      ),
    [activeElectionRound, selectedPollingStation],
  );

  const currentForm = useMemo(() => {
    const form = allForms?.forms.find((form) => form.id === formId);
    return form;
  }, [allForms]);

  const questions: Record<string, ApiFormQuestion> = useMemo(
    () => mapAPIQuestionsToFormQuestions(currentForm?.questions),
    [currentForm],
  );

  const activeQuestion: {
    index: number;
    question: ApiFormQuestion;
  } = useMemo(
    () => ({
      index: Object.keys(questions).findIndex((key) => key === questionId) || 0,
      question: questions[questionId as string],
    }),
    [questions],
  );

  const formTitle = useMemo(() => {
    return `${currentForm?.code} - ${currentForm?.name[(language as string) || currentForm.defaultLanguage]} (${(language as string) || currentForm?.defaultLanguage})`;
  }, [currentForm]);

  const { mutate: updateSubmission } = useMutation({
    mutationKey: ["upsertFormSubmission"],
    mutationFn: async (payload: FormSubmissionAPIPayload) => {
      return upsertFormSubmission(payload);
    },
    onMutate: async (payload: FormSubmissionAPIPayload) => {
      // Cancel any outgoing refetches
      // (so they don't overwrite our optimistic update)
      await queryClient.cancelQueries({ queryKey: formSubmissionsQK });

      // Snapshot the previous value
      const previousData = queryClient.getQueryData<FormSubmissionsApiResponse>(formSubmissionsQK);

      // // Optimistically update to the new value
      if (previousData && payload.answers) {
        const updatedSubmission = previousData.submissions.find((s) => s.formId === formId);

        queryClient.setQueryData<FormSubmissionsApiResponse>(formSubmissionsQK, {
          submissions: [
            ...previousData.submissions.filter((s) => s.formId !== formId),
            {
              ...payload,
              id: (updatedSubmission?.id as string) || "-1",
            },
          ],
        });
        return;
      }

      // Return a context object with the snapshotted value
      return { previousData };
    },
    onError: (err, newData, context) => {
      console.log(err);
      queryClient.setQueryData(formSubmissionsQK, context?.previousData);
    },
    onSettled: () => {
      // TODO: we want to keep the mutation in pending until the refetch is done?
      return queryClient.invalidateQueries({ queryKey: formSubmissionsQK });
    },
  });

  const onSubmitAnswer = (formValues: any) => {
    const questionId = activeQuestion?.question.id as string;
    if (activeElectionRound?.id && selectedPollingStation?.pollingStationId && activeQuestion) {
      // map the answer values
      const updatedAnswer = mapFormSubmissionDataToAPIFormSubmissionAnswer(
        questionId,
        activeQuestion?.question.$questionType,
        formValues[questionId],
      );

      // update the answer to the question key
      const updatedAnswers = {
        ...answers,
        [activeQuestion.question.id]: updatedAnswer,
      };

      // update the api
      updateSubmission({
        pollingStationId: selectedPollingStation?.pollingStationId as string,
        electionRoundId: activeElectionRound?.id as string,
        formId: currentForm?.id as string,
        answers: Object.values(updatedAnswers).filter(Boolean) as ApiFormAnswer[],
      });

      // get next question
      if (currentForm?.questions && activeQuestion.index === currentForm?.questions.length - 1) {
        // if last question go back
        return router.back();
      }
      const nextQuestionId = currentForm?.questions[activeQuestion.index + 1].id;
      if (nextQuestionId) {
        router.replace(
          `/form-questionnaire/${questions[nextQuestionId].id}?formId=${formId}&language=${language}`,
        );
      }
    }
  };

  // TODO: DO we save the data on back button press
  // TODO: Same with the header back button press
  const onBackButtonPress = () => {
    // get next question
    if (currentForm?.questions && activeQuestion.index === 0) {
      return;
    }
    const previousQuestionId = currentForm?.questions[activeQuestion.index - 1].id;
    if (previousQuestionId) {
      router.replace(
        `/form-questionnaire/${questions[previousQuestionId].id}?formId=${formId}&language=${language}`,
      );
    }
  };

  const onClearForm = () => {
    const formState = setFormDefaultValues(questionId as string);
    reset(formState);
  };

  if (isLoadingForms || isLoadingAnswers) {
    return <Typography>Loading</Typography>;
  }

  if (formsError || answersError) {
    return <Typography>Form Error</Typography>;
  }

  return (
    <Screen
      preset="fixed"
      backgroundColor="white"
      style={$screenStyle}
      contentContainerStyle={$containerStyle}
    >
      <Header
        title={`${formTitle}`}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />
      <YStack padding="$md" style={$containerStyle}>
        <YStack gap="$xxs">
          <XStack justifyContent="space-between">
            <Typography>Form progress</Typography>
            <Typography justifyContent="space-between">{`${activeQuestion?.index + 1}/${currentForm?.questions.length}`}</Typography>
          </XStack>
          <LinearProgress
            current={activeQuestion?.index + 1}
            total={currentForm?.questions.length || 0}
          />
          <XStack justifyContent="flex-end">
            <Typography onPress={onClearForm} color="$red10">
              Clear answer
            </Typography>
          </XStack>
        </YStack>
        <YStack style={{ flex: 1 }} justifyContent="center">
          <Controller
            key={activeQuestion?.question.id}
            name={activeQuestion?.question.id as string}
            rules={{ required: true }}
            control={control}
            render={({ field: { value, onChange } }) => {
              if (!activeQuestion) return <></>;

              const { question } = activeQuestion;
              switch (question.$questionType) {
                case "numberQuestion":
                  return (
                    <WizardFormInput
                      type="numeric"
                      label={`${question.code}. ${question.text[language as string]}`}
                      placeholder={question.inputPlaceholder[language as string]}
                      paragraph={question.helptext[language as string]}
                      onChangeText={onChange}
                      value={value}
                    />
                  );
                case "textQuestion":
                  return (
                    <WizardFormInput
                      type="textarea"
                      label={`${question.code}. ${question.text[language as string]}`}
                      placeholder={question.inputPlaceholder[language as string]}
                      paragraph={question.helptext[language as string]}
                      onChangeText={onChange}
                      maxLength={1000}
                      helper="1000 characters"
                      value={value}
                    />
                  );
                case "dateQuestion":
                  return (
                    <WizardDateFormInput
                      label={`${question.code}. ${question.text[language as string]}`}
                      placeholder="Please enter a date"
                      paragraph={question.helptext[language as string]}
                      onChange={onChange}
                      value={value}
                    />
                  );
                case "singleSelectQuestion":
                  return (
                    <>
                      <WizardRadioFormInput
                        label={`${question.code}. ${question.text[language as string]}`}
                        paragraph={question.helptext[language as string]}
                        options={question.options.map((option) => ({
                          id: option.id,
                          value: option.id,
                          label: option.text[language as string],
                        }))}
                        onValueChange={(radioValue) =>
                          onChange({ ...value, radioValue, textValue: null })
                        }
                        value={value.radioValue || ""}
                      />
                      {question.options.map((option) => {
                        if (option.isFreeText && option.id === value.radioValue) {
                          return (
                            <Input
                              key={option.id}
                              type="textarea"
                              marginTop="$md"
                              value={value.textValue || ""}
                              placeholder="Please enter a text..."
                              onChangeText={(textValue) => {
                                onChange({ ...value, textValue });
                              }}
                            />
                          );
                        }
                        return <></>;
                      })}
                    </>
                  );
                case "multiSelectQuestion":
                  return (
                    <WizardFormElement
                      key={question.id}
                      label={`${question.code}. ${question.text[language as string]}`}
                    >
                      {question.options.map((option) => {
                        const selections: Record<string, { optionId: string; text: string }> =
                          value || {};
                        return (
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
                                  return onChange(
                                    Object.values(rest).filter(Boolean).length > 0 ? rest : "",
                                  );
                                }
                              }}
                            />
                            {selections[option.id]?.optionId === option.id && option.isFreeText && (
                              <Input
                                type="textarea"
                                marginTop="$md"
                                value={selections[option.id]?.text}
                                placeholder="Please enter a text..."
                                onChangeText={(textValue) => {
                                  selections[option.id] = { optionId: option.id, text: textValue };
                                  onChange(selections);
                                }}
                              />
                            )}
                          </YStack>
                        );
                      })}
                    </WizardFormElement>
                  );
                case "ratingQuestion":
                  return (
                    <WizardRatingFormInput
                      type="single"
                      id={question.id}
                      label={`${question.code}. ${question.text[language as string]}`}
                      paragraph={question.helptext[language as string]}
                      scale={question.scale}
                      onValueChange={onChange}
                      value={value}
                    />
                  );
              }
            }}
          />
        </YStack>
      </YStack>
      <WizzardControls
        isFirstElement={activeQuestion?.index === 0}
        isLastElement={
          currentForm?.questions && activeQuestion?.index === currentForm?.questions?.length - 1
        }
        isNextDisabled={!isValid}
        onNextButtonPress={handleSubmit(onSubmitAnswer)}
        onPreviousButtonPress={onBackButtonPress}
      />
    </Screen>
  );
};

const $screenStyle: ViewStyle = {
  backgroundColor: "white",
  justifyContent: "space-between",
};

const $containerStyle: ViewStyle = {
  flex: 1,
};

export default FormQuestionnaire;
