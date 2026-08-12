import { useMemo, useState } from 'react'
import { Download } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { ScrollArea } from '@/components/ui/scroll-area'
import { downloadCsv } from '../utils/download-csv'

export type LevelStatsEntry = {
  path: string
  value: number
  unit?: string
}

/** How many rows fit in the card before the rest moves behind "View all". */
const PREVIEW_SIZE = 5

const csvColumns = [
  { key: 'path', label: 'Location' },
  { key: 'value', label: 'Value' },
]

type LevelStatisticsCardProps = {
  title: string
  entries: LevelStatsEntry[]
  /** Base name of the exported file, without extension. */
  fileName: string
}

function EntryRow({ path, value, unit = '' }: LevelStatsEntry) {
  return (
    <div className='flex items-center justify-between gap-4 border-b py-2 last:border-b-0'>
      <span className='truncate text-sm'>{path}</span>
      <span className='shrink-0 text-sm font-medium tabular-nums'>
        {value.toLocaleString()}
        {unit}
      </span>
    </div>
  )
}

/**
 * One metric, broken down by location, for a single administrative level.
 *
 * Only the leading rows are shown: a level can hold hundreds of locations, and
 * a card that long would bury every card after it. The rest stay one click
 * away, and the export always carries the full list.
 */
export function LevelStatisticsCard({
  title,
  entries,
  fileName,
}: LevelStatisticsCardProps) {
  const [isDialogOpen, setIsDialogOpen] = useState(false)

  const sorted = useMemo(
    () =>
      // Copied before sorting: the array is derived from the query cache.
      [...entries].sort(
        (left, right) =>
          right.value - left.value || left.path.localeCompare(right.path)
      ),
    [entries]
  )

  const exportEntries = () => {
    downloadCsv(
      csvColumns,
      sorted.map((entry) => ({
        path: entry.path,
        value: `${entry.value}${entry.unit ?? ''}`,
      })),
      `${fileName}.csv`
    )
  }

  return (
    <Card>
      <CardHeader>
        <div className='flex items-start justify-between gap-2'>
          <CardTitle className='text-sm font-medium'>{title}</CardTitle>
          <Button
            type='button'
            variant='ghost'
            size='icon'
            aria-label={`Export ${title}`}
            onClick={exportEntries}
            disabled={sorted.length === 0}
          >
            <Download className='size-4' />
          </Button>
        </div>
      </CardHeader>
      <CardContent>
        {sorted.length === 0 ? (
          <p className='text-muted-foreground py-4 text-center text-sm'>
            No data yet
          </p>
        ) : (
          <>
            {sorted.slice(0, PREVIEW_SIZE).map((entry) => (
              <EntryRow key={entry.path} {...entry} />
            ))}

            {sorted.length > PREVIEW_SIZE ? (
              <Button
                variant='link'
                className='mt-2 h-auto p-0'
                onClick={() => setIsDialogOpen(true)}
              >
                View all {sorted.length}
              </Button>
            ) : null}
          </>
        )}
      </CardContent>

      <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
        <DialogContent className='sm:max-w-md'>
          <DialogHeader>
            <DialogTitle>{title}</DialogTitle>
            <DialogDescription>
              Every location, ordered from highest to lowest.
            </DialogDescription>
          </DialogHeader>
          <ScrollArea className='max-h-[60vh]'>
            {sorted.map((entry) => (
              <EntryRow key={entry.path} {...entry} />
            ))}
          </ScrollArea>
          <div className='flex justify-end'>
            <Button type='button' variant='outline' onClick={exportEntries}>
              <Download className='mr-2 size-4' />
              Export CSV
            </Button>
          </div>
        </DialogContent>
      </Dialog>
    </Card>
  )
}
