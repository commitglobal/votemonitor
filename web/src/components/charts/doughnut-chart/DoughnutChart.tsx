import { ArcElement, CategoryScale, Chart, ChartData, ChartOptions, Legend, Tooltip } from 'chart.js';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { ChartJSOrUndefined } from 'node_modules/react-chartjs-2/dist/types';
import React, { forwardRef, useEffect, useRef, useState } from 'react';
import { Doughnut } from 'react-chartjs-2';

export interface DoughnutProps {
    title: string;
    data: ChartData<"doughnut">;
    aspectRatio?: number;
};

Chart.register(ArcElement, CategoryScale, ChartDataLabels, Tooltip, Legend);

const DoughnutChart = forwardRef<ChartJSOrUndefined<"doughnut">, DoughnutProps>((props, chartRef) => {
    const [aspectRatio, setAspectRatio] = useState(props.aspectRatio || (window.innerWidth <= 1800 ? 1 : 1.2));

    // @ts-ignore
    useEffect(() => {
        if (!props.aspectRatio) {
            const handleResize = () => {
                setAspectRatio(window.innerWidth <= 1800 ? 1 : 1.2);
            };
            window.addEventListener('resize', handleResize);

            // Cleanup function
            return () => {
                window.removeEventListener('resize', handleResize);
            };
        }
    }, [props.aspectRatio]);

    const options: ChartOptions<"doughnut"> = {
        responsive: true,
        maintainAspectRatio: true,
        aspectRatio: aspectRatio,
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
                },
                title: {
                    text: 'djdjdjdjjd',
                    color: '#FF0000'
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
                <div className="text-2xl font-bold">{props.data.datasets.reduce((t, d) => t + d.data.reduce((a, b) => a + b), 0)}</div>
                <span>{props.title}</span>
            </div>
            <div>
                <div>
                    <Doughnut data={props.data} options={options} ref={chartRef} plugins={[htmlLegendPlugin]} />
                </div>
            </div>
        </div>
    )
});

export default DoughnutChart;