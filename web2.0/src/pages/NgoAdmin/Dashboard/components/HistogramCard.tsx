import { useMemo } from 'react'
import { format } from 'date-fns'
import type { HistogramEntry } from '@/types/statistics'
import { Area, AreaChart, CartesianGrid, XAxis, YAxis } from 'recharts'
import { DateTimeHourBucketFormat } from '@/constants/formats'
import {
  ChartContainer,
  ChartTooltip,
  ChartTooltipContent,
  type ChartConfig,
} from '@/components/ui/chart'
import { ChartCard, type ChartColumn } from './ChartCard'

/**
 * Whether the metric is something to celebrate or something to watch.
 *
 * Flagged answers and reports are problems, and the current dashboard already
 * paints them differently. The tone is never the only signal: the card title
 * names the metric, so a reader who cannot separate the hues loses nothing.
 */
type HistogramTone = 'neutral' | 'negative'

const chartConfigs: Record<HistogramTone, ChartConfig> = {
  neutral: { value: { label: 'Count', color: 'var(--chart-1)' } },
  negative: { value: { label: 'Count', color: 'var(--destructive)' } },
}

const columns: ChartColumn[] = [
  { key: 'bucket', label: 'Time' },
  { key: 'value', label: 'Count' },
]

type HistogramCardProps = {
  title: string
  histogram: HistogramEntry[] | undefined
  fileName: string
  tone?: HistogramTone
}

export function HistogramCard({
  title,
  histogram,
  fileName,
  tone = 'neutral',
}: HistogramCardProps) {
  const points = useMemo(() => {
    // The API returns buckets in UTC and in no particular order. Parsing to a
    // timestamp converts to the reader's own timezone, and sorting keeps the
    // line moving forward in time. The source array belongs to the query cache,
    // so it is copied before sorting rather than reordered in place.
    return (histogram ?? [])
      .map((entry) => ({
        bucket: new Date(entry.bucket).getTime(),
        value: entry.value,
      }))
      .sort((left, right) => left.bucket - right.bucket)
  }, [histogram])

  const total = useMemo(
    () => points.reduce((sum, point) => sum + point.value, 0),
    [points]
  )

  // The period the chart covers, printed once under the title instead of
  // labelling every point.
  const interval = useMemo(() => {
    if (points.length === 0) {
      return null
    }

    const first = format(points[0].bucket, DateTimeHourBucketFormat)
    const last = format(
      points[points.length - 1].bucket,
      DateTimeHourBucketFormat
    )

    return first === last ? first : `${first} — ${last}`
  }, [points])

  const rows = useMemo(
    () =>
      points.map((point) => ({
        bucket: format(point.bucket, DateTimeHourBucketFormat),
        value: point.value,
      })),
    [points]
  )

  const summary = (
    <>
      <p className='text-2xl font-semibold tracking-tight'>
        {total.toLocaleString()}
      </p>
      {interval ? (
        <p className='text-muted-foreground text-xs'>{interval}</p>
      ) : null}
    </>
  )

  return (
    <ChartCard
      title={title}
      summary={summary}
      columns={columns}
      rows={rows}
      fileName={fileName}
    >
      {points.length === 0 ? (
        <p className='text-muted-foreground py-8 text-center text-sm'>
          No data yet
        </p>
      ) : (
        // A single series needs no legend: the card title already says what is
        // plotted, and a one-swatch box would only repeat it.
        <ChartContainer
          config={chartConfigs[tone]}
          className='aspect-auto h-[160px] w-full'
        >
          <AreaChart data={points} margin={{ left: 12, right: 12 }}>
            <CartesianGrid vertical={false} />
            <XAxis
              dataKey='bucket'
              // A time scale rather than categories, so gaps between buckets
              // show up as gaps instead of being evenly spaced away.
              type='number'
              scale='time'
              domain={['dataMin', 'dataMax']}
              tickLine={false}
              axisLine={false}
              tickMargin={8}
              minTickGap={32}
              tickFormatter={(value: number) => format(value, 'MMM d, HH:mm')}
            />
            <YAxis
              tickLine={false}
              axisLine={false}
              width={36}
              allowDecimals={false}
              tickFormatter={(value: number) => value.toLocaleString()}
            />
            <ChartTooltip
              content={
                <ChartTooltipContent
                  indicator='line'
                  labelFormatter={(_, payload) =>
                    format(
                      Number(payload?.[0]?.payload?.bucket ?? 0),
                      DateTimeHourBucketFormat
                    )
                  }
                />
              }
            />
            <Area
              dataKey='value'
              type='monotone'
              stroke='var(--color-value)'
              strokeWidth={2}
              fill='var(--color-value)'
              // A wash rather than a saturated block, so the line stays the
              // loudest thing in the chart.
              fillOpacity={0.1}
            />
          </AreaChart>
        </ChartContainer>
      )}
    </ChartCard>
  )
}
