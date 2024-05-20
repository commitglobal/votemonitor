import TableTagList from '@/components/table-tag-list/TableTagList';
import { Badge } from '@/components/ui/badge';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { cn } from '@/lib/utils';
import { ChevronRightIcon } from '@heroicons/react/24/outline';
import { Link } from '@tanstack/react-router';
import { format } from 'date-fns';

import { MediaFilesCell } from '../components/MediaFilesCell/MediaFilesCell';

import type { ColumnDef } from '@tanstack/react-table';
import type { FormSubmissionByEntry, FormSubmissionByForm, FormSubmissionByObserver } from '../models/form-submission';
import type { QuickReport } from '../models/quick-report';
import type { QuestionExtraData } from '../types';

export const formSubmissionsByEntryColumnDefs: ColumnDef<FormSubmissionByEntry>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title='Time submitted' column={column} />,
    accessorKey: 'timeSubmitted',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{format(row.original.timeSubmitted, 'u-MM-dd KK:mm')}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Form code' column={column} />,
    accessorKey: 'formCode',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Form type' column={column} />,
    accessorKey: 'formType',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Station number' column={column} />,
    accessorKey: 'number',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L1' column={column} />,
    accessorKey: 'level1',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level1 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L2' column={column} />,
    accessorKey: 'level2',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level2 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L3' column={column} />,
    accessorKey: 'level3',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level3 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L4' column={column} />,
    accessorKey: 'level4',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level4 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L5' column={column} />,
    accessorKey: 'level5',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level5 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Observer' column={column} />,
    accessorKey: 'observerName',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.observerName}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Tags' column={column} />,
    accessorKey: 'tags',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({
      row: {
        original: { tags },
      },
    }) => <TableTagList tags={tags} />,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Responses' column={column} />,
    accessorKey: 'numberOfQuestionsAnswered',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Flagged answers' column={column} />,
    accessorKey: 'numberOfFlaggedAnswers',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Question notes' column={column} />,
    accessorKey: 'notesCount',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Medial files' column={column} />,
    accessorKey: 'mediaFilesCount',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Status' column={column} />,
    accessorKey: 'status',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => (
      <Badge
        className={cn({
          'text-slate-700 bg-slate-200': row.original?.needsFollowUp === undefined,
          'text-red-600 bg-red-200': row.original?.needsFollowUp === true,
          'text-yellow-600 bg-yellow-200': row.original?.needsFollowUp === false,
        })}>
        {row.original?.needsFollowUp === undefined
          ? 'N/A'
          : row.original?.needsFollowUp
          ? 'Needs followup'
          : 'Followed up'}
      </Badge>
    ),
  },
  {
    header: '',
    accessorKey: 'action',
    enableSorting: false,
    cell: ({ row }) => (
      <div className='text-right'>
        <Link
          className='hover:bg-purple-100 inline-flex h-6 w-6 rounded-full items-center justify-center'
          params={{ submissionId: row.original.submissionId }}
          to='/responses/$submissionId'>
          <ChevronRightIcon className='w-4 text-purple-600' />
        </Link>
      </div>
    ),
  },
];

export const formSubmissionsByObserverColumnDefs: ColumnDef<FormSubmissionByObserver>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title='Observer name' column={column} />,
    accessorKey: 'observerName',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.observerName}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Observer contact' column={column} />,
    accessorKey: 'phoneNumber',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Observer tags' column={column} />,
    accessorKey: 'tags',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({
      row: {
        original: { tags },
      },
    }) => <TableTagList tags={tags} />,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Locations' column={column} />,
    accessorKey: 'numberOfLocations',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Forms' column={column} />,
    accessorKey: 'numberOfFormsSubmitted',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Flagged answers' column={column} />,
    accessorKey: 'numberOfFlaggedAnswers',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Status' column={column} />,
    accessorKey: 'status',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) =>
      row.original?.needsFollowUp === true ? (
        <Badge
          className={cn({
            'text-red-600 bg-red-200': row.original?.needsFollowUp === true,
          })}>
          Needs followup
        </Badge>
      ) : (
        <span>-</span>
      ),
  },
  {
    header: '',
    accessorKey: 'action',
    enableSorting: false,
    cell: ({ row }) => (
      <div className='text-right'>
        <Link
          search
          className='hover:bg-purple-100 inline-flex h-6 w-6 rounded-full items-center justify-center'
          params={{ monitoringObserverId: row.original.monitoringObserverId}}
          to='/monitoring-observers/view/$monitoringObserverId'>
          <ChevronRightIcon className='w-4 text-purple-600' />
        </Link>
      </div>
    ),
  },
];

