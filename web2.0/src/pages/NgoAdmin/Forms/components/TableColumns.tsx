import { format } from 'date-fns'
import type { ColumnDef } from '@tanstack/react-table'
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
import type { FormRowWithSubrows } from './Table'

export function getFormsTableColumns(): ColumnDef<FormRowWithSubrows>[] {
  return [
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Form code' column={column} />
      ),
      accessorFn: (row) => row.code,
      id: 'code',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => {
        const form = row.original
        // Hide code for subrows
        return form.isSubrow ? <div>-</div> : <div>{form.code}</div>
      },
      meta: {
        label: 'Form code',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Form name' column={column} />
      ),
      id: 'name',
      cell: ({ row }) => {
        const form = row.original
        const isSubrow = form.isSubrow
        const language = isSubrow ? form.subrowLanguage : form.defaultLanguage
        
        return (
          <div>
            {form.name[language!] ?? '-'}
          </div>
        )
      },

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
      cell: ({ row }) => {
        const form = row.original
        // Hide form type for subrows
        return form.isSubrow ? <div>-</div> : <div>{mapFormType(form.formType)}</div>
      },

      meta: {
        label: 'Form type',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Language' column={column} />
      ),
      accessorFn: (row) => {
        const form = row as FormRowWithSubrows
        return form.isSubrow ? form.subrowLanguage : form.defaultLanguage
      },
      id: 'defaultLanguage',
      enableSorting: false,
      enableGlobalFilter: true,
      cell: ({ row }) => {
        const form = row.original
        const isSubrow = form.isSubrow
        const language = isSubrow ? form.subrowLanguage : form.defaultLanguage
        
        return (
          <div>{language ? mapLanguageNameByCode(language) : '-'}</div>
        )
      },

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
      cell: ({ row }) => {
        const form = row.original
        // Hide number of questions for subrows
        return form.isSubrow ? <div>-</div> : <div>{form.numberOfQuestions}</div>
      },

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
      cell: ({ row }) => {
        const form = row.original
        // Hide status for subrows
        return form.isSubrow ? <div>-</div> : <FormStatusBadge formStatus={form.status} />
      },

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
      cell: ({ row }) => {
        const form = row.original
        // Hide last updated for subrows
        if (form.isSubrow) {
          return <div>-</div>
        }
        return (
          <Tooltip>
            <TooltipTrigger asChild>
              <div
                className='cursor-pointer hover:underline'
                title={form.lastModifiedBy}
              >
                {format(form.lastModifiedOn, DateTimeFormat)}
              </div>
            </TooltipTrigger>
            <TooltipContent className='max-w-md break-words whitespace-pre-wrap'>
              {form.lastModifiedBy}
            </TooltipContent>
          </Tooltip>
        )
      },

      meta: {
        label: 'Last updated on',
      },
    },

    {
      header: '',
      id: 'actions',
      enableSorting: false,
      cell: ({ row }) => {
        const form = row.original
        // Hide actions for subrows
        return form.isSubrow ? <div></div> : <FormRowActions form={form} />
      },
    },
  ]
}
