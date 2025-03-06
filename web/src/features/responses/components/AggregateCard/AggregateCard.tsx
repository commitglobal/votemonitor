import { saveChart } from '@/components/charts/utils/save-chart';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Square2StackIcon } from '@heroicons/react/24/solid';
import { useRef } from 'react';

import { Responder } from '../../models/form-submissions-aggregated';
import { DateAggregateContent } from '../DateAggregateContent/DateAggregateContent';
import { MultiSelectAggregateContent } from '../MultiSelectAggregateContent/MultiSelectAggregateContent';
import { NumberAggregateContent } from '../NumberAggregateContent/NumberAggregateContent';
import { RatingAggregateContent } from '../RatingAggregateContent/RatingAggregateContent';
import { ResponseExtraDataSection } from '../ReponseExtraDataSection/ResponseExtraDataSection';
import { SingleSelectAggregateContent } from '../SingleSelectAggregateContent/SingleSelectAggregateContent';
import { TextAggregateContent } from '../TextAggregateContent/TextAggregateContent';

import type { FunctionComponent } from '@/common/types';
import {
  Attachment,
  BaseQuestionAggregate,
  isDateAggregate,
  isMultiSelectAggregate,
  isNumberAggregate,
  isRatingAggregate,
  isSingleSelectAggregate,
  isTextAggregate,
  Note,
  SubmissionType,
} from '../../models/common';

type AggregateCardProps = {
  submissionType: SubmissionType;
  aggregate: BaseQuestionAggregate;
  language: string;
  responders?: Record<string, Responder>;
  attachments: Attachment[];
  notes: Note[];
};

export function AggregateCard({
  submissionType,
  aggregate,
  language,
  attachments,
  notes,
  responders,
}: AggregateCardProps): FunctionComponent {
  const questionNotes = notes.filter((note) => note.questionId === aggregate.questionId);
  const questionAttachments = attachments.filter((attachment) => attachment.questionId === aggregate.questionId);

  const chartRef = useRef(null);

  return (
    <Card key={aggregate.questionId}>
      <CardHeader>
        <CardTitle className='flex items-center justify-between text-xl gap-x-4'>
          <span>
            {aggregate.question.code}. {aggregate.question.text[language]}
          </span>
          {!isTextAggregate(aggregate) && !isNumberAggregate(aggregate) && (
            <Button
              className='gap-1'
              onClick={() => {
                saveChart(chartRef, '');
              }}
              variant='outline'>
              <Square2StackIcon className='w-4 h-4' /> Copy chart
            </Button>
          )}
        </CardTitle>
        <span className='text-sm italic'>{aggregate.question.helptext?.[language]}</span>
        <p className='text-gray-500'>{aggregate.answersAggregated} answers</p>
      </CardHeader>
      {aggregate.answersAggregated > 0 ? (
        <CardContent>
          {isDateAggregate(aggregate) && <DateAggregateContent ref={chartRef} aggregate={aggregate} />}

          {isMultiSelectAggregate(aggregate) && (
            <MultiSelectAggregateContent ref={chartRef} aggregate={aggregate} language={language} />
          )}

          {isNumberAggregate(aggregate) && <NumberAggregateContent aggregate={aggregate} />}

          {isRatingAggregate(aggregate) && (
            <RatingAggregateContent ref={chartRef} aggregate={aggregate} language={language} />
          )}

          {isSingleSelectAggregate(aggregate) && (
            <SingleSelectAggregateContent ref={chartRef} aggregate={aggregate} language={language} />
          )}

          {isTextAggregate(aggregate) && (
            <TextAggregateContent aggregate={aggregate} responders={responders} submissionType={submissionType} />
          )}

          {(notes.length > 0 || attachments.length > 0) && (
            <ResponseExtraDataSection
              attachments={questionAttachments}
              notes={questionNotes}
              aggregateDisplay={true}
              submissionType={submissionType}
            />
          )}
        </CardContent>
      ) : null}
    </Card>
  );
}
