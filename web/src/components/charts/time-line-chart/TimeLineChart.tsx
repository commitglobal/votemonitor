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
  TimeScale,
  TimeSeriesScale,
} from 'chart.js';
import type { ChartJSOrUndefined } from 'node_modules/react-chartjs-2/dist/types';
import { forwardRef } from 'react';
import { Line } from 'react-chartjs-2';
import 'chartjs-adapter-date-fns';
import { enGB } from 'date-fns/locale';
import { DateTimeHourBucketFormat } from '@/common/formats';

ChartJS.register(
  CategoryScale,
  LinearScale,
  TimeScale,
  TimeSeriesScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  Filler
);

const defaultOptions: ChartOptions<'line'> = {
  maintainAspectRatio: false,
  devicePixelRatio: 1.5,
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
  scales: {
    y: {
      min: 0,
    },
    x: {
      type: 'timeseries',
      ticks: {
        source: 'auto',
        autoSkip: true,
        autoSkipPadding: 12,
      },
      time: {
        unit: 'hour',
        tooltipFormat: DateTimeHourBucketFormat,
        displayFormats: {
          hour: 'HH:00',
        },
      },
      bounds: 'data',
      adapters: {
        date: { locale: enGB },
      },
    },
  },
};

export interface TimeLineProps {
  title?: string;
  data: ChartData<'line', number[]>;
  total?: number;
}

const TimeLineChart = forwardRef<ChartJSOrUndefined<'line', number[]>, TimeLineProps>((props, chartRef) => {
  return (
    <div>
      {(props.title) && (
        <div>
          {(props.total!==undefined) && <div className='text-2xl font-bold'>{props.total ?? 0}</div>}
          {props.title && <span className='text-sm text-slate-500'>{props.title}</span>}
        </div>
      )}
      <div className='h-[200px]'>
        <Line data={props.data} options={defaultOptions} ref={chartRef} />
      </div>
    </div>
  );
});

export default TimeLineChart;
