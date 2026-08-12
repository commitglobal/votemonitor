import { format } from 'date-fns'
import type { ColumnDef } from '@tanstack/react-table'
import type { GuidesObserverModel } from '@/types/guides-observer'
import { DateTimeFormat } from '@/constants/formats'
import { DataTableColumnHeader } from '@/components/data-table/data-table-column-header'
import { GuideTypeIcon } from './GuideTypeIcon'
import { GuideRowActions } from './RowActions'

export function getGuidesObserversTableColumns(): ColumnDef<GuidesObserverModel>[] {
  return [
    {
      header: '',
      id: 'guideType',
      enableSorting: false,
      size: 16,
      minSize: 25,
      maxSize: 25,
      cell: ({ row }) => (
        <div className='w-[30px]'>
          <GuideTypeIcon guideType={row.original.guideType} />
        </div>
      ),
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Title' column={column} />
      ),
      accessorFn: (row) => row.title,
      id: 'title',
      enableSorting: true,
      cell: ({ row }) => <div className='truncate'>{row.original.title}</div>,
      meta: {
        label: 'Title',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Uploaded on' column={column} />
      ),
      accessorFn: (row) => row.createdOn,
      id: 'createdOn',
      enableSorting: true,
      size: 200,
      cell: ({ row }) => (
        <div className='truncate'>
          {format(row.original.createdOn, DateTimeFormat)}
        </div>
      ),
      meta: {
        label: 'Uploaded on',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Created by' column={column} />
      ),
      accessorFn: (row) => row.createdBy,
      id: 'createdBy',
      enableSorting: true,
      cell: ({ row }) => (
        <div className='truncate'>{row.original.createdBy}</div>
      ),
      meta: {
        label: 'Created by',
      },
    },
    {
      header: '',
      id: 'actions',
      enableSorting: false,
      size: 40,
      cell: ({ row }) => <GuideRowActions guide={row.original} />,
    },
  ]
}
