import { useId } from 'react'
import type { ObserversStats } from '@/types/statistics'
import { ChartCard, type ChartColumn } from './ChartCard'

/**
 * The three account states, in a fixed order.
 *
 * The order never changes with the data: a slot keeps its colour even when a
 * state drops to zero, so the reader is not re-learning the chart on every
 * refresh. `chart-3` renders as a desaturated slate, which is what "suspended"
 * should look like next to two saturated hues.
 */
const segments = [
  { key: 'active', label: 'Active', color: 'var(--chart-1)' },
  { key: 'pending', label: 'Pending', color: 'var(--chart-2)' },
  { key: 'suspended', label: 'Suspended', color: 'var(--chart-3)' },
] as const

const columns: ChartColumn[] = [
  { key: 'status', label: 'Status' },
  { key: 'count', label: 'Accounts' },
  { key: 'share', label: 'Share' },
]

/** Drawn in a fixed coordinate space and stretched to the card by the viewBox. */
const BAR_WIDTH = 300
const BAR_HEIGHT = 12
/** Surface-coloured gap between segments — what separates them, instead of a border. */
const SEGMENT_GAP = 2

type ObserverAccountsCardProps = {
  stats?: ObserversStats
}

/**
 * Part-to-whole breakdown of the observer accounts.
 *
 * A stacked proportion bar rather than a donut: shares are compared along one
 * axis instead of by angle, and the labels sit next to their segments instead
 * of in a separate legend. It is drawn as SVG so the same export path works
 * here as for the recharts figures.
 */
export function ObserverAccountsCard({ stats }: ObserverAccountsCardProps) {
  // Scoped so several of these cards on one page cannot share a clip path.
  const clipId = useId()

  const total = stats?.totalNumberOfObservers ?? 0

  const values = {
    active: stats?.activeObservers ?? 0,
    pending: stats?.pendingObservers ?? 0,
    suspended: stats?.suspendedObservers ?? 0,
  }

  const visible = segments.filter((segment) => values[segment.key] > 0)

  // Laid out left to right, each segment as wide as its share, with the gap
  // taken out of its own width so the bar still ends exactly at BAR_WIDTH.
  // Each x is derived from the segment before it, so nothing outside the fold
  // has to be reassigned while rendering.
  const drawn = visible.reduce<
    { key: string; color: string; x: number; width: number }[]
  >((placed, segment, index) => {
    const share = values[segment.key] / total
    const isLast = index === visible.length - 1
    const width = Math.max(share * BAR_WIDTH - (isLast ? 0 : SEGMENT_GAP), 1)
    const previous = placed[placed.length - 1]
    const x = previous ? previous.x + previous.width + SEGMENT_GAP : 0

    return [...placed, { key: segment.key, color: segment.color, x, width }]
  }, [])

  const rows = segments.map((segment) => ({
    status: segment.label,
    count: values[segment.key].toLocaleString(),
    share:
      total > 0 ? `${Math.round((values[segment.key] / total) * 100)}%` : '0%',
  }))

  return (
    <ChartCard
      title='Observer accounts'
      columns={columns}
      rows={rows}
      fileName='observer-accounts'
      summary={
        <>
          <p className='text-2xl font-semibold tracking-tight'>
            {total.toLocaleString()}
          </p>
          <p className='text-muted-foreground text-xs'>total accounts</p>
        </>
      }
    >
      {total === 0 ? (
        <p className='text-muted-foreground py-8 text-center text-sm'>
          No accounts yet
        </p>
      ) : (
        <>
          <svg
            viewBox={`0 0 ${BAR_WIDTH} ${BAR_HEIGHT}`}
            preserveAspectRatio='none'
            className='h-3 w-full'
            role='img'
            aria-label={segments
              .map((segment) => `${segment.label}: ${values[segment.key]}`)
              .join(', ')}
          >
            {/* Rounding the ends through a clip keeps the inner edges square,
                so the bar reads as one whole rather than three pills. */}
            <defs>
              <clipPath id={clipId}>
                <rect
                  width={BAR_WIDTH}
                  height={BAR_HEIGHT}
                  rx={BAR_HEIGHT / 2}
                />
              </clipPath>
            </defs>
            <g clipPath={`url(#${clipId})`}>
              {drawn.map((segment) => (
                <rect
                  key={segment.key}
                  x={segment.x}
                  y={0}
                  width={segment.width}
                  height={BAR_HEIGHT}
                  fill={segment.color}
                />
              ))}
            </g>
          </svg>

          {/* Identity never rests on colour alone: every segment is named here,
              with its swatch beside the text rather than colouring it. */}
          <ul className='mt-3 flex flex-wrap gap-x-4 gap-y-1'>
            {segments.map((segment) => (
              <li
                key={segment.key}
                className='flex items-center gap-1.5 text-sm'
              >
                <span
                  className='size-2 shrink-0 rounded-full'
                  style={{ backgroundColor: segment.color }}
                  aria-hidden
                />
                <span className='text-muted-foreground'>{segment.label}</span>
                <span className='font-medium'>
                  {values[segment.key].toLocaleString()}
                </span>
              </li>
            ))}
          </ul>
        </>
      )}
    </ChartCard>
  )
}
