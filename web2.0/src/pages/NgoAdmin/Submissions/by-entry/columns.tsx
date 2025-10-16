import { format } from 'date-fns'
import { Link } from '@tanstack/react-router'
import type { ColumnDef } from '@tanstack/react-table'
import { router } from '@/main'
import type { FormSubmissionModel } from '@/types/forms-submission'
import { ChevronRightIcon } from 'lucide-react'
import { DateTimeFormat } from '@/constants/formats'
import { Badge } from '@/components/ui/badge'
import {
  HoverCard,
  HoverCardContent,
  HoverCardTrigger,
} from '@/components/ui/hover-card'
import { DataTableColumnHeader } from '@/components/data-table-column-header'
import FormSubmissionFollowUpStatusBadge from '@/components/form-submission-follow-up-status-badge'

export const getFormSubmissionsColumns = (
  electionRoundId: string
): ColumnDef<FormSubmissionModel>[] => {
  return [
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Entry ID' column={column} />
      ),
      accessorFn: (row) => row.submissionId,
      id: 'submissionId',
      enableSorting: true,
      enableGlobalFilter: true,
      meta: {
        label: 'Entry ID',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Time submitted' column={column} />
      ),
      accessorFn: (row) => row.timeSubmitted,
      id: 'timeSubmitted',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => (
        <div>{format(row.original.timeSubmitted, DateTimeFormat)}</div>
      ),
      meta: {
        label: 'Time submitted',
      },
    },
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
        <DataTableColumnHeader title='Location - L1' column={column} />
      ),
      accessorFn: (row) => row.level1,
      id: 'level1',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => <div>{row.original.level1 ?? '-'}</div>,
      meta: {
        label: 'Location - L1',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Location - L2' column={column} />
      ),
      accessorFn: (row) => row.level2,
      id: 'level2',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => <div>{row.original.level2 ?? '-'}</div>,
      meta: {
        label: 'Location - L2',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Location - L3' column={column} />
      ),
      accessorFn: (row) => row.level3,
      id: 'level3',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => <div>{row.original.level3 ?? '-'}</div>,
      meta: {
        label: 'Location - L3',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Location - L4' column={column} />
      ),
      accessorFn: (row) => row.level4,
      id: 'level4',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => <div>{row.original.level4 ?? '-'}</div>,
      meta: {
        label: 'Location - L4',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Location - L5' column={column} />
      ),
      accessorFn: (row) => row.level5,
      id: 'level5',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => <div>{row.original.level5 ?? '-'}</div>,
      meta: {
        label: 'Location - L5',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Station number' column={column} />
      ),
      accessorFn: (row) => row.number,
      id: 'number',
      enableSorting: true,
      enableGlobalFilter: true,
      meta: {
        label: 'Station number',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Observer' column={column} />
      ),
      accessorFn: (row) => row.observerName,
      id: 'observerName',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({ row }) => <div>{row.original.observerName}</div>,
      meta: {
        label: 'Observer',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='NGO' column={column} />
      ),
      accessorFn: (row) => row.ngoName,
      id: 'ngoName',
      enableSorting: false,
      enableGlobalFilter: true,
      cell: ({ row }) => <div>{row.original.ngoName}</div>,
      meta: {
        label: 'NGO',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Observer tags' column={column} />
      ),
      accessorFn: (row) => row.tags,
      id: 'tags',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({
        row: {
          original: { tags },
        },
      }) =>
        tags.length ? (
          <HoverCard>
            <HoverCardTrigger>{tags.length} tags</HoverCardTrigger>
            <HoverCardContent className='w-80'>
              <div className='flex flex-wrap gap-2'>
                {tags.map((tag) => (
                  <Badge key={tag} variant='outline'>
                    {tag}
                  </Badge>
                ))}
              </div>
            </HoverCardContent>
          </HoverCard>
        ) : (
          '-'
        ),
      meta: {
        label: 'Observer tags',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Questions answered' column={column} />
      ),
      accessorFn: (row) => row.numberOfQuestionsAnswered,
      id: 'numberOfQuestionsAnswered',
      enableSorting: true,
      enableGlobalFilter: true,
      meta: {
        label: 'Questions answered',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Flagged answers' column={column} />
      ),
      accessorFn: (row) => row.numberOfFlaggedAnswers,
      id: 'numberOfFlaggedAnswers',
      enableSorting: true,
      enableGlobalFilter: true,
      meta: {
        label: 'Flagged answers',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Question notes' column={column} />
      ),
      accessorFn: (row) => row.notesCount,
      id: 'notesCount',
      enableSorting: true,
      enableGlobalFilter: true,
      meta: {
        label: 'Question notes',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Media files' column={column} />
      ),
      accessorFn: (row) => row.mediaFilesCount,
      id: 'mediaFilesCount',
      enableSorting: true,
      enableGlobalFilter: true,
      meta: {
        label: 'Media files',
      },
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title='Follow-up status' column={column} />
      ),
      accessorFn: (row) => row.followUpStatus,
      id: 'followUpStatus',
      enableSorting: false,
      enableGlobalFilter: true,
      cell: ({ row }) => (
        <FormSubmissionFollowUpStatusBadge
          followUpStatus={row.original.followUpStatus}
        />
      ),
      meta: {
        label: 'Follow-up status',
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
              submissionId: row.original.submissionId,
            }}
            to='/elections/$electionRoundId/submissions/$submissionId'
            search={{ from: router.state.location.search }}
          >
            <ChevronRightIcon className='h-4 w-4' />
          </Link>
        </div>
      ),
    },
  ]
}