export const formSubmissionsByFormColumnDefs: ColumnDef<FormSubmissionByForm>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title='Form code' column={column} />,
    accessorKey: 'formCode',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Form type' column={column} />,
    accessorKey: 'formType',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Responses' column={column} />,
    accessorKey: 'numberOfSubmissions',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Flagged answers' column={column} />,
    accessorKey: 'numberOfFlaggedAnswers',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Question notes' column={column} />,
    accessorKey: 'numberOfNotes',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Media files' column={column} />,
    accessorKey: 'numberOfMediaFiles',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: '',
    accessorKey: 'action',
    enableSorting: false,
    cell: ({ row }) => (
      <div className='text-right'>
        <Link
          className='hover:bg-purple-100 inline-flex h-6 w-6 rounded-full items-center justify-center'
          params={{ formId: row.original.formId }}
          to='/responses/$formId/aggregated'>
          <ChevronRightIcon className='w-4 text-purple-600' />
        </Link>
      </div>
    ),
  },
];

export const questionExtraInfoColumnDefs: ColumnDef<QuestionExtraData>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title='Time submitted' column={column} />,
    accessorKey: 'timeSubmitted',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{format(row.original.timeSubmitted, 'u-MM-dd KK:mm')}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Note' column={column} />,
    accessorKey: 'text',
    enableSorting: true,
    enableGlobalFilter: true,
    minSize: 260,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Media files' column={column} />,
    accessorKey: 'attachments',
    enableSorting: false,
    enableGlobalFilter: false,
    cell: MediaFilesCell,
    size: 200,
  },
];

export const quickReportsColumnDefs: ColumnDef<QuickReport>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title='Entry ID' column={column} />,
    accessorKey: 'id',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Time submitted' column={column} />,
    accessorKey: 'timestamp',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Issue title' column={column} />,
    accessorKey: 'title',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Description' column={column} />,
    accessorKey: 'description',
    enableSorting: true,
    enableGlobalFilter: true,
    size: 200,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Medial files' column={column} />,
    accessorKey: 'numberOfAttachments',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Station number' column={column} />,
    accessorKey: 'number',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L1' column={column} />,
    accessorKey: 'level1',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level1 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L2' column={column} />,
    accessorKey: 'level2',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level2 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L3' column={column} />,
    accessorKey: 'level3',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level3 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L4' column={column} />,
    accessorKey: 'level4',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level4 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L5' column={column} />,
    accessorKey: 'level5',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level5 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Observer' column={column} />,
    accessorKey: 'observerName',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => (
      <div>
        {row.original.firstName} {row.original.lastName}
      </div>
    ),
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Observer contact' column={column} />,
    accessorKey: 'email',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: '',
    accessorKey: 'action',
    enableSorting: false,
    cell: ({ row }) => (
      <div className='text-right'>
        <Link
          className='hover:bg-purple-100 inline-flex h-6 w-6 rounded-full items-center justify-center'
          params={{ quickReportId: row.original.id }}
          to='/responses/quick-reports/$quickReportId'>
          <ChevronRightIcon className='w-4 text-purple-600' />
        </Link>
      </div>
    ),
  },
];
