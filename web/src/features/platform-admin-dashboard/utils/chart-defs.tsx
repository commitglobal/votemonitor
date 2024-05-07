
import type { ChartData, ScriptableContext } from 'chart.js';

export const observersAccountsDataConfig: ChartData<"doughnut"> = {
  labels: ['Active', 'Pending', 'Suspended'],
  datasets: [
    {
      data: [182, 33, 19],
      backgroundColor: [
        '#7833B3', // active color
        '#D3C1E5', // pending color
        '#DADADA', // suspended color
      ],
      borderWidth: 1,
    },
  ]
};

export const observersOnFieldDataConfig: ChartData<"doughnut"> = {
  labels: ['Active', 'Inactive'],
  datasets: [
    {
      data: [124, 234 - 124],
      backgroundColor: [
        '#7833B3', // active color
        '#DADADA', // inactive color
      ],
      borderWidth: 1,
    },
  ]
};

export const pollingStationsDataConfig: ChartData<"doughnut"> = {
  labels: ['Visited', 'Not visited'],
  datasets: [
    {
      data: [345, 1725 - 345],
      backgroundColor: [
        '#7833B3', // visited color
        '#DADADA', // not visited color
      ],
      borderWidth: 1,
    },
  ]
};

export const timeSpentObservingDataConfig: ChartData<"doughnut"> = {
  labels: ['Total time'],
  datasets: [
    {
      data: [13],
      backgroundColor: [
        '#7833B3', // total color
      ],
    },
  ]
};



// NOTE!! server returns dates in UTC extract local hour part
export const startedFormsDataConfig: ChartData<"line", number[]> = {
  labels: ['8:00', '9:00', '10:00', '11:00', '12:00', '13:00', '14:00', '15:00', '16:00', '17:00', '18:00', '19:00', '20:00'],
  datasets: [
    {
      data: [80, 120, 44, 22, 122, 113, 50, 269, 116, 17, 18, 19, 20],
      borderColor: '#7A33B3',
      backgroundColor: (context: ScriptableContext<'line'>): CanvasGradient => {
        const ctx = context.chart.ctx;
        const gradient = ctx.createLinearGradient(0, 0, 0, 180);

        gradient.addColorStop(0, "#7A33B3");
        gradient.addColorStop(1, "rgba(122, 51, 179, 0.00)");

        return gradient;
      },
      fill: 'origin',
      borderWidth: 1
    },
  ]
};

// NOTE!! server returns dates in UTC extract local hour part
export const questionsAnsweredDataConfig: ChartData<"line", number[]> = {
  labels: ['8:00', '9:00', '10:00', '11:00', '12:00', '13:00', '14:00', '15:00', '16:00', '17:00', '18:00', '19:00', '20:00'],
  datasets: [
    {
      data: [80, 120, 44, 22, 122, 113, 50, 269, 116, 17, 18, 19, 20],
      borderColor: '#7A33B3',
      backgroundColor: (context: ScriptableContext<'line'>): CanvasGradient => {
        const ctx = context.chart.ctx;
        const gradient = ctx.createLinearGradient(0, 0, 0, 180);

        gradient.addColorStop(0, "#7A33B3");
        gradient.addColorStop(1, "rgba(122, 51, 179, 0.00)");

        return gradient;
      },
      fill: 'origin',
      borderWidth: 1
    },
  ]
};
// NOTE!! server returns dates in UTC extract local hour part
export const flaggedAnswersDataConfig: ChartData<"line", number[]> = {
  labels: ['8:00', '9:00', '10:00', '11:00', '12:00', '13:00', '14:00', '15:00', '16:00', '17:00', '18:00', '19:00', '20:00'],
  datasets: [
    {
      data: [80, 120, 44, 22, 122, 113, 50, 269, 116, 17, 18, 19, 20],
      borderColor: '#EC6666',
      backgroundColor: (context: ScriptableContext<'line'>): CanvasGradient => {
        const ctx = context.chart.ctx;
        const gradient = ctx.createLinearGradient(0, 0, 0, 180);

        gradient.addColorStop(0, "#EC6666");
        gradient.addColorStop(1, "rgba(256, 256, 256, 0.00)");

        return gradient;
      },
      fill: 'origin',
      borderWidth: 1
    },
  ]
};

// NOTE!! server returns dates in UTC extract local hour part
export const quickReportsDataConfig: ChartData<"line", number[]> = {
  labels: ['8:00', '9:00', '10:00', '11:00', '12:00', '13:00', '14:00', '15:00', '16:00', '17:00', '18:00', '19:00', '20:00'],
  datasets: [
    {
      data: [80, 120, 44, 22, 122, 113, 50, 269, 116, 17, 18, 19, 20],
      borderColor: '#EC6666',
      backgroundColor: (context: ScriptableContext<'line'>): CanvasGradient => {
        const ctx = context.chart.ctx;
        const gradient = ctx.createLinearGradient(0, 0, 0, 180);

        gradient.addColorStop(0, "#EC6666");
        gradient.addColorStop(1, "rgba(256, 256, 256, 0.00)");

        return gradient;
      },
      fill: 'origin',
      borderWidth: 1
    },
  ]
};