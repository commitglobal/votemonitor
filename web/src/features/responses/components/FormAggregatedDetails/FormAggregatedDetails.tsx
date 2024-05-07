import { Square2StackIcon } from '@heroicons/react/24/solid';
import { useLoaderData } from '@tanstack/react-router';
import { useRef } from 'react';
import type { FunctionComponent } from '@/common/types';
import { saveChart } from '@/components/charts/utils/save-chart';
import Layout from '@/components/layout/Layout';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import {
  isDateAggregate,
  isMultiSelectAggregate,
  isNumberAggregate,
  isRatingAggregate,
  isSingleSelectAggregate,
  isTextAggregate,
} from '../../models/form-aggregated';
import { DateAggregateContent } from '../DateAggregateContent/DateAggregateContent';
import { MultiSelectAggregateContent } from '../MultiSelectAggregateContent/MultiSelectAggregateContent';
import { NumberAggregateContent } from '../NumberAggregateContent/NumberAggregateContent';
import { RatingAggregateContent } from '../RatingAggregateContent/RatingAggregateContent';
import { SingleSelectAggregateContent } from '../SingleSelectAggregateContent/SingleSelectAggregateContent';
import { TextAggregateContent } from '../TextAggregateContent/TextAggregateContent';

export default function FormAggregatedDetails(): FunctionComponent {
  const formSubmission = useLoaderData({ from: '/responses/$formId/aggregated' });
  const { submissionsAggregate } = formSubmission;
  const { defaultLanguage, formCode, formType, aggregates } = submissionsAggregate;

  const chartRef = useRef(null);

  return (
    <Layout title={`${formCode} - ${formType.name}`}>
      <div className='flex flex-col gap-10'>
        {Object.values(aggregates).map((aggregate) => {
          return (
            <Card key={aggregate.questionId} className='max-w-4xl'>
              <CardHeader>
                <CardTitle className='text-xl flex justify-between items-center'>
                  {aggregate.question.code}. {aggregate.question.text[defaultLanguage]}
                  {!isTextAggregate(aggregate) && !isRatingAggregate(aggregate) && (
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
                  <MultiSelectAggregateContent ref={chartRef} aggregate={aggregate} language={defaultLanguage} />
                )}

                {isNumberAggregate(aggregate) && <NumberAggregateContent ref={chartRef} aggregate={aggregate} />}

                {isRatingAggregate(aggregate) && <RatingAggregateContent aggregate={aggregate} />}

                {isSingleSelectAggregate(aggregate) && (
                  <SingleSelectAggregateContent ref={chartRef} aggregate={aggregate} language={defaultLanguage} />
                )}

                {isTextAggregate(aggregate) && <TextAggregateContent aggregate={aggregate} />}
              </CardContent>
            </Card>
          );
        })}
      </div>
    </Layout>
  );
}
