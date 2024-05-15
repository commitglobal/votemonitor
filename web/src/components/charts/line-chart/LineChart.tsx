import {
  CategoryScale,
  type ChartData,
  Chart as ChartJS,
  type ChartOptions,
  Legend,
  LineElement,
  LinearScale,
  PointElement,
  Title,
  Tooltip,
  Filler,
} from 'chart.js';
import type { ChartJSOrUndefined } from 'node_modules/react-chartjs-2/dist/types';
import { forwardRef } from 'react';
import { Line } from 'react-chartjs-2';

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend, Filler);

const defaultOptions: ChartOptions<'line'> = {
  maintainAspectRatio: false,
  plugins: {
    legend: {
      display: false,
    },
    tooltip: {
      enabled: true,
    },
    datalabels: {
      display: false,
    },
  },
};

export interface LineProps {
  title?: string;
  data: ChartData<'line', number[]>;
  showTotal?: boolean;
}

const LineChart = forwardRef<ChartJSOrUndefined<'line', number[]>, LineProps>((props, chartRef) => {
  return (
    <div>
      {(props.showTotal || props.title) && (
        <div>
          {props.showTotal && (
            <div className='text-2xl font-bold'>
              {props.data.datasets.reduce((t, d) => t + d.data.reduce((a, b) => a + b), 0)}
            </div>
          )}
          {props.title && <span className='text-sm text-slate-500'>{props.title}</span>}
        </div>
      )}
      <div className='h-[200px]'>
        <Line data={props.data} options={defaultOptions} ref={chartRef} />
      </div>
    </div>
  );
});

export default LineChart;
