import { format } from 'date-fns'
import type { ColumnDef } from '@tanstack/react-table'
import { FormModel } from '@/types/form'
import { mapFormType, mapLanguageNameByCode } from '@/lib/i18n'
import { DateTimeFormat } from '@/constants/formats'
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from '@/components/ui/tooltip'
import FormStatusBadge from '@/components/badges/from-status-badge'
import { DataTableColumnHeader } from '@/components/data-table/data-table-column-header'
import { FormRowActions } from './RowActions'

export function getFormsTableColumns(): ColumnDef<FormModel>[] {
  return [
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Form code' column={column} />
      ),
      accessorFn: (row) => row.code,
      id: 'code',
      enableSorting: true,
      enableGlobalFilter: true,
      meta: {
        label: 'Form code',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Form name' column={column} />
      ),
      id: 'name',
      cell: ({ row }) => (
        <div>{row.original.name[row.original.defaultLanguage] ?? '-'}</div>
      ),

      enableSorting: true,
      enableGlobalFilter: true,

      meta: {
        label: 'Form name',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Form type' column={column} />
      ),
      accessorFn: (row) => row.formType,
      id: 'formType',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => <div>{mapFormType(row.original.formType)}</div>,

      meta: {
        label: 'Form type',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Language' column={column} />
      ),
      accessorFn: (row) => row.defaultLanguage,
      id: 'defaultLanguage',
      enableSorting: false,
      enableGlobalFilter: true,
      cell: ({ row }) => (
        <div>{mapLanguageNameByCode(row.original.defaultLanguage)}</div>
      ),

      meta: {
        label: 'Language',
      },
    },

    {
      header: ({ column }) => (
        <DataTableColumnHeader title='# of questions' column={column} />
      ),
      accessorFn: (row) => row.numberOfQuestions,
      id: 'numberOfQuestions',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => <div>{row.original.numberOfQuestions}</div>,

      meta: {
        label: '# of questions',
      },
    },

    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Status' column={column} />
      ),
      accessorFn: (row) => row.status,
      id: 'status',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => <FormStatusBadge formStatus={row.original.status} />,

      meta: {
        label: 'Issue title',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Last updated on' column={column} />
      ),
      accessorFn: (row) => row.lastModifiedOn,
      id: 'lastModifiedOn',
      enableSorting: true,
      enableGlobalFilter: true,
      size: 200,
      cell: ({ row }) => (
        <Tooltip>
          <TooltipTrigger asChild>
            <div
              className='cursor-pointer hover:underline'
              title={row.original.lastModifiedBy}
            >
              {format(row.original.lastModifiedOn, DateTimeFormat)}
            </div>
          </TooltipTrigger>
          <TooltipContent className='max-w-md break-words whitespace-pre-wrap'>
            {row.original.lastModifiedBy}
          </TooltipContent>
        </Tooltip>
      ),

      meta: {
        label: 'Last updated on',
      },
    },

    {
      header: '',
      id: 'actions',
      enableSorting: false,
      cell: ({ row }) => <FormRowActions form={row.original} />,
    },
  ]
}
