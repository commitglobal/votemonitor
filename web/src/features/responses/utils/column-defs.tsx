import TableTagList from '@/components/table-tag-list/TableTagList';
import { Badge } from '@/components/ui/badge';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { cn } from '@/lib/utils';
import { ArrowTopRightOnSquareIcon, ChevronRightIcon } from '@heroicons/react/24/outline';
import { Link } from '@tanstack/react-router';
import { format } from 'date-fns';

import { MediaFilesCell } from '../components/MediaFilesCell/MediaFilesCell';

import { DateTimeFormat } from '@/common/formats';
import { FollowUpStatus } from '@/common/types';
import { Button } from '@/components/ui/button';
import type { RowData } from '@/components/ui/DataTable/DataTable';
import type { ColumnDef } from '@tanstack/react-table';
import { type CitizenReportByEntry } from '../models/citizen-report';
import { SubmissionType } from '../models/common';
import {
  type FormSubmissionByEntry,
  type FormSubmissionByForm,
  type FormSubmissionByObserver,
} from '../models/form-submission';
import { type QuickReport } from '../models/quick-report';
import type { QuestionExtraData } from '../types';
import { mapQuickReportLocationType } from './helpers';

export const formSubmissionsByEntryColumnDefs: ColumnDef<FormSubmissionByEntry & RowData>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title='Time submitted' column={column} />,
    accessorKey: 'timeSubmitted',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{format(row.original.timeSubmitted, DateTimeFormat)}</div>,
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
    header: ({ column }) => <DataTableColumnHeader title='Questions answered' column={column} />,
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
    header: ({ column }) => <DataTableColumnHeader title='Follow-up status' column={column} />,
    accessorKey: 'followUpStatus',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => (
      <Badge
        className={cn({
          'text-slate-700 bg-slate-200': row.original.followUpStatus === FollowUpStatus.NotApplicable,
          'text-red-600 bg-red-200': row.original.followUpStatus === FollowUpStatus.NeedsFollowUp,
          'text-green-600 bg-green-200': row.original.followUpStatus === FollowUpStatus.Resolved,
        })}>
        {row.original.followUpStatus === FollowUpStatus.NotApplicable
          ? 'Not Applicable'
          : row.original.followUpStatus === FollowUpStatus.NeedsFollowUp
            ? 'Needs follow-up'
            : 'Resolved'}
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

export const formSubmissionsForObserverColumnDefs: ColumnDef<FormSubmissionByEntry & RowData>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title='Time submitted' column={column} />,
    accessorKey: 'timeSubmitted',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{format(row.original.timeSubmitted, DateTimeFormat)}</div>,
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
    header: ({ column }) => <DataTableColumnHeader title='Questions answered' column={column} />,
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
    header: ({ column }) => <DataTableColumnHeader title='Follow-up status' column={column} />,
    accessorKey: 'followUpStatus',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => (
      <Badge
        className={cn({
          'text-slate-700 bg-slate-200': row.original.followUpStatus === FollowUpStatus.NotApplicable,
          'text-red-600 bg-red-200': row.original.followUpStatus === FollowUpStatus.NeedsFollowUp,
          'text-green-600 bg-green-200': row.original.followUpStatus === FollowUpStatus.Resolved,
        })}>
        {row.original.followUpStatus === FollowUpStatus.NotApplicable
          ? 'Not Applicable'
          : row.original.followUpStatus === FollowUpStatus.NeedsFollowUp
            ? 'Needs follow-up'
            : 'Resolved'}
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

export const formSubmissionsByObserverColumnDefs: ColumnDef<FormSubmissionByObserver & RowData>[] = [
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
    header: ({ column }) => <DataTableColumnHeader title='Follow-up status' column={column} />,
    accessorKey: 'followUpStatus',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) =>
      row.original.followUpStatus === FollowUpStatus.NeedsFollowUp ? (
        <Badge
          className={cn({
            'text-red-600 bg-red-200': row.original.followUpStatus === FollowUpStatus.NeedsFollowUp,
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
          className='hover:bg-purple-100 inline-flex h-6 w-6 rounded-full items-center justify-center'
          params={{ monitoringObserverId: row.original.monitoringObserverId, tab: 'details' }}
          to='/monitoring-observers/view/$monitoringObserverId/$tab'
          target='_blank'
          onClick={(e) => {
            e.stopPropagation();
          }}>
          <ChevronRightIcon className='w-4 text-purple-600' />
        </Link>
      </div>
    ),
  },
];

export const formSubmissionsByFormColumnDefs: ColumnDef<FormSubmissionByForm & RowData>[] = [
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

export const answerExtraInfoColumnDefs: ColumnDef<QuestionExtraData>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title='Type' column={column} />,
    accessorKey: 'type',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.type}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Time submitted' column={column} />,
    accessorKey: 'timeSubmitted',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{format(row.original.timeSubmitted, DateTimeFormat)}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Preview' column={column} />,
    accessorKey: 'preview',
    enableSorting: false,
    enableGlobalFilter: false,
    cell: ({ row }) => <div>{row.original.type === "Note" ? row.original.text : <MediaFilesCell attachment={row.original} />}</div>,
  },
];

export const aggregatedAnswerExtraInfoColumnDefs: ColumnDef<QuestionExtraData>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title='SubmissionID' column={column} />,
    accessorKey: 'submissionId',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>
      {
        row.original.submissionType === SubmissionType.FormSubmission &&
        <Link to='/responses/$submissionId' params={{ submissionId: row.original.submissionId }} preload='intent' target='_blank' >
          <Button type='button' variant={'link'} className='text-purple-500'>
            {row.original.submissionId.substring(0, 8)}
            <ArrowTopRightOnSquareIcon className='w-4' />
          </Button>
        </Link>
      }
      {
        row.original.submissionType === SubmissionType.CitizenReport &&
        <Link to='/responses/citizen-reports/$citizenReportId' params={{ citizenReportId: row.original.submissionId }} preload='intent' target='_blank' >
          <Button type='button' variant={'link'} className='text-purple-500'>
            {row.original.submissionId.substring(0, 8)}
            <ArrowTopRightOnSquareIcon className='w-4' />
          </Button>
        </Link>
      }
    </div>
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Type' column={column} />,
    accessorKey: 'type',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.type}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Time submitted' column={column} />,
    accessorKey: 'timeSubmitted',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{format(row.original.timeSubmitted, DateTimeFormat)}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Preview' column={column} />,
    accessorKey: 'preview',
    enableSorting: false,
    enableGlobalFilter: false,
    cell: ({ row }) => <div>{row.original.type === "Note" ? row.original.text : <MediaFilesCell attachment={row.original} />}</div>,
  },
];

