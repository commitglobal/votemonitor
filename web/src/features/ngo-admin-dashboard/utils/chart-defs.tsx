
import { round } from '@/lib/utils';
import type { ChartData, ScriptableContext } from 'chart.js';
import { format } from 'date-fns';

export const observersAccountsDataConfig = (stats?: ObserversStats): ChartData<"doughnut"> => {
  const labels = [];
  const data = [];
  const colors = [];

  if (stats?.activeObservers) {
    labels.push('Active');
    data.push(stats.activeObservers);
    // active color
    colors.push('#7833B3');
  }

  if (stats?.pendingObservers) {
    labels.push('Pending');
    data.push(stats.pendingObservers);
    // pending color
    colors.push('#D3C1E5');
  }

  if (stats?.suspendedObservers) {
    labels.push('Suspended');
    data.push(stats.suspendedObservers);
    // suspended color
    colors.push('#DADADA');
  }

  return {
    labels: labels,
    datasets: [
      {
        data: data,
        backgroundColor: colors,
        borderWidth: 1,
      },
    ]
  }
};

export const observersOnTheFieldDataConfig = (totalNumberOfObservers?: number, numberOfObserversOnTheField?: number): ChartData<"doughnut"> => {
  const labels = [];
  const data = [];
  const colors = [];

  if (numberOfObserversOnTheField) {
    labels.push('Active');
    data.push(numberOfObserversOnTheField);
    // active color
    colors.push('#7833B3');
  }

  labels.push('Inactive');
  data.push((totalNumberOfObservers ?? 0) - (numberOfObserversOnTheField ?? 0));
  // inactive color
  colors.push('#DADADA');

  return {
    labels: labels,
    datasets: [
      {
        data: data,
        backgroundColor: colors,
        borderWidth: 1,
      },
    ]
  }
};

export const pollingStationsDataConfig = (pollingStationsStats?: PollingStationsStats): ChartData<"doughnut"> => {
  const labels = [];
  const data = [];
  const colors = [];

  if (pollingStationsStats?.numberOfVisitedPollingStations) {
    labels.push('Visited');
    data.push();
    // active color
    colors.push('#7833B3');
  }

  labels.push('Not visited');
  data.push((pollingStationsStats?.totalNumberOfPollingStations ?? 0) - (pollingStationsStats?.numberOfVisitedPollingStations ?? 0));
  // inactive color
  colors.push('#DADADA');

  return {
    labels: labels,
    datasets: [
      {
        data: data,
        backgroundColor: colors,
        borderWidth: 1,
      },
    ]
  };
}

export const timeSpentObservingDataConfig = (totalMinutesObserving?: number): ChartData<"doughnut"> => {
  const totalHours = totalMinutesObserving ? round(totalMinutesObserving / 60, 2) : 0;

  return {
    labels: ['Total time'],
    datasets: [
      {
        data: [totalHours],
        backgroundColor: [
          '#7833B3', // total color
        ],
      },
    ]
  };
}

// NOTE!! server returns dates in UTC extract local hour part
export const histogramChartConfig = (histogram: HistogramEntry[] | undefined, variant: 'red' | 'blue' = 'blue'): ChartData<"line", number[]> => {
  const labels: string[] = [];
  const data: number[] = [];

  histogram
    ?.sort((a, b) => new Date(a.bucket).getTime() - new Date(b.bucket).getTime())
    ?.forEach(b => {
      labels.push(format(new Date(b.bucket), 'kk:00'));
      data.push(b.value);
    });
  return {
    labels: labels,
    datasets: [
      {
        data: data,
        borderColor: variant === 'blue' ? '#7A33B3' : '#EC6666',
        backgroundColor: (context: ScriptableContext<'line'>): CanvasGradient => {
          const ctx = context.chart.ctx;
          const gradient = ctx.createLinearGradient(0, 0, 0, 180);

          if (variant === 'blue') {
            gradient.addColorStop(0, "#7A33B3");
            gradient.addColorStop(1, "rgba(122, 51, 179, 0.00)");
          }

          if (variant === 'red') {
            gradient.addColorStop(0, "#EC6666");
            gradient.addColorStop(1, "rgba(256, 256, 256, 0.00)");
          }

          return gradient;
        },
        fill: 'origin',
        borderWidth: 1
      },
    ]
  };
}
