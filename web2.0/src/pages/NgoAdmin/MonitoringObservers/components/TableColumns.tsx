import { format } from 'date-fns'
import type { ColumnDef } from '@tanstack/react-table'
import type { MonitoringObserverModel } from '@/types/monitoring-observer'
import { DateTimeFormat } from '@/constants/formats'
import { Badge } from '@/components/ui/badge'
import MonitoringObserverStatusBadge from '@/components/badges/monitoring-observer-status-badge'
import { DataTableColumnHeader } from '@/components/data-table/data-table-column-header'
import { MonitoringObserverRowActions } from './RowActions'

/** Beyond this, the remaining tags collapse into a "+N" badge. */
const VISIBLE_TAGS = 3

export function getMonitoringObserversTableColumns(): ColumnDef<MonitoringObserverModel>[] {
  return [
    {
      id: 'displayName',
      accessorKey: 'displayName',
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title='Name' />
      ),
      cell: ({ row }) => (
        <div className='truncate'>{row.original.displayName}</div>
      ),
      meta: { label: 'Name' },
      enableSorting: true,
      enableHiding: true,
    },
    {
      id: 'email',
      accessorKey: 'email',
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title='Email' />
      ),
      cell: ({ row }) => <div className='truncate'>{row.original.email}</div>,
      meta: { label: 'Email' },
      enableSorting: true,
      enableHiding: true,
    },
    {
      id: 'tags',
      accessorKey: 'tags',
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title='Observer tags' />
      ),
      // Tags are an array, so there is no meaningful order to sort them by.
      enableSorting: false,
      enableHiding: true,
      cell: ({ row }) => {
        const tags = row.original.tags ?? []

        if (tags.length === 0) {
          return <span className='text-muted-foreground'>-</span>
        }

        const hidden = tags.length - VISIBLE_TAGS

        return (
          <div className='flex flex-wrap items-center gap-1'>
            {tags.slice(0, VISIBLE_TAGS).map((tag) => (
              <Badge key={tag} variant='secondary' className='font-normal'>
                {tag}
              </Badge>
            ))}
            {hidden > 0 ? (
              // The full list would push every other column off screen; the
              // count keeps the row honest about what is not shown.
              <Badge variant='outline' title={tags.join(', ')}>
                +{hidden}
              </Badge>
            ) : null}
          </div>
        )
      },
      meta: { label: 'Observer tags' },
    },
    {
      id: 'phoneNumber',
      accessorKey: 'phoneNumber',
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title='Phone' />
      ),
      cell: ({ row }) => (
        <div className='truncate'>
          {row.original.phoneNumber || (
            <span className='text-muted-foreground'>-</span>
          )}
        </div>
      ),
      meta: { label: 'Phone' },
      enableSorting: true,
      enableHiding: true,
    },
    {
      id: 'status',
      accessorKey: 'status',
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title='Observer status' />
      ),
      cell: ({ row }) => (
        <MonitoringObserverStatusBadge status={row.original.status} />
      ),
      meta: { label: 'Observer status' },
      enableSorting: true,
      enableHiding: true,
    },
    {
      id: 'latestActivityAt',
      accessorKey: 'latestActivityAt',
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title='Latest activity at' />
      ),
      size: 180,
      cell: ({ row }) => {
        const latestActivityAt = row.original.latestActivityAt

        // Observers who never opened the mobile app have no activity at all.
        return latestActivityAt ? (
          <div className='truncate'>
            {format(latestActivityAt, DateTimeFormat)}
          </div>
        ) : (
          <span className='text-muted-foreground'>Never</span>
        )
      },
      meta: { label: 'Latest activity at' },
      enableSorting: true,
      enableHiding: true,
    },
    {
      header: '',
      id: 'actions',
      enableSorting: false,
      size: 40,
      cell: ({ row }) => (
        <MonitoringObserverRowActions observer={row.original} />
      ),
    },
  ]
}
