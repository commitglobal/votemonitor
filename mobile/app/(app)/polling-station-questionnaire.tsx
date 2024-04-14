import { ViewStyle } from "react-native";
import { Typography } from "../../components/Typography";
import { Controller, useForm } from "react-hook-form";
import FormInput from "../../components/FormInputs/FormInput";
import { Button, CheckedState, YStack } from "tamagui";
import {
  upsertPollingStationGeneralInformationMutation,
  usePollingStationInformationForm,
} from "../../services/queries.service";
import { useUserData } from "../../contexts/user/UserContext.provider";
import { ApiFormQuestion } from "../../services/interfaces/question.type";
import DateFormInput from "../../components/FormInputs/DateFormInput";
import RadioFormInput from "../../components/FormInputs/RadioFormInput";
import { Screen } from "../../components/Screen";
import CheckboxInput from "../../components/Inputs/CheckboxInput";
import FormElement from "../../components/FormInputs/FormElement";
import {
  ApiFormAnswer,
  FormQuestionAnswerTypeMapping,
} from "../../services/interfaces/answer.type";

const PollingStationQuestionnaire = () => {
  const {
    control,
    handleSubmit,
    formState: { errors },
    getValues,
    setValue,
  } = useForm();

  const { activeElectionRound, selectedPollingStation } = useUserData();

  const { data } = usePollingStationInformationForm(activeElectionRound?.id);

  const { mutate } = upsertPollingStationGeneralInformationMutation();

  const onSubmit = (formData: Record<string, string | string[]>) => {
    console.log(formData);

    const answers: ApiFormAnswer[] = Object.keys(formData)
      .map((questionId: string) => {
        const question: ApiFormQuestion | undefined = data?.questions.find(
          (q) => q.id === questionId,
        );

        if (!question) return undefined;
        if (!formData[questionId]) return undefined;

        switch (FormQuestionAnswerTypeMapping[question.$questionType]) {
          case "numberAnswer":
          case "ratingAnswer":
            return {
              $answerType: "numberAnswer",
              questionId,
              value: formData[questionId],
            } as ApiFormAnswer;
          case "textAnswer":
            return {
              $answerType: "textAnswer",
              questionId,
              Text: formData[questionId],
            } as ApiFormAnswer;
          case "dateAnswer":
            return {
              $answerType: "dateAnswer",
              questionId,
              Date: new Date(formData[questionId] as string).toISOString(),
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
      // TODO Cum scapam de asta elegant?
      mutate(
        {
          electionRoundId: activeElectionRound?.id,
          pollingStationId: selectedPollingStation?.pollingStationId,
          answers,
        },
        {
          onSuccess: () => {
            console.log("A mers");
          },
          onError: (err) => {
            console.log("N-a mers", err);
          },
        },
      );
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

  return (
    <Screen preset="scroll" contentContainerStyle={$containerStyle}>
      <Typography>This is the polling station questionnaire</Typography>
      {data?.questions.map((question: ApiFormQuestion) => {
        if (
          ["numberQuestion", "textQuestion", "dateQuestion", "singleSelectQuestion"].includes(
            question.$questionType,
          )
        ) {
          return (
            <Controller
              key={question.id}
              name={question.id}
              control={control}
              render={({ field: { onChange, value } }) => (
                <YStack>
                  {question.$questionType === "numberQuestion" && (
                    <FormInput
                      type="numeric"
                      label={`${question.code}. ${question.text.EN}`}
                      helper={question.helptext.EN}
                      onChangeText={onChange}
                      value={value}
                    />
                  )}
                  {question.$questionType === "textQuestion" && (
                    <FormInput
                      type="text"
                      label={`${question.code}. ${question.text.EN}`}
                      helper={question.helptext.EN}
                      onChangeText={onChange}
                      value={value}
                    />
                  )}
                  {question.$questionType === "dateQuestion" && (
                    <DateFormInput
                      label={`${question.code}. ${question.text.EN}`}
                      helper={question.helptext.EN}
                      onChange={onChange}
                      value={value}
                    />
                  )}
                  {question.$questionType === "singleSelectQuestion" && (
                    // TODO: need to handle free text option
                    <RadioFormInput
                      options={question.options.map((option) => ({
                        id: option.id,
                        label: option.text.EN,
                        value: option.id,
                      }))}
                      label={`${question.code}. ${question.text.EN}`}
                      value={value}
                      onValueChange={onChange}
                    />
                  )}
                  {/* {question.$questionType === "ratingQuestion" && (
                    // TODO: need to handle free text option
                    <RadioFormInput
                      options={question.options.map((option) => ({
                        id: option.id,
                        label: option.text.EN,
                        value: option.id,
                      }))}
                      label={`${question.code}. ${question.text.EN}`}
                      value={value}
                      onValueChange={onChange}
                    />
                  )} */}
                </YStack>
              )}
            />
          );
        }

        if (question.$questionType === "multiSelectQuestion") {
          return (
            <FormElement key={question.id} label={`${question.code}. ${question.text.EN}`}>
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
            </FormElement>
          );
        }

        return <Typography key={question.id}></Typography>;
      })}
      <Button onPress={handleSubmit(onSubmit)}>Submit answer</Button>
      <Button onPress={() => console.log(getValues())}>Get VAlues</Button>
      <Button onPress={() => console.log(getValues("5b7c0c8c-c116-4f85-8b14-14137fe91f6a"))}>
        Get VAlues for multi
      </Button>
    </Screen>
  );
};

const $containerStyle: ViewStyle = {
  padding: 10,
};

export default PollingStationQuestionnaire;
