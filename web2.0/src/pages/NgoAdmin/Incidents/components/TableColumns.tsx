'use client'

import * as React from 'react'
import type { ColumnDef } from '@tanstack/react-table'
import type { DataTableRowAction } from '@/types/data-table'
import type { MonitoringObserverModel } from '@/types/monitoring-observer'
import { Ellipsis } from 'lucide-react'
import { Button } from '@/components/ui/button'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import { DataTableColumnHeader } from '@/components/data-table/data-table-column-header'

interface GetTasksTableColumnsProps {
  setRowAction: React.Dispatch<
    React.SetStateAction<DataTableRowAction<MonitoringObserverModel> | null>
  >
}

export function getIncidentReportsTableColumns({
  setRowAction,
}: GetTasksTableColumnsProps): ColumnDef<MonitoringObserverModel>[] {
  return [
    {
      id: 'displayName',
      accessorKey: 'displayName',
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title='Name' />
      ),
      cell: ({ row }) => (
        <div className='w-20 truncate'>{row.original.displayName}</div>
      ),
      meta: {
        label: 'Name',
      },
      enableSorting: true,
      enableHiding: true,
    },
    {
      id: 'email',
      accessorKey: 'email',
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title='Email' />
      ),
      cell: ({ row }) => (
        <div className='w-20 truncate'>{row.original.email}</div>
      ),
      meta: {
        label: 'Email',
      },
      enableSorting: true,
      enableHiding: true,
    },

    {
      id: 'actions',
      cell: function Cell({ row }) {
        return (
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button
                aria-label='Open menu'
                variant='ghost'
                className='data-[state=open]:bg-muted flex size-8 p-0'
              >
                <Ellipsis className='size-4' aria-hidden='true' />
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align='end' className='w-40'>
              <DropdownMenuItem
                onSelect={() => setRowAction({ row, variant: 'update' })}
              >
                Edit
              </DropdownMenuItem>

              <DropdownMenuSeparator />
              <DropdownMenuItem
                onSelect={() => setRowAction({ row, variant: 'delete' })}
              >
                Delete
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        )
      },
      size: 40,
    },
  ]
}