export const quickReportsColumnDefs: ColumnDef<QuickReport>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title='Time submitted' column={column} />,
    accessorKey: 'timestamp',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{format(row.original.timestamp, DateTimeFormat)}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Observer' column={column} />,
    accessorKey: 'observerName',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => (
      <div>
        {row.original.firstName} {row.original.lastName}
      </div>
    ),
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location type' column={column} />,
    accessorKey: 'quickReportLocationType',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => (
      <div>
        {mapQuickReportLocationType(row.original.quickReportLocationType)}
      </div>
    ),
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Issue title' column={column} />,
    accessorKey: 'title',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.title.slice(0, 100) + '...'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Description' column={column} />,
    accessorKey: 'description',
    enableSorting: false,
    enableGlobalFilter: true,
    size: 200,
    cell: ({ row }) => <div>{row.original.description.slice(0, 100) + '...'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Medial files' column={column} />,
    accessorKey: 'numberOfAttachments',
    enableSorting: false,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L1' column={column} />,
    accessorKey: 'level1',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level1 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L2' column={column} />,
    accessorKey: 'level2',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level2 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L3' column={column} />,
    accessorKey: 'level3',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level3 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L4' column={column} />,
    accessorKey: 'level4',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level4 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L5' column={column} />,
    accessorKey: 'level5',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level5 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Station number' column={column} />,
    accessorKey: 'number',
    enableSorting: false,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Polling station details' column={column} />,
    accessorKey: 'pollingStationDetails',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.pollingStationDetails ?? '-'}</div>,
  },

  {
    header: ({ column }) => <DataTableColumnHeader title='Follow-up status' column={column} />,
    accessorKey: 'followUpStatus',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => (
      <Badge
        className={cn({
          'text-slate-700 bg-slate-200': row.original.followUpStatus === FollowUpStatus.NotApplicable,
          'text-red-600 bg-red-200': row.original.followUpStatus === FollowUpStatus.NeedsFollowUp,
          'text-green-600 bg-green-200': row.original.followUpStatus === FollowUpStatus.Resolved,
        })}>
        {row.original.followUpStatus === FollowUpStatus.NotApplicable
          ? 'Not Applicable'
          : row.original.followUpStatus === FollowUpStatus.NeedsFollowUp
            ? 'Needs follow-up'
            : 'Resolved'}
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
          params={{ quickReportId: row.original.id }}
          to='/responses/quick-reports/$quickReportId'>
          <ChevronRightIcon className='w-4 text-purple-600' />
        </Link>
      </div>
    ),
  },
];


export const citizenReportsByEntryColumnDefs: ColumnDef<CitizenReportByEntry & RowData>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title='Time submitted' column={column} />,
    accessorKey: 'timeSubmitted',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{format(row.original.timeSubmitted, DateTimeFormat)}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Form code' column={column} />,
    accessorKey: 'formCode',
    enableSorting: true,
    enableGlobalFilter: true,
  },

  {
    header: ({ column }) => <DataTableColumnHeader title='Questions answered' column={column} />,
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
    header: ({ column }) => <DataTableColumnHeader title='Follow-up status' column={column} />,
    accessorKey: 'followUpStatus',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => (
      <Badge
        className={cn({
          'text-slate-700 bg-slate-200': row.original.followUpStatus === FollowUpStatus.NotApplicable,
          'text-red-600 bg-red-200': row.original.followUpStatus === FollowUpStatus.NeedsFollowUp,
          'text-green-600 bg-green-200': row.original.followUpStatus === FollowUpStatus.Resolved,
        })}>
        {row.original.followUpStatus === FollowUpStatus.NotApplicable
          ? 'Not Applicable'
          : row.original.followUpStatus === FollowUpStatus.NeedsFollowUp
            ? 'Needs follow-up'
            : 'Resolved'}
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
          params={{ citizenReportId: row.original.id }}
          to='/responses/citizen-reports/$citizenReportId'>
          <ChevronRightIcon className='w-4 text-purple-600' />
        </Link>
      </div>
    ),
  },
];