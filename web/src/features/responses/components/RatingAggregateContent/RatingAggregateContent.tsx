import type { FunctionComponent } from '@/common/types';
import { RatingGroup } from '@/components/ui/ratings';
import type { RatingQuestionAggregate } from '../../models/form-aggregated';
import { ratingScaleToNumber } from '@/lib/utils';

type RatingAggregateContentProps = {
  aggregate: RatingQuestionAggregate;
};

export function RatingAggregateContent({ aggregate }: RatingAggregateContentProps): FunctionComponent {
  return (
    <>
      <div className='flex gap-6'>
        <div>Min: {aggregate.min}</div>
        <div>Max: {aggregate.max}</div>
      </div>
      <RatingGroup
        className='max-w-fit'
        scale={ratingScaleToNumber(aggregate.question.scale)}
        defaultValue={aggregate.average.toString()}
        disabled
      />
    </>
  );
}
