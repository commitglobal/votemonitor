import type { ChartJSOrUndefined } from 'node_modules/react-chartjs-2/dist/types';
import { format } from 'date-fns';
import { forwardRef } from 'react';
import type { FunctionComponent } from '@/common/types';
import LineChart from '@/components/charts/line-chart/LineChart';
import { getChartBackgroundColorGradient } from '@/components/charts/utils/chart-options';
import type { DateQuestionAggregate } from '../../models/form-aggregated';
import { purple500 } from '../../utils/chart-colors';

type DateAggregateContentProps = {
  aggregate: DateQuestionAggregate;
};

const DateAggregateContent = forwardRef<ChartJSOrUndefined<'line', number[]>, DateAggregateContentProps>(
  ({ aggregate }, ref): FunctionComponent => {
    const dataset = aggregate.answers.reduce<Record<string, number>>((data, { value }) => {
      const time = format(new Date(value), 'u-MM-dd KK:00');
      return { ...data, [time]: (data?.[time] ?? 0) + 1 };
    }, {});

    return (
      <LineChart
        ref={ref}
        data={{
          labels: Object.keys(dataset),
          datasets: [
            {
              data: Object.values(dataset),
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
