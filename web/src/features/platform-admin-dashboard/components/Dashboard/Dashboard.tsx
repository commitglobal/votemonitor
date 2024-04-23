import Layout from '@/components/layout/Layout';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { MutableRefObject, ReactElement, useRef } from 'react';
import { Chart as ChartJS, ArcElement, Tooltip, Legend } from 'chart.js';
import { Doughnut } from 'react-chartjs-2';
import { observersAccountsDataConfig ,observersOnFieldDataConfig } from '../../utils/chart-defs';
import DoughnutChart from '@/components/charts/doughnut-chart/DoughnutChart';
import { ArrowDownTrayIcon } from '@heroicons/react/24/solid';
import { Button } from '@/components/ui/button';
import { saveAs } from 'file-saver';


ChartJS.register(ArcElement, Tooltip, Legend);

export default function PlatformAdminDashboard(): ReactElement {
  const observersAccountsChartRef = useRef(null);
  const observersOnFieldChartRef = useRef(null);

  function saveChart(chartRef: any, chartName: string): void {
    const base64Image = chartRef.current.toBase64Image().replace('data:image/png;base64,', '');
    console.log(base64Image);

    const binaryString = atob(base64Image);
    const arrayBuffer = new ArrayBuffer(binaryString.length);
    const uintArray = new Uint8Array(arrayBuffer);

    for (let i = 0; i < binaryString.length; i++) {
      uintArray[i] = binaryString.charCodeAt(i);
    }

    const blob = new Blob([uintArray], { type: 'image/png' });
    const url = window.URL.createObjectURL(blob);

    const a = document.createElement('a');
    a.style.display = 'none';
    a.href = url;
    a.download = chartName;

    document.body.appendChild(a);
    a.click();

    window.URL.revokeObjectURL(url);
  }

  return (
    <Layout title='Dashboard' subtitle='Key indicators.'>
      <div className="flex-col md:flex">
        <div className="flex-1 space-y-4">
          <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
            <Card>
              <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle className="text-sm font-medium">
                  Observers accounts
                </CardTitle>
                <Button type='button' variant='ghost' onClick={() => saveChart(observersAccountsChartRef, 'observers-accounts.png')}>
                  <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                </Button>
              </CardHeader>
              <CardContent>
                <DoughnutChart title='total accounts' data={observersAccountsDataConfig} ref={observersAccountsChartRef} />
              </CardContent>
            </Card>
            <Card>
              <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle className="text-sm font-medium">
                  Observers on field
                </CardTitle>
                <Button type='button' variant='ghost' onClick={() => saveChart(observersOnFieldChartRef, 'observers-on-field.png')}>
                  <ArrowDownTrayIcon className='w-6 h-6 fill-gray-400' />
                </Button>
              </CardHeader>
              <CardContent>
                <DoughnutChart title='Observers' data={observersOnFieldDataConfig} ref={observersOnFieldChartRef} />
              </CardContent>
            </Card>
            <Card>
              <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle className="text-sm font-medium">Polling stations</CardTitle>
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth="2"
                  className="h-4 w-4 text-muted-foreground"
                >
                  <rect width="20" height="14" x="2" y="5" rx="2" />
                  <path d="M2 10h20" />
                </svg>
              </CardHeader>
              <CardContent>
                <div className="text-2xl font-bold">+12,234</div>
                <p className="text-xs text-muted-foreground">
                  +19% from last month
                </p>
              </CardContent>
            </Card>
            <Card>
              <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                <CardTitle className="text-sm font-medium">
                  Time spent observing
                </CardTitle>
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth="2"
                  className="h-4 w-4 text-muted-foreground"
                >
                  <path d="M22 12h-4l-3 9L9 3l-3 9H2" />
                </svg>
              </CardHeader>
              <CardContent>
                <div className="text-2xl font-bold">+573</div>
                <p className="text-xs text-muted-foreground">
                  +201 since last hour
                </p>
              </CardContent>
            </Card>
          </div>
          <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-7">
            <Card className="col-span-4">
              <CardHeader>
                <CardTitle>Overview</CardTitle>
              </CardHeader>
              <CardContent className="pl-2">
              </CardContent>
            </Card>
            <Card className="col-span-3">
              <CardHeader>
                <CardTitle>Recent Sales</CardTitle>
                <CardDescription>
                  You made 265 sales this month.
                </CardDescription>
              </CardHeader>
              <CardContent>
              </CardContent>
            </Card>
          </div>
        </div>
      </div>
    </Layout>
  )
}