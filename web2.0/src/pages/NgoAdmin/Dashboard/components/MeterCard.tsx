import {
  Label,
  PolarAngleAxis,
  PolarRadiusAxis,
  RadialBar,
  RadialBarChart,
} from 'recharts'
import { ChartContainer, type ChartConfig } from '@/components/ui/chart'
import { ChartCard, type ChartColumn } from './ChartCard'

const chartConfig = {
  value: {
    label: 'Value',
    color: 'var(--chart-1)',
  },
} satisfies ChartConfig

const columns: ChartColumn[] = [
  { key: 'metric', label: 'Metric' },
  { key: 'value', label: 'Value' },
]

type MeterCardProps = {
  title: string
  /** How much of the total has been reached. */
  value: number
  total: number
  /** Names what the number counts, printed under the ring. */
  caption: string
  /** Label of the counted thing in the data tab, such as "Visited". */
  reachedLabel: string
  remainingLabel: string
  fileName: string
}

/**
 * A single ratio measured against a limit.
 *
 * A meter rather than a two-slice pie: the reader is judging progress towards a
 * known total, not comparing two categories. The unfilled track comes from the
 * muted surface step, so the remaining share stays visible without competing
 * with the fill.
 */
export function MeterCard({
  title,
  value,
  total,
  caption,
  reachedLabel,
  remainingLabel,
  fileName,
}: MeterCardProps) {
  // A zero total would collapse the angle axis and paint a full ring for a
  // value of zero, which reads as "complete" when nothing has happened yet.
  const domainMax = Math.max(total, 1)
  const percentage = total > 0 ? Math.round((value / total) * 100) : 0

  const rows = [
    { metric: reachedLabel, value: value.toLocaleString() },
    {
      metric: remainingLabel,
      value: Math.max(total - value, 0).toLocaleString(),
    },
    { metric: 'Total', value: total.toLocaleString() },
    { metric: 'Share', value: `${percentage}%` },
  ]

  return (
    <ChartCard
      title={title}
      columns={columns}
      rows={rows}
      fileName={fileName}
      summary={
        <p className='text-muted-foreground text-xs'>
          {percentage}% {caption}
        </p>
      }
    >
      <ChartContainer
        config={chartConfig}
        className='mx-auto aspect-square max-h-[180px]'
      >
        <RadialBarChart
          data={[{ value, fill: 'var(--color-value)' }]}
          // Starts at twelve o'clock and runs clockwise, the direction a
          // progress ring is read in.
          startAngle={90}
          endAngle={-270}
          innerRadius={70}
          outerRadius={90}
        >
          <PolarAngleAxis
            // Without an explicit domain recharts scales the bar to the largest
            // value present, so a lone data point would always fill the ring.
            type='number'
            domain={[0, domainMax]}
            angleAxisId={0}
            tick={false}
          />
          <RadialBar
            dataKey='value'
            angleAxisId={0}
            background
            cornerRadius={4}
          />
          <PolarRadiusAxis tick={false} tickLine={false} axisLine={false}>
            <Label
              content={({ viewBox }) => {
                if (!viewBox || !('cx' in viewBox) || !('cy' in viewBox)) {
                  return null
                }

                return (
                  <text
                    x={viewBox.cx}
                    y={viewBox.cy}
                    textAnchor='middle'
                    dominantBaseline='middle'
                  >
                    <tspan
                      x={viewBox.cx}
                      y={viewBox.cy}
                      className='fill-foreground text-3xl font-semibold'
                    >
                      {value.toLocaleString()}
                    </tspan>
                    <tspan
                      x={viewBox.cx}
                      y={(viewBox.cy ?? 0) + 22}
                      className='fill-muted-foreground text-xs'
                    >
                      {`of ${total.toLocaleString()}`}
                    </tspan>
                  </text>
                )
              }}
            />
          </PolarRadiusAxis>
        </RadialBarChart>
      </ChartContainer>
    </ChartCard>
  )
}
