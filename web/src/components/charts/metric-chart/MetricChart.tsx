import { ArcElement, CategoryScale, Chart, ChartData, ChartOptions, Legend, Plugin, Tooltip } from 'chart.js';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { ChartJSOrUndefined } from 'node_modules/react-chartjs-2/dist/types';
import { forwardRef } from 'react';
import { Doughnut } from 'react-chartjs-2';

export interface MetricChartProps {
  title: string;
  data: ChartData<'doughnut'>;
  unit?: string;
}

Chart.register(ArcElement, CategoryScale, ChartDataLabels, Tooltip, Legend);

const MetricChart = forwardRef<ChartJSOrUndefined<'doughnut'>, MetricChartProps>((props, chartRef) => {
  const options: ChartOptions<'doughnut'> = {
    maintainAspectRatio: false,
    devicePixelRatio: 1.5,
    plugins: {
      datalabels: {
        display: false,
      },
      legend: {
        display: false,
      },
      tooltip: {
        enabled: false,
      },
    },
    elements: {
      arc: {
        borderWidth: 0,
      },
    },
    cutout: '70%',
  };

  const metricLabel: Plugin<'doughnut'> = {
    id: 'chartLabel',
    beforeDatasetDraw: (chart) => {
      const { ctx, data } = chart;
      const unit = props.unit ?? '';
      ctx.save();
      const xCoor = chart.getDatasetMeta(0).data[0]?.x;
      const yCoor = chart.getDatasetMeta(0).data[0]?.y;
      ctx.font =
        'bold 20px  ui-sans-serif, system-ui, sans-serif, "Apple Color Emoji", "Segoe UI Emoji", "Segoe UI Symbol", "Noto Color Emoji"';
      ctx.textAlign = 'center';
      ctx.fillText(data.datasets[0]!.data[0]! + '' + unit, xCoor!, yCoor! + 10);
    },
  };

  return (
    <div>
      <div>
        <div className='text-2xl font-bold'>
          {props.data.datasets.reduce((t, d) => t + d.data.reduce((a, b) => a + b), 0) + '' + (props.unit ?? '')}
        </div>
        <span className='text-sm text-slate-500'>{props.title}</span>
      </div>
      <div>
        <div>
          <Doughnut data={props.data} options={options} ref={chartRef} plugins={[metricLabel]} />
        </div>
      </div>
    </div>
  );
});

export default MetricChart;
