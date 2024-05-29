import { ArcElement, CategoryScale, ChartData, Chart as ChartJS, ChartOptions, Legend, Tooltip } from 'chart.js';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { ChartJSOrUndefined } from 'node_modules/react-chartjs-2/dist/types';
import { forwardRef } from 'react';
import { Doughnut } from 'react-chartjs-2';

ChartJS.register(ArcElement, CategoryScale, ChartDataLabels, Tooltip, Legend);
export interface DoughnutProps {
    title: string;
    total: number;
    data: ChartData<"doughnut">;
};


const DoughnutChart = forwardRef<ChartJSOrUndefined<"doughnut">, DoughnutProps>((props, chartRef) => {
    const options: ChartOptions<"doughnut"> = {
        maintainAspectRatio: false,
        devicePixelRatio: 1.5,
        plugins: {
            datalabels: {
                color: '#FFFFFF',
                font: {
                    weight: 'bold'
                },
                anchor: 'center',
                align: 'center'
            },
            legend: {
                position: 'bottom' as const,
                align: 'end',
                onClick: () => { },
                labels: {
                    boxWidth: 20,
                    boxHeight: 20,
                    usePointStyle: true,
                    pointStyle: 'circle' as const,
                }
            },
            tooltip: {
                callbacks: {
                    label: function (context) {
                        const label = context.label || '';
                        const value = context.parsed;
                        return ` ${label}: ${value}`;
                    }
                }
            }
        },
        rotation: -90,
        circumference: 180,
    };

    return (
        <div>
            <div>
                <div className="text-2xl font-bold">{props.total}</div>
                <span className='text-sm text-slate-500'>{props.title}</span>
            </div>
            <div>
                <div>
                    <Doughnut data={props.data} options={options} ref={chartRef} />
                </div>
            </div>
        </div>
    )
});

export default DoughnutChart;