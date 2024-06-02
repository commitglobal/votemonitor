import { ArcElement, CategoryScale, ChartData, Chart as ChartJS, ChartOptions, Legend, Plugin, Tooltip } from 'chart.js';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { ChartJSOrUndefined } from 'node_modules/react-chartjs-2/dist/types';
import { forwardRef } from 'react';
import { Doughnut } from 'react-chartjs-2';

ChartJS.register(ArcElement, CategoryScale, ChartDataLabels, Tooltip, Legend);

export interface GaugeProps {
    title: string;
    metricLabel: string;
    data: ChartData<"doughnut">;
    value: number;
    total: number;
};

const GaugeChart = forwardRef<ChartJSOrUndefined<"doughnut">, GaugeProps>((props, chartRef) => {
    function formatAsPercentage(value: number, total: number): string {
        if (total) {
            const percentage = value / total * 100;
            return percentage.toFixed(2) + "%";
        }

        return '0 %';
    }

    const options: ChartOptions<"doughnut"> = {
        maintainAspectRatio: false,
        responsive: true,
        devicePixelRatio: 1.5,
        plugins: {
            datalabels: {
                color: '#DADADA',
                font: {
                    weight: 'bold'
                },
                anchor: 'center',
                align: 'center'
            },
            legend: {
                display: false,
            }
        },
        rotation: -90,
        circumference: 180,
    };

    const gaugeLabel = (value: number, total: number): Plugin<"doughnut"> => {
        return {
            id: 'chartLabel',
            beforeDatasetDraw: (chart, args, pluginOptions) => {
                const { ctx, data } = chart;

                ctx.save();
                const xCoor = chart.getDatasetMeta(0).data[0]?.x;
                const yCoor = chart.getDatasetMeta(0).data[0]?.y;
                ctx.font = 'bold 20px  ui-sans-serif, system-ui, sans-serif, "Apple Color Emoji", "Segoe UI Emoji", "Segoe UI Symbol", "Noto Color Emoji"';
                ctx.fillStyle = '#7833B3';
                ctx.textAlign = 'center';
                var percentage = formatAsPercentage(value, total)
                ctx.fillText(percentage, xCoor!, yCoor!);
            }
        }
    };

    return (
        <div>
            <div>
                <div className="text-2xl font-bold">{
                    props.value
                }
                </div>
                <span className='text-sm text-slate-500'>{props.title}</span>
            </div>
            <div>
               <Doughnut
                    data={props.data}
                    options={options}
                    ref={chartRef}
                    plugins={[gaugeLabel(props.value, props.total)]} 
                    redraw />
            </div>
            <div className='text-sm text-slate-500 text-center'>
                <span >{props.metricLabel}</span>
            </div>
        </div>
    )
});

export default GaugeChart;