import React, { forwardRef } from "react";
import { Control, Controller } from "react-hook-form";
import WizardFormInput from "./WizardFormInputs/WizardFormInput";
import WizardDateFormInput from "./WizardFormInputs/WizardDateFormInput";
import WizardRadioFormInput from "./WizardFormInputs/WizardRadioFormInput";
import FormInput from "./FormInputs/FormInput";
import WizardFormElement from "./WizardFormInputs/WizardFormElement";
import { YStack } from "tamagui";
import CheckboxInput from "./Inputs/CheckboxInput";
import WizardRatingFormInput from "./WizardFormInputs/WizardRatingFormInput";
import { useLocalSearchParams } from "expo-router";
import { SearchParamType } from "../app/(observer)/(app)/form-questionnaire/[questionId]";
import { Typography } from "./Typography";
import { useTranslation } from "react-i18next";
import { ApiFormQuestion } from "../services/interfaces/question.type";

interface IQuestionFormProps {
  control: Control<any, any>;
  activeQuestion: IActiveQuestion;
  handleFocus?: () => void;
}

interface IActiveQuestion {
  indexInAllQuestions: number;
  indexInDisplayedQuestions: number;
  question: ApiFormQuestion;
}

const QuestionForm = forwardRef(
  ({ control, activeQuestion, handleFocus }: IQuestionFormProps, textareaRef) => {
    const { questionId, language } = useLocalSearchParams<SearchParamType>();
    const { t } = useTranslation("polling_station_form_wizard");

    if (!language || !questionId) {
      return <Typography>Incorrect page params</Typography>;
    }

    return (
      <>
        <Controller
          key={activeQuestion?.question?.id}
          name={activeQuestion?.question?.id}
          rules={{ required: false }}
          control={control}
          render={({ field: { value, onChange } }) => {
            if (!activeQuestion) return <></>;

            const { question } = activeQuestion;
            switch (question.$questionType) {
              case "numberQuestion":
                return (
                  <WizardFormInput
                    type="numeric"
                    label={`${question.code}. ${question.text[language]}`}
                    placeholder={question?.inputPlaceholder?.[language] || ""}
                    paragraph={question.helptext?.[language] || ""}
                    onChangeText={onChange}
                    value={value}
                    maxLength={10}
                    helper={t("form.max", {
                      value: 10,
                    })}
                  />
                );
              case "textQuestion":
                return (
                  <WizardFormInput
                    type="textarea"
                    ref={textareaRef}
                    onFocus={handleFocus}
                    label={`${question.code}. ${question.text[language]}`}
                    placeholder={question?.inputPlaceholder?.[language] || ""}
                    paragraph={question.helptext?.[language] || ""}
                    onChangeText={onChange}
                    maxLength={1024}
                    helper={t("form.max", {
                      value: 1024,
                    })}
                    value={value}
                  />
                );
              case "dateQuestion":
                return (
                  <WizardDateFormInput
                    label={`${question.code}. ${question.text[language]}`}
                    placeholder={t("form.date_placeholder")}
                    paragraph={question.helptext?.[language] || ""}
                    onChange={onChange}
                    value={value}
                  />
                );
              case "singleSelectQuestion":
                return (
                  <>
                    <WizardRadioFormInput
                      label={`${question.code}. ${question.text[language]}`}
                      paragraph={question.helptext?.[language] || ""}
                      options={question.options.map((option) => ({
                        id: option.id,
                        value: option.id,
                        label: option.text[language],
                      }))}
                      onValueChange={(radioValue) =>
                        onChange({ ...value, radioValue, textValue: null })
                      }
                      value={value.radioValue || ""}
                    />
                    {question.options.map((option) => {
                      if (option.isFreeText && option.id === value.radioValue) {
                        return (
                          <FormInput
                            type="textarea"
                            ref={textareaRef}
                            onFocus={handleFocus}
                            key={option.id + "free"}
                            marginTop="$md"
                            value={value.textValue || ""}
                            placeholder={t("form.text_placeholder")}
                            onChangeText={(textValue) => {
                              onChange({ ...value, textValue });
                            }}
                            maxLength={1024}
                            helper={t("form.max", {
                              value: 1024,
                            })}
                          />
                        );
                      }
                      return false;
                    })}
                  </>
                );
              case "multiSelectQuestion":
                return (
                  <WizardFormElement
                    key={question.id}
                    label={`${question.code}. ${question.text[language]}`}
                    paragraph={question.helptext?.[language] || ""}
                  >
                    {question.options.map((option) => {
                      const selections: Record<string, { optionId: string; text: string }> =
                        value || {};
                      return (
                        <YStack key={option.id}>
                          <CheckboxInput
                            marginBottom="$md"
                            id={option.id}
                            label={option.text[language]}
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
                            <FormInput
                              ref={textareaRef}
                              onFocus={handleFocus}
                              type="textarea"
                              marginTop="$md"
                              value={selections[option.id]?.text}
                              placeholder={t("form.text_placeholder")}
                              onChangeText={(textValue) => {
                                selections[option.id] = {
                                  optionId: option.id,
                                  text: textValue,
                                };
                                onChange(selections);
                              }}
                              maxLength={1024}
                              helper={t("form.max", {
                                value: 1024,
                              })}
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
                    label={`${question.code}. ${question.text[language]}`}
                    paragraph={question.helptext?.[language] || ""}
                    scale={question.scale}
                    onValueChange={onChange}
                    value={value}
                  />
                );
            }
          }}
        />
      </>
    );
  },
);

export default QuestionForm;
