import { saveChart } from '@/components/charts/utils/save-chart';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Square2StackIcon } from '@heroicons/react/24/solid';
import { useLoaderData } from '@tanstack/react-router';
import { useRef } from 'react';

import {
  BaseQuestionAggregate,
  isDateAggregate,
  isMultiSelectAggregate,
  isNumberAggregate,
  isRatingAggregate,
  isSingleSelectAggregate,
  isTextAggregate,
  Responder,
} from '../../models/form-aggregated';
import { DateAggregateContent } from '../DateAggregateContent/DateAggregateContent';
import { MultiSelectAggregateContent } from '../MultiSelectAggregateContent/MultiSelectAggregateContent';
import { NumberAggregateContent } from '../NumberAggregateContent/NumberAggregateContent';
import { QuestionExtraDataSection } from '../QuestionExtraDataSection/QuestionExtraDataSection';
import { RatingAggregateContent } from '../RatingAggregateContent/RatingAggregateContent';
import { SingleSelectAggregateContent } from '../SingleSelectAggregateContent/SingleSelectAggregateContent';
import { TextAggregateContent } from '../TextAggregateContent/TextAggregateContent';

import type { FunctionComponent } from '@/common/types';
type AggregateCardProps = {
  aggregate: BaseQuestionAggregate;
  language: string;
  responders: Record<string, Responder>;
};

export function AggregateCard({ aggregate, language, responders }: AggregateCardProps): FunctionComponent {
  const chartRef = useRef(null);

  const formSubmission = useLoaderData({ from: '/responses/$formId/aggregated' });

  const notes = formSubmission.notes.filter((note) => note.questionId === aggregate.questionId);
  const attachments = formSubmission.attachments.filter((attachment) => attachment.questionId === aggregate.questionId);

  return (
    <Card key={aggregate.questionId} className='max-w-4xl'>
      <CardHeader>
        <CardTitle className='text-xl flex justify-between items-center'>
          {aggregate.question.code}. {aggregate.question.text[language]}
          {!isTextAggregate(aggregate) && !isNumberAggregate(aggregate) && (
            <Button
              className='gap-1'
              onClick={() => {
                saveChart(chartRef, '');
              }}
              variant='outline'>
              <Square2StackIcon className='h-4 w-4' /> Copy chart
            </Button>
          )}
        </CardTitle>
        <p className='text-gray-500'>{aggregate.answersAggregated} answers</p>
      </CardHeader>

      <CardContent>
        {isDateAggregate(aggregate) && <DateAggregateContent ref={chartRef} aggregate={aggregate} />}

        {isMultiSelectAggregate(aggregate) && (
          <MultiSelectAggregateContent ref={chartRef} aggregate={aggregate} language={language} />
        )}

        {isNumberAggregate(aggregate) && <NumberAggregateContent ref={chartRef} aggregate={aggregate} />}

        {isRatingAggregate(aggregate) && <RatingAggregateContent ref={chartRef} aggregate={aggregate} />}

        {isSingleSelectAggregate(aggregate) && (
          <SingleSelectAggregateContent ref={chartRef} aggregate={aggregate} language={language} />
        )}

        {isTextAggregate(aggregate) && <TextAggregateContent aggregate={aggregate} responders={responders} />}

        {(notes.length > 0 || attachments.length > 0) && (
          <QuestionExtraDataSection attachments={attachments} notes={notes} />
        )}
      </CardContent>
    </Card>
  );
}
