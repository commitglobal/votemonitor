import type { FunctionComponent } from '@/common/types';
import LineChart from '@/components/charts/line-chart/LineChart';
import { getChartBackgroundColorGradient } from '@/components/charts/utils/chart-options';
import type { ChartJSOrUndefined } from 'node_modules/react-chartjs-2/dist/types';
import { forwardRef } from 'react';
import type { DateQuestionAggregate } from '../../models/form-aggregated';
import { purple500 } from '../../utils/chart-colors';
import { DateTimeHourBucketFormat } from '@/common/formats';
import { format } from 'date-fns';

type DateAggregateContentProps = {
  aggregate: DateQuestionAggregate;
};

const DateAggregateContent = forwardRef<ChartJSOrUndefined<'line', number[]>, DateAggregateContentProps>(
  ({ aggregate }, ref): FunctionComponent => {

    return (
      <LineChart
        ref={ref}
        data={{
          labels: Object.keys(aggregate.answersHistogram).sort().map(bucker=> format(new Date(bucker), DateTimeHourBucketFormat)),
          datasets: [
            {
              data: Object.keys(aggregate.answersHistogram).sort().map(key => aggregate.answersHistogram[key]!),
              borderColor: purple500,
              backgroundColor: getChartBackgroundColorGradient(purple500),
              fill: 'origin',
              borderWidth: 1,
              cubicInterpolationMode: 'monotone',
              tension: 0.4
            },
          ],
        }}
      />
    );
  }
);

export { DateAggregateContent };

