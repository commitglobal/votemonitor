import {
    CategoryScale,
    ChartData,
    Chart as ChartJS,
    ChartOptions,
    Legend,
    LineElement,
    LinearScale,
    PointElement,
    Title,
    Tooltip,
    Filler
} from 'chart.js';
import { ChartJSOrUndefined } from 'node_modules/react-chartjs-2/dist/types';
import { forwardRef } from 'react';
import { Line } from 'react-chartjs-2';

ChartJS.register(
    CategoryScale,
    LinearScale,
    PointElement,
    LineElement,
    Title,
    Tooltip,
    Legend,
    Filler
);

export interface LineProps {
    title: string;
    data: ChartData<"line">;
};

const LineChart = forwardRef<ChartJSOrUndefined<"line">, LineProps>((props, chartRef) => {
    const options: ChartOptions<"line"> = {
        maintainAspectRatio: false,
        plugins: {
            legend: {
                display: false
            },
            tooltip: {
                enabled: true
            },
            datalabels: {
                display: false
            }
        },
        // elements:{
        //     point:{
        //         radius: 3
        //     }
        // }
    };

    return (
        <div>
            <div>
                <div className="text-2xl font-bold">
                    {
                        // @ts-ignore
                        props.data.datasets.reduce((t, d) => t + d.data.reduce((a, b) => a + b), 0)
                    }
                </div>
                <span className='text-sm text-slate-500'>{props.title}</span>
            </div>
            <div className='h-[200px]'>
                <Line
                    data={props.data}
                    options={options}
                    ref={chartRef} />
            </div>
        </div>
    )
});

export default LineChart;