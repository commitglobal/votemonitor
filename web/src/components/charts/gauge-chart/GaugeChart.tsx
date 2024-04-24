import { ArcElement, CategoryScale, Chart as ChartJS, ChartData, ChartOptions, Legend, Plugin, Tooltip } from 'chart.js';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { ChartJSOrUndefined } from 'node_modules/react-chartjs-2/dist/types';
import { forwardRef, useEffect, useState } from 'react';
import { Doughnut } from 'react-chartjs-2';

ChartJS.register(ArcElement, CategoryScale, ChartDataLabels, Tooltip, Legend);

export interface GaugeProps {
    title: string;
    metricLabel: string;
    data: ChartData<"doughnut">;
};

const GaugeChart = forwardRef<ChartJSOrUndefined<"doughnut">, GaugeProps>((props, chartRef) => {
    function formatAsPercentage(value: number, total: number): string {
        const percentage = value / total * 100;
        return percentage.toFixed(2) + "%";
    }
    const options: ChartOptions<"doughnut"> = {
        maintainAspectRatio: false,
        responsive: true,
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

    const gaugeLabel: Plugin<"doughnut"> = {
        id: 'chartLabel',
        beforeDatasetDraw: (chart, args, pluginOptions) => {
            const { ctx, data } = chart;

            ctx.save();
            const xCoor = chart.getDatasetMeta(0).data[0]?.x;
            const yCoor = chart.getDatasetMeta(0).data[0]?.y;
            ctx.font = 'bold 20px  ui-sans-serif, system-ui, sans-serif, "Apple Color Emoji", "Segoe UI Emoji", "Segoe UI Symbol", "Noto Color Emoji"';
            ctx.fillStyle = '#7833B3';
            ctx.textAlign = 'center';
            var percentage = formatAsPercentage(data!.datasets[0]!.data[0]!, data.datasets.reduce((t, d) => t + d.data.reduce((a, b) => a + b), 0))
            ctx.fillText(percentage, xCoor!, yCoor!);
            ctx.font = '16px ui-sans-serif, system-ui, sans-serif, "Apple Color Emoji", "Segoe UI Emoji", "Segoe UI Symbol", "Noto Color Emoji"';
            ctx.fillStyle = '#AEAEAE';
            ctx.textAlign = 'center';
            ctx.fillText(props.metricLabel, xCoor!, yCoor! + 20);
        }
    };

    return (
        <div>
            <div>
                <div className="text-2xl font-bold">{
                    //@ts-ignore
                    props.data.datasets[0]!.data[0]
                }
                </div>
                <span className='text-sm text-slate-500'>{props.title}</span>
            </div>
            <div>
                <div>
                    <Doughnut
                        data={props.data}
                        options={options}
                        ref={chartRef}
                        plugins={[gaugeLabel]} />
                </div>
            </div>
        </div>
    )
});

export default GaugeChart;