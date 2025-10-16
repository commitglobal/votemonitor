import { Link } from '@tanstack/react-router'
import type { ColumnDef } from '@tanstack/react-table'
import { router } from '@/main'
import { AggregatedFormSubmissionsTableRow } from '@/types/submissions-aggregate'
import { ChevronRightIcon } from 'lucide-react'
import { DataTableColumnHeader } from '@/components/data-table-column-header'

export const getAggregatedFormSubmissionsColumns = (
  electionRoundId: string
): ColumnDef<AggregatedFormSubmissionsTableRow>[] => {
  return [
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Form code' column={column} />
      ),
      accessorFn: (row) => row.formCode,
      id: 'formCode',
      enableSorting: true,
      enableGlobalFilter: true,
      meta: {
        label: 'Form code',
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
      meta: {
        label: 'Form type',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Form name' column={column} />
      ),
      accessorFn: (row) => row.formName,
      id: 'formName',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => (
        <div className='break-words'>
          {row.original.formName[row.original.defaultLanguage] ?? '-'}
        </div>
      ),
      meta: {
        label: 'Form name',
      },
    },

    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Number of submissions' column={column} />
      ),
      accessorFn: (row) => row.numberOfSubmissions,
      id: 'numberOfSubmissions',
      enableSorting: true,
      enableGlobalFilter: true,
      meta: {
        label: 'Number of submissions',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader
          title='Number of flagged answers'
          column={column}
        />
      ),
      accessorFn: (row) => row.numberOfFlaggedAnswers,
      id: 'numberOfFlaggedAnswers',
      enableSorting: true,
      enableGlobalFilter: true,
      meta: {
        label: 'Number of flagged answers',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Number of attachments' column={column} />
      ),
      accessorFn: (row) => row.numberOfMediaFiles,
      id: 'numberOfMediaFiles',
      enableSorting: true,
      enableGlobalFilter: true,
      meta: {
        label: 'Number of attachments',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Number of notes' column={column} />
      ),
      accessorFn: (row) => row.numberOfNotes,
      id: 'numberOfNotes',
      enableSorting: true,
      enableGlobalFilter: true,
      meta: {
        label: 'Number of notes',
      },
    },

    {
      header: '',
      id: 'actions',
      enableSorting: false,
      cell: ({ row }) => (
        <div className='text-right'>
          <Link
            className='inline-flex h-6 w-6 items-center justify-center rounded-full hover:bg-purple-100'
            params={{
              electionRoundId,
              formId: row.original.formId,
            }}
            to='/elections/$electionRoundId/submissions/by-form/$formId'
            search={{ ...router.state.location.search }}
          >
            <ChevronRightIcon className='h-4 w-4' />
          </Link>
        </div>
      ),
    },
  ]
}
