import type { ChartTypeRegistry, ScriptableContext } from 'chart.js';

export function getChartBackgroundColorGradient<TType extends keyof ChartTypeRegistry>(color: string) {
  return (context: ScriptableContext<TType>): CanvasGradient => {
    const ctx = context.chart.ctx;
    const gradient = ctx.createLinearGradient(0, 0, 0, ctx.canvas.height);

    gradient.addColorStop(0, color);
    gradient.addColorStop(1, 'rgba(122, 51, 179, 0.00)');

    return gradient;
  };
}
