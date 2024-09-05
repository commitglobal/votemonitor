import { saveChart } from '@/components/charts/utils/save-chart';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Square2StackIcon } from '@heroicons/react/24/solid';
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
import { RatingAggregateContent } from '../RatingAggregateContent/RatingAggregateContent';
import { ResponseExtraDataSection } from '../ReponseExtraDataSection/ResponseExtraDataSection';
import { SingleSelectAggregateContent } from '../SingleSelectAggregateContent/SingleSelectAggregateContent';
import { TextAggregateContent } from '../TextAggregateContent/TextAggregateContent';

import type { FunctionComponent } from '@/common/types';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { formAggregatedDetailsQueryOptions, Route } from '@/routes/responses/$formId.aggregated';
import { useSuspenseQuery } from '@tanstack/react-query';
type AggregateCardProps = {
  aggregate: BaseQuestionAggregate;
  language: string;
  responders: Record<string, Responder>;
};

export function AggregateCard({ aggregate, language, responders }: AggregateCardProps): FunctionComponent {
  const { formId } = Route.useParams()
  const currentElectionRoundId = useCurrentElectionRoundStore(s => s.currentElectionRoundId);
  const { data: formSubmission } = useSuspenseQuery(formAggregatedDetailsQueryOptions(currentElectionRoundId, formId));

  const notes = formSubmission.notes.filter((note) => note.questionId === aggregate.questionId);
  const attachments = formSubmission.attachments.filter((attachment) => attachment.questionId === aggregate.questionId);

  const chartRef = useRef(null);

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

        {isNumberAggregate(aggregate) && <NumberAggregateContent aggregate={aggregate} />}

        {isRatingAggregate(aggregate) && <RatingAggregateContent ref={chartRef} aggregate={aggregate} />}

        {isSingleSelectAggregate(aggregate) && (
          <SingleSelectAggregateContent ref={chartRef} aggregate={aggregate} language={language} />
        )}

        {isTextAggregate(aggregate) && <TextAggregateContent aggregate={aggregate} responders={responders} />}

        {(notes.length > 0 || attachments.length > 0) && (
          <ResponseExtraDataSection attachments={attachments} notes={notes} aggregateDisplay={true} />
        )}
      </CardContent>
    </Card>
  );
}
