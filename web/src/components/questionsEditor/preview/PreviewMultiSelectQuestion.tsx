import { MultiSelectAnswer, MultiSelectQuestion } from '@/common/types';
import { useMemo, useState } from 'react';

import {} from '@/common/types';
import { Checkbox, CheckboxField, CheckboxGroup } from '@/components/ui/checkbox';
import { Field, Fieldset, Label, Legend } from '@/components/ui/fieldset';
import { Text } from '@/components/ui/text';
import { Textarea } from '@/components/ui/textarea';
import { useParams } from '@tanstack/react-router';

export interface PreviewMultiSelectQuestionProps {
  languageCode: string;
  question: MultiSelectQuestion;
  answer: MultiSelectAnswer;
  setAnswer: (answer: MultiSelectAnswer) => void;
}

function PreviewMultiSelectQuestion({ languageCode, question, answer, setAnswer }: PreviewMultiSelectQuestionProps) {
  const [selectedOptionIds, setSelectedOptionIds] = useState<Set<string>>(new Set());
  const [freeTextSelected, setFreeTextSelected] = useState(false);

  const regularOptions = useMemo(() => {
    if (!question.options) {
      return [];
    }
    const regularOptions = question.options.filter((option) => !option.isFreeText);
    return regularOptions;
  }, [question.options]);

  // Currently we only support one free text option
  const freeTextOption = useMemo(() => question.options.find((option) => option.isFreeText), [question.options]);

  const handleOptionSelected = (optionId: string) => {
    // Since we cannot mutate the state value directly better to instantiate new state with the values of the state
    const optionIds = new Set(selectedOptionIds);

    if (optionIds.has(optionId)) {
      optionIds.delete(optionId);
    } else {
      optionIds.add(optionId);
    }
    setSelectedOptionIds(optionIds);
  };

  const params: any = useParams({
    strict: false,
  });

  return (
    <Fieldset>
      <Legend>
        {question.code} - {question.text[params['languageCode'] ? params['languageCode'] : languageCode]}
      </Legend>
      {!!question.helptext && (
        <Text>{question.helptext[params['languageCode'] ? params['languageCode'] : languageCode]}</Text>
      )}
      <CheckboxGroup>
        {regularOptions.map((option) => (
          <CheckboxField key={option.id}>
            <Checkbox name={option.id} onChange={() => handleOptionSelected(option.id)} />
            <Label>{option.text[params['languageCode'] ? params['languageCode'] : languageCode]}</Label>
          </CheckboxField>
        ))}

        {!!freeTextOption && (
          <CheckboxField>
            <Checkbox
              name={freeTextOption.id}
              onChange={(checked) => {
                handleOptionSelected(freeTextOption.id);
                setFreeTextSelected(checked);
              }}
            />
            <Label>{freeTextOption.text[params['languageCode'] ? params['languageCode'] : languageCode]}</Label>
          </CheckboxField>
        )}
      </CheckboxGroup>
      {freeTextSelected && (
        <Field>
          <Textarea />
        </Field>
      )}
    </Fieldset>
  );
}

export default PreviewMultiSelectQuestion;
