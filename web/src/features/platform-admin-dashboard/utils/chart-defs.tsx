
import { ChartData, ChartOptions } from 'chart.js';

export const observersAccountsDataConfig: ChartData<"doughnut"> = {
  labels: ['Active', 'Pending', 'Suspended'],
  datasets: [
    {
      label: '# of observers',
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
      label: '# of observers',
      data: [124, 234 - 124],
      backgroundColor: [
        '#7833B3', // active color
        '#D3C1E5', // pending color
        '#DADADA', // inactive color
      ],
      borderWidth: 1,
    },
  ]
};
