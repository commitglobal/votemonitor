import type { ChartJSOrUndefined } from 'node_modules/react-chartjs-2/dist/types';
import type { MutableRefObject } from 'react';

export function saveChart(
  chartRef: MutableRefObject<ChartJSOrUndefined<'line', number[]> | null>,
  chartName: string
): void {
  const base64Image = chartRef.current?.toBase64Image().replace('data:image/png;base64,', '');

  if (base64Image) {
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
}
