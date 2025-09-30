import { DateTimeFormat } from '@/common/formats';
import {
  CitizenReportFollowUpStatus,
  FormSubmissionFollowUpStatus,
  IncidentReportFollowUpStatus,
  QuickReportFollowUpStatus,
} from '@/common/types';
import TableTagList from '@/components/table-tag-list/TableTagList';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import type { RowData } from '@/components/ui/DataTable/DataTable';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { cn } from '@/lib/utils';
import { ArrowTopRightOnSquareIcon, ChevronRightIcon } from '@heroicons/react/24/outline';
import { Link } from '@tanstack/react-router';
import type { ColumnDef } from '@tanstack/react-table';
import { format } from 'date-fns';
import { ArrowDown, ArrowUp, ArrowUpDown } from 'lucide-react';
import MediaFilesCell from '../components/MediaFilesCell/MediaFilesCell';
import { CitizenReportsAggregatedByForm, type CitizenReportByEntry } from '../models/citizen-report';
import { SubmissionType } from '../models/common';
import {
  type FormSubmissionByEntry,
  type FormSubmissionByForm,
  type FormSubmissionByObserver,
} from '../models/form-submission';
import { IncidentReportByEntry, IncidentReportByForm, IncidentReportByObserver } from '../models/incident-report';
import { type QuickReport } from '../models/quick-report';
import type { QuestionExtraData } from '../types';
import { mapIncidentCategory, mapIncidentReportLocationType, mapQuickReportLocationType } from './helpers';

export const formSubmissionsByEntryColumnDefs: ColumnDef<FormSubmissionByEntry & RowData>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title='Entry ID' column={column} />,
    accessorFn: (row) => row.submissionId,
    id: 'submissionId',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Time submitted' column={column} />,
    accessorFn: (row) => row.timeSubmitted,
    id: 'timeSubmitted',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{format(row.original.timeSubmitted, DateTimeFormat)}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Form code' column={column} />,
    accessorFn: (row) => row.formCode,
    id: 'formCode',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Form type' column={column} />,
    accessorFn: (row) => row.formType,
    id: 'formType',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Form name' column={column} />,
    accessorFn: (row) => row.formName,
    id: 'formName',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div className='break-words'>{row.original.formName[row.original.defaultLanguage] ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L1' column={column} />,
    accessorFn: (row) => row.level1,
    id: 'level1',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level1 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L2' column={column} />,
    accessorFn: (row) => row.level2,
    id: 'level2',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level2 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L3' column={column} />,
    accessorFn: (row) => row.level3,
    id: 'level3',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level3 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L4' column={column} />,
    accessorFn: (row) => row.level4,
    id: 'level4',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level4 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L5' column={column} />,
    accessorFn: (row) => row.level5,
    id: 'level5',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level5 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Station number' column={column} />,
    accessorFn: (row) => row.number,
    id: 'number',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Observer' column={column} />,
    accessorFn: (row) => row.observerName,
    id: 'observerName',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.observerName}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='NGO' column={column} />,
    accessorFn: (row) => row.ngoName,
    id: 'ngoName',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.ngoName}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Observer tags' column={column} />,
    accessorFn: (row) => row.tags,
    id: 'tags',
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
    accessorFn: (row) => row.numberOfQuestionsAnswered,
    id: 'numberOfQuestionsAnswered',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Flagged answers' column={column} />,
    accessorFn: (row) => row.numberOfFlaggedAnswers,
    id: 'numberOfFlaggedAnswers',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Question notes' column={column} />,
    accessorFn: (row) => row.notesCount,
    id: 'notesCount',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Media files' column={column} />,
    accessorFn: (row) => row.mediaFilesCount,
    id: 'mediaFilesCount',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Follow-up status' column={column} />,
    accessorFn: (row) => row.followUpStatus,
    id: 'followUpStatus',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => (
      <Badge
        className={cn({
          'text-slate-700 bg-slate-200': row.original.followUpStatus === FormSubmissionFollowUpStatus.NotApplicable,
          'text-red-600 bg-red-200': row.original.followUpStatus === FormSubmissionFollowUpStatus.NeedsFollowUp,
          'text-green-600 bg-green-200': row.original.followUpStatus === FormSubmissionFollowUpStatus.Resolved,
        })}>
        {row.original.followUpStatus === FormSubmissionFollowUpStatus.NotApplicable
          ? 'Not Applicable'
          : row.original.followUpStatus === FormSubmissionFollowUpStatus.NeedsFollowUp
            ? 'Needs follow-up'
            : 'Resolved'}
      </Badge>
    ),
  },
  {
    header: '',
    id: 'action',
    enableSorting: false,
    cell: ({ row }) => (
      <div className='text-right'>
        <Link
          className='inline-flex items-center justify-center w-6 h-6 rounded-full hover:bg-purple-100'
          params={{ submissionId: row.original.submissionId }}
          to='/responses/form-submissions/$submissionId'>
          <ChevronRightIcon className='w-4 text-purple-600' />
        </Link>
      </div>
    ),
  },
];

export const observerFormSubmissionsColumnDefs: ColumnDef<FormSubmissionByEntry & RowData>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title='Time submitted' column={column} />,
    accessorFn: (row) => row.timeSubmitted,
    id: 'timeSubmitted',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{format(row.original.timeSubmitted, DateTimeFormat)}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Form code' column={column} />,
    accessorFn: (row) => row.formCode,
    id: 'formCode',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Form type' column={column} />,
    accessorFn: (row) => row.formType,
    id: 'formType',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Form name' column={column} />,
    accessorFn: (row) => row.formName,
    id: 'formName',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div className='break-words'>{row.original.formName[row.original.defaultLanguage] ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L1' column={column} />,
    accessorFn: (row) => row.level1,
    id: 'level1',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level1 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L2' column={column} />,
    accessorFn: (row) => row.level2,
    id: 'level2',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level2 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L3' column={column} />,
    accessorFn: (row) => row.level3,
    id: 'level3',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level3 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L4' column={column} />,
    accessorFn: (row) => row.level4,
    id: 'level4',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level4 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L5' column={column} />,
    accessorFn: (row) => row.level5,
    id: 'level5',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level5 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Station number' column={column} />,
    accessorFn: (row) => row.number,
    id: 'number',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Questions answered' column={column} />,
    accessorFn: (row) => row.numberOfQuestionsAnswered,
    id: 'numberOfQuestionsAnswered',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Flagged answers' column={column} />,
    accessorFn: (row) => row.numberOfFlaggedAnswers,
    id: 'numberOfFlaggedAnswers',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Question notes' column={column} />,
    accessorFn: (row) => row.notesCount,
    id: 'notesCount',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Media files' column={column} />,
    accessorFn: (row) => row.mediaFilesCount,
    id: 'mediaFilesCount',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Follow-up status' column={column} />,
    accessorFn: (row) => row.followUpStatus,
    id: 'followUpStatus',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => (
      <Badge
        className={cn({
          'text-slate-700 bg-slate-200': row.original.followUpStatus === FormSubmissionFollowUpStatus.NotApplicable,
          'text-red-600 bg-red-200': row.original.followUpStatus === FormSubmissionFollowUpStatus.NeedsFollowUp,
          'text-green-600 bg-green-200': row.original.followUpStatus === FormSubmissionFollowUpStatus.Resolved,
        })}>
        {row.original.followUpStatus === FormSubmissionFollowUpStatus.NotApplicable
          ? 'Not Applicable'
          : row.original.followUpStatus === FormSubmissionFollowUpStatus.NeedsFollowUp
            ? 'Needs follow-up'
            : 'Resolved'}
      </Badge>
    ),
  },
  {
    header: '',
    id: 'action',
    enableSorting: false,
    cell: ({ row }) => (
      <div className='text-right'>
        <Link
          className='inline-flex items-center justify-center w-6 h-6 rounded-full hover:bg-purple-100'
          params={{ submissionId: row.original.submissionId }}
          to='/responses/form-submissions/$submissionId'>
          <ChevronRightIcon className='w-4 text-purple-600' />
        </Link>
      </div>
    ),
  },
];

export const formSubmissionsByObserverColumnDefs: ColumnDef<FormSubmissionByObserver & RowData>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title='Observer name' column={column} />,
    accessorFn: (row) => row.observerName,
    id: 'observerName',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.observerName}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Observer contact' column={column} />,
    accessorFn: (row) => row.phoneNumber,
    id: 'phoneNumber',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='NGO' column={column} />,
    accessorFn: (row) => row.ngoName,
    id: 'ngoName',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.ngoName}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Observer tags' column={column} />,
    accessorFn: (row) => row.tags,
    id: 'tags',
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
    accessorFn: (row) => row.numberOfLocations,
    id: 'numberOfLocations',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Forms' column={column} />,
    accessorFn: (row) => row.numberOfFormsSubmitted,
    id: 'numberOfFormsSubmitted',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Flagged answers' column={column} />,
    accessorFn: (row) => row.numberOfFlaggedAnswers,
    id: 'numberOfFlaggedAnswers',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Follow-up status' column={column} />,
    accessorFn: (row) => row.followUpStatus,
    id: 'followUpStatus',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) =>
      row.original.followUpStatus === FormSubmissionFollowUpStatus.NeedsFollowUp ? (
        <Badge
          className={cn({
            'text-red-600 bg-red-200': row.original.followUpStatus === FormSubmissionFollowUpStatus.NeedsFollowUp,
          })}>
          Needs followup
        </Badge>
      ) : (
        <span>-</span>
      ),
  },
  {
    header: '',
    id: 'action',
    enableSorting: false,
    cell: ({ row }) => (
      <div className='text-right'>
        <Link
          className='inline-flex items-center justify-center w-6 h-6 rounded-full hover:bg-purple-100'
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
    accessorFn: (row) => row.formCode,
    id: 'formCode',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Form type' column={column} />,
    accessorFn: (row) => row.formType,
    id: 'formType',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Form name' column={column} />,
    accessorFn: (row) => row.formName,
    id: 'formName',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div className='break-words'>{row.original.formName[row.original.defaultLanguage] ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Responses' column={column} />,
    accessorFn: (row) => row.numberOfSubmissions,
    id: 'numberOfSubmissions',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Flagged answers' column={column} />,
    accessorFn: (row) => row.numberOfFlaggedAnswers,
    id: 'numberOfFlaggedAnswers',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Question notes' column={column} />,
    accessorFn: (row) => row.numberOfNotes,
    id: 'numberOfNotes',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Media files' column={column} />,
    accessorFn: (row) => row.numberOfMediaFiles,
    id: 'numberOfMediaFiles',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: '',
    id: 'action',
    enableSorting: false,
    cell: ({ row }) => (
      <div className='text-right'>
        <Link
          className='inline-flex items-center justify-center w-6 h-6 rounded-full hover:bg-purple-100'
          params={{ formId: row.original.formId }}
          to='/responses/form-submissions/$formId/aggregated'>
          <ChevronRightIcon className='w-4 text-purple-600' />
        </Link>
      </div>
    ),
  },
];

export const answerExtraInfoColumnDefs: ColumnDef<QuestionExtraData>[] = [
  {
    accessorKey: 'type',
    header: () => <div className='w-[90px]'>Type</div>,
    cell: ({ row }) => <div className='w-[80px]'>{row.getValue('type')}</div>,
    size: 80,
    enableResizing: false,
  },
  {
    accessorKey: 'timeSubmitted',
    header: ({ column }) => {
      const isSorted = column.getIsSorted();

      return (
        <Button variant='ghost' onClick={() => column.toggleSorting(column.getIsSorted() === 'asc')}>
          Time submitted
          {isSorted === 'asc' ? (
            <ArrowUp className='w-4 h-4' />
          ) : isSorted === 'desc' ? (
            <ArrowDown className='w-4 h-4' />
          ) : (
            <ArrowUpDown className='w-4 h-4' />
          )}
        </Button>
      );
    },
    cell: ({ row }) => {
      const formatted = format(row.original.timeSubmitted, DateTimeFormat);

      return <div className=''>{formatted}</div>;
    },
    size: 80,
    enableResizing: false,
  },

  {
    id: 'preview',
    enableSorting: false,
    enableGlobalFilter: false,
    cell: ({ row }) => (
      <div className='w-full'>
        {row.original.type === 'Note' ? row.original.text : <MediaFilesCell attachment={row.original} />}
      </div>
    ),
    size: 300,
  },
];

export const aggregatedAnswerExtraInfoColumnDefs: ColumnDef<QuestionExtraData>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title='SubmissionID' column={column} className='w-[100px]' />,
    accessorFn: (row) => row.submissionId,
    id: 'submissionId',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => (
      <div className='w-[100px]'>
        {row.original.submissionType === SubmissionType.CitizenReport ? (
          <Link
            to='/responses/citizen-reports/$citizenReportId'
            params={{ citizenReportId: row.original.submissionId }}
            preload='intent'
            target='_blank'>
            <Button type='button' variant={'link'} className='text-purple-500'>
              {row.original.submissionId.substring(0, 8)}
              <ArrowTopRightOnSquareIcon className='w-4' />
            </Button>
          </Link>
        ) : (
          <Link
            to='/responses/form-submissions/$submissionId'
            params={{ submissionId: row.original.submissionId }}
            preload='intent'
            target='_blank'>
            <Button type='button' variant={'link'} className='text-purple-500'>
              {row.original.submissionId.substring(0, 8)}
              <ArrowTopRightOnSquareIcon className='w-4' />
            </Button>
          </Link>
        )}
      </div>
    ),
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Type' column={column} />,
    accessorFn: (row) => row.type,
    id: 'type',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div className='max-w-[100px]'>{row.original.type}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Time submitted' column={column} />,
    accessorFn: (row) => row.timeSubmitted,
    id: 'timeSubmitted',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div className='max-w-[100px]'>{format(row.original.timeSubmitted, DateTimeFormat)}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Preview' column={column} />,
    id: 'preview',
    enableSorting: false,
    enableGlobalFilter: false,
    cell: ({ row }) => {
      return (
        <div>
          {row.original.type === 'Note' ? (
            <div className='flex space-x-2 break-words min-w-[350px]'>
              <span>{row.original.text}</span>
            </div>
          ) : (
            <MediaFilesCell attachment={row.original} />
          )}
        </div>
      );
    },
  },
];

export const quickReportsColumnDefs: ColumnDef<QuickReport>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title='Time submitted' column={column} />,
    accessorFn: (row) => row.timestamp,
    id: 'timestamp',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{format(row.original.timestamp, DateTimeFormat)}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Observer' column={column} />,
    id: 'observerName',
    accessorFn: (row) => row.observerName,
    enableSorting: false,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='NGO' column={column} />,
    accessorFn: (row) => row.ngoName,
    id: 'ngoName',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.ngoName}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location type' column={column} />,
    accessorFn: (row) => row.quickReportLocationType,
    id: 'quickReportLocationType',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{mapQuickReportLocationType(row.original.quickReportLocationType)}</div>,
  },

  {
    header: ({ column }) => <DataTableColumnHeader title='Incident category' column={column} />,
    accessorFn: (row) => row.incidentCategory,
    id: 'incidentCategory',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{mapIncidentCategory(row.original.incidentCategory)}</div>,
  },

  {
    header: ({ column }) => <DataTableColumnHeader title='Issue title' column={column} />,
    accessorFn: (row) => row.title,
    id: 'title',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.title.slice(0, 100) + '...'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Description' column={column} />,
    accessorFn: (row) => row.description,
    id: 'description',
    enableSorting: false,
    enableGlobalFilter: true,
    size: 200,
    cell: ({ row }) => <div>{row.original.description.slice(0, 100) + '...'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Media files' column={column} />,
    accessorFn: (row) => row.numberOfAttachments,
    id: 'numberOfAttachments',
    enableSorting: false,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L1' column={column} />,
    accessorFn: (row) => row.level1,
    id: 'level1',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level1 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L2' column={column} />,
    accessorFn: (row) => row.level2,
    id: 'level2',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level2 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L3' column={column} />,
    accessorFn: (row) => row.level3,
    id: 'level3',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level3 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L4' column={column} />,
    accessorFn: (row) => row.level4,
    id: 'level4',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level4 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L5' column={column} />,
    accessorFn: (row) => row.level5,
    id: 'level5',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level5 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Station number' column={column} />,
    accessorFn: (row) => row.number,
    id: 'number',
    enableSorting: false,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Polling station details' column={column} />,
    accessorFn: (row) => row.pollingStationDetails,
    id: 'pollingStationDetails',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.pollingStationDetails ?? '-'}</div>,
  },

  {
    header: ({ column }) => <DataTableColumnHeader title='Follow-up status' column={column} />,
    accessorFn: (row) => row.followUpStatus,
    id: 'followUpStatus',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => (
      <Badge
        className={cn({
          'text-slate-700 bg-slate-200': row.original.followUpStatus === QuickReportFollowUpStatus.NotApplicable,
          'text-red-600 bg-red-200': row.original.followUpStatus === QuickReportFollowUpStatus.NeedsFollowUp,
          'text-green-600 bg-green-200': row.original.followUpStatus === QuickReportFollowUpStatus.Resolved,
        })}>
        {row.original.followUpStatus === QuickReportFollowUpStatus.NotApplicable
          ? 'Not Applicable'
          : row.original.followUpStatus === QuickReportFollowUpStatus.NeedsFollowUp
            ? 'Needs follow-up'
            : 'Resolved'}
      </Badge>
    ),
  },
  {
    header: '',
    id: 'action',
    enableSorting: false,
    cell: ({ row }) => (
      <div className='text-right'>
        <Link
          className='inline-flex items-center justify-center w-6 h-6 rounded-full hover:bg-purple-100'
          params={{ quickReportId: row.original.id }}
          to='/responses/quick-reports/$quickReportId'>
          <ChevronRightIcon className='w-4 text-purple-600' />
        </Link>
      </div>
    ),
  },
];

export const observerQuickReportsColumnDefs: ColumnDef<QuickReport>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title='Time submitted' column={column} />,
    accessorFn: (row) => row.timestamp,
    id: 'timestamp',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{format(row.original.timestamp, DateTimeFormat)}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location type' column={column} />,
    accessorFn: (row) => row.quickReportLocationType,
    id: 'quickReportLocationType',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{mapQuickReportLocationType(row.original.quickReportLocationType)}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Issue title' column={column} />,
    accessorFn: (row) => row.title,
    id: 'title',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.title.slice(0, 100) + '...'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Description' column={column} />,
    accessorFn: (row) => row.description,
    id: 'description',
    enableSorting: false,
    enableGlobalFilter: true,
    size: 200,
    cell: ({ row }) => <div>{row.original.description.slice(0, 100) + '...'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Media files' column={column} />,
    accessorFn: (row) => row.numberOfAttachments,
    id: 'numberOfAttachments',
    enableSorting: false,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L1' column={column} />,
    accessorFn: (row) => row.level1,
    id: 'level1',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level1 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L2' column={column} />,
    accessorFn: (row) => row.level2,
    id: 'level2',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level2 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L3' column={column} />,
    accessorFn: (row) => row.level3,
    id: 'level3',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level3 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L4' column={column} />,
    accessorFn: (row) => row.level4,
    id: 'level4',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level4 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L5' column={column} />,
    accessorFn: (row) => row.level5,
    id: 'level5',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.level5 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Station number' column={column} />,
    accessorFn: (row) => row.number,
    id: 'number',
    enableSorting: false,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Polling station details' column={column} />,
    accessorFn: (row) => row.pollingStationDetails,
    id: 'pollingStationDetails',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.pollingStationDetails ?? '-'}</div>,
  },

  {
    header: ({ column }) => <DataTableColumnHeader title='Follow-up status' column={column} />,
    accessorFn: (row) => row.followUpStatus,
    id: 'followUpStatus',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => (
      <Badge
        className={cn({
          'text-slate-700 bg-slate-200': row.original.followUpStatus === QuickReportFollowUpStatus.NotApplicable,
          'text-red-600 bg-red-200': row.original.followUpStatus === QuickReportFollowUpStatus.NeedsFollowUp,
          'text-green-600 bg-green-200': row.original.followUpStatus === QuickReportFollowUpStatus.Resolved,
        })}>
        {row.original.followUpStatus === QuickReportFollowUpStatus.NotApplicable
          ? 'Not Applicable'
          : row.original.followUpStatus === QuickReportFollowUpStatus.NeedsFollowUp
            ? 'Needs follow-up'
            : 'Resolved'}
      </Badge>
    ),
  },
  {
    header: '',
    id: 'action',
    enableSorting: false,
    cell: ({ row }) => (
      <div className='text-right'>
        <Link
          className='inline-flex items-center justify-center w-6 h-6 rounded-full hover:bg-purple-100'
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
    accessorFn: (row) => row.timeSubmitted,
    id: 'timeSubmitted',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{format(row.original.timeSubmitted, DateTimeFormat)}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Form code' column={column} />,
    accessorFn: (row) => row.formCode,
    id: 'formCode',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Form name' column={column} />,
    accessorFn: (row) => row.formName,
    id: 'formName',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => (
      <div className='break-words'>{row.original.formName[row.original.formDefaultLanguage] ?? '-'}</div>
    ),
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Questions answered' column={column} />,
    accessorFn: (row) => row.numberOfQuestionsAnswered,
    id: 'numberOfQuestionsAnswered',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Flagged answers' column={column} />,
    accessorFn: (row) => row.numberOfFlaggedAnswers,
    id: 'numberOfFlaggedAnswers',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Level 1' column={column} />,
    accessorFn: (row) => row.level1,
    id: 'level1',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Level 2' column={column} />,
    accessorFn: (row) => row.level2,
    id: 'level2',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Level 3' column={column} />,
    accessorFn: (row) => row.level3,
    id: 'level3',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Level 4' column={column} />,
    accessorFn: (row) => row.level4,
    id: 'level4',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Level 5' column={column} />,
    accessorFn: (row) => row.level5,
    id: 'level5',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Question notes' column={column} />,
    accessorFn: (row) => row.notesCount,
    id: 'notesCount',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Media files' column={column} />,
    accessorFn: (row) => row.mediaFilesCount,
    id: 'mediaFilesCount',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Follow-up status' column={column} />,
    accessorFn: (row) => row.followUpStatus,
    id: 'followUpStatus',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => (
      <Badge
        className={cn({
          'text-slate-700 bg-slate-200': row.original.followUpStatus === CitizenReportFollowUpStatus.NotApplicable,
          'text-red-600 bg-red-200': row.original.followUpStatus === CitizenReportFollowUpStatus.NeedsFollowUp,
          'text-green-600 bg-green-200': row.original.followUpStatus === CitizenReportFollowUpStatus.Resolved,
        })}>
        {row.original.followUpStatus === CitizenReportFollowUpStatus.NotApplicable
          ? 'Not Applicable'
          : row.original.followUpStatus === CitizenReportFollowUpStatus.NeedsFollowUp
            ? 'Needs follow-up'
            : 'Resolved'}
      </Badge>
    ),
  },
  {
    header: '',
    id: 'action',
    enableSorting: false,
    cell: ({ row }) => (
      <div className='text-right'>
        <Link
          className='inline-flex items-center justify-center w-6 h-6 rounded-full hover:bg-purple-100'
          params={{ citizenReportId: row.original.id }}
          to='/responses/citizen-reports/$citizenReportId'>
          <ChevronRightIcon className='w-4 text-purple-600' />
        </Link>
      </div>
    ),
  },
];

export const citizenReportsAggregatedByFormColumnDefs: ColumnDef<CitizenReportsAggregatedByForm & RowData>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title='Form code' column={column} />,
    accessorFn: (row) => row.formCode,
    id: 'formCode',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Form name' column={column} />,
    accessorFn: (row) => row.formName,
    id: 'formName',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div className='break-words'>{row.original.formName[row.original.formDefaultLanguage]}</div>,
  },

  {
    header: ({ column }) => <DataTableColumnHeader title='Responses' column={column} />,
    accessorFn: (row) => row.numberOfSubmissions,
    id: 'numberOfSubmissions',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Flagged answers' column={column} />,
    accessorFn: (row) => row.numberOfFlaggedAnswers,
    id: 'numberOfFlaggedAnswers',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Question notes' column={column} />,
    accessorFn: (row) => row.numberOfNotes,
    id: 'numberOfNotes',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Media files' column={column} />,
    accessorFn: (row) => row.numberOfMediaFiles,
    id: 'numberOfMediaFiles',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: '',
    id: 'action',
    enableSorting: false,
    cell: ({ row }) => (
      <div className='text-right'>
        <Link
          className='inline-flex items-center justify-center w-6 h-6 rounded-full hover:bg-purple-100'
          params={{ formId: row.original.formId }}
          to='/responses/citizen-reports/$formId/aggregated'>
          <ChevronRightIcon className='w-4 text-purple-600' />
        </Link>
      </div>
    ),
  },
];

export const incidentReportsByEntryColumnDefs: ColumnDef<IncidentReportByEntry & RowData>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title='Entry ID' column={column} />,
    accessorFn: (row) => row.incidentReportId,
    id: 'incidentReportId',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Time submitted' column={column} />,
    accessorFn: (row) => row.timeSubmitted,
    id: 'timeSubmitted',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{format(row.original.timeSubmitted, DateTimeFormat)}</div>,
  },

  {
    header: ({ column }) => <DataTableColumnHeader title='Form code' column={column} />,
    accessorFn: (row) => row.formCode,
    id: 'formCode',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Form name' column={column} />,
    accessorFn: (row) => row.formName,
    id: 'formName',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => (
      <div className='break-words'>{row.original.formName[row.original.formDefaultLanguage] ?? '-'}</div>
    ),
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L1' column={column} />,
    accessorFn: (row) => row.pollingStationLevel1,
    id: 'pollingStationLevel1',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.pollingStationLevel1 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L2' column={column} />,
    accessorFn: (row) => row.pollingStationLevel2,
    id: 'pollingStationLevel2',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.pollingStationLevel2 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L3' column={column} />,
    accessorFn: (row) => row.pollingStationLevel3,
    id: 'pollingStationLevel3',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.pollingStationLevel3 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L4' column={column} />,
    accessorFn: (row) => row.pollingStationLevel4,
    id: 'pollingStationLevel4',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.pollingStationLevel4 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L5' column={column} />,
    accessorFn: (row) => row.pollingStationLevel5,
    id: 'pollingStationLevel5',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.pollingStationLevel5 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Number' column={column} />,
    accessorFn: (row) => row.pollingStationNumber,
    id: 'pollingStationNumber',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.pollingStationNumber ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location description' column={column} />,
    accessorFn: (row) => row.locationDescription,
    id: 'locationDescription',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.locationDescription ?? '-'}</div>,
  },

  {
    header: ({ column }) => <DataTableColumnHeader title='Observer' column={column} />,
    accessorFn: (row) => row.observerName,
    id: 'observerName',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.observerName}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='NGO' column={column} />,
    accessorFn: (row) => row.ngoName,
    id: 'ngoName',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.ngoName}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Observer tags' column={column} />,
    accessorFn: (row) => row.tags,
    id: 'tags',
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
    accessorFn: (row) => row.numberOfQuestionsAnswered,
    id: 'numberOfQuestionsAnswered',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Flagged answers' column={column} />,
    accessorFn: (row) => row.numberOfFlaggedAnswers,
    id: 'numberOfFlaggedAnswers',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Question notes' column={column} />,
    accessorFn: (row) => row.notesCount,
    id: 'notesCount',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Media files' column={column} />,
    accessorFn: (row) => row.mediaFilesCount,
    id: 'mediaFilesCount',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Follow-up status' column={column} />,
    accessorFn: (row) => row.followUpStatus,
    id: 'followUpStatus',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => (
      <Badge
        className={cn({
          'text-slate-700 bg-slate-200': row.original.followUpStatus === IncidentReportFollowUpStatus.NotApplicable,
          'text-red-600 bg-red-200': row.original.followUpStatus === IncidentReportFollowUpStatus.NeedsFollowUp,
          'text-green-600 bg-green-200': row.original.followUpStatus === IncidentReportFollowUpStatus.Resolved,
        })}>
        {row.original.followUpStatus === IncidentReportFollowUpStatus.NotApplicable
          ? 'Not Applicable'
          : row.original.followUpStatus === IncidentReportFollowUpStatus.NeedsFollowUp
            ? 'Needs follow-up'
            : 'Resolved'}
      </Badge>
    ),
  },
  {
    header: '',
    id: 'action',
    enableSorting: false,
    cell: ({ row }) => (
      <div className='text-right'>
        <Link
          className='inline-flex items-center justify-center w-6 h-6 rounded-full hover:bg-purple-100'
          params={{ incidentReportId: row.original.incidentReportId }}
          to='/responses/incident-reports/$incidentReportId'>
          <ChevronRightIcon className='w-4 text-purple-600' />
        </Link>
      </div>
    ),
  },
];

export const observerIncidentReportsColumnDefs: ColumnDef<IncidentReportByEntry & RowData>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title='Time submitted' column={column} />,
    accessorFn: (row) => row.timeSubmitted,
    id: 'timeSubmitted',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{format(row.original.timeSubmitted, DateTimeFormat)}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Form code' column={column} />,
    accessorFn: (row) => row.formCode,
    id: 'formCode',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Form name' column={column} />,
    accessorFn: (row) => row.formName,
    id: 'formName',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => (
      <div className='break-words'>{row.original.formName[row.original.formDefaultLanguage] ?? '-'}</div>
    ),
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location type' column={column} />,
    accessorFn: (row) => row.locationType,
    id: 'locationType',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{mapIncidentReportLocationType(row.original.locationType)}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location description' column={column} />,
    accessorFn: (row) => row.locationDescription,
    id: 'locationDescription',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.locationDescription ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L1' column={column} />,
    accessorFn: (row) => row.pollingStationLevel1,
    id: 'pollingStationLevel1',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.pollingStationLevel1 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L2' column={column} />,
    accessorFn: (row) => row.pollingStationLevel2,
    id: 'pollingStationLevel2',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.pollingStationLevel2 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L3' column={column} />,
    accessorFn: (row) => row.pollingStationLevel3,
    id: 'pollingStationLevel3',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.pollingStationLevel3 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L4' column={column} />,
    accessorFn: (row) => row.pollingStationLevel4,
    id: 'pollingStationLevel4',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.pollingStationLevel4 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L5' column={column} />,
    accessorFn: (row) => row.pollingStationLevel5,
    id: 'pollingStationLevel5',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.pollingStationLevel5 ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Number' column={column} />,
    accessorFn: (row) => row.pollingStationLevel5,
    id: 'pollingStationNumber',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.pollingStationNumber ?? '-'}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Questions answered' column={column} />,
    accessorFn: (row) => row.numberOfQuestionsAnswered,
    id: 'numberOfQuestionsAnswered',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Flagged answers' column={column} />,
    accessorFn: (row) => row.numberOfFlaggedAnswers,
    id: 'numberOfFlaggedAnswers',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Question notes' column={column} />,
    accessorFn: (row) => row.notesCount,
    id: 'notesCount',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Media files' column={column} />,
    accessorFn: (row) => row.mediaFilesCount,
    id: 'mediaFilesCount',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Follow-up status' column={column} />,
    accessorFn: (row) => row.followUpStatus,
    id: 'followUpStatus',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => (
      <Badge
        className={cn({
          'text-slate-700 bg-slate-200': row.original.followUpStatus === IncidentReportFollowUpStatus.NotApplicable,
          'text-red-600 bg-red-200': row.original.followUpStatus === IncidentReportFollowUpStatus.NeedsFollowUp,
          'text-green-600 bg-green-200': row.original.followUpStatus === IncidentReportFollowUpStatus.Resolved,
        })}>
        {row.original.followUpStatus === IncidentReportFollowUpStatus.NotApplicable
          ? 'Not Applicable'
          : row.original.followUpStatus === IncidentReportFollowUpStatus.NeedsFollowUp
            ? 'Needs follow-up'
            : 'Resolved'}
      </Badge>
    ),
  },
  {
    header: '',
    id: 'action',
    enableSorting: false,
    cell: ({ row }) => (
      <div className='text-right'>
        <Link
          className='inline-flex items-center justify-center w-6 h-6 rounded-full hover:bg-purple-100'
          params={{ incidentReportId: row.original.incidentReportId }}
          to='/responses/incident-reports/$incidentReportId'>
          <ChevronRightIcon className='w-4 text-purple-600' />
        </Link>
      </div>
    ),
  },
];

export const incidentReportsByObserverColumnDefs: ColumnDef<IncidentReportByObserver & RowData>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title='Observer name' column={column} />,
    accessorFn: (row) => row.observerName,
    id: 'observerName',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.observerName}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Observer contact' column={column} />,
    accessorFn: (row) => row.phoneNumber,
    id: 'phoneNumber',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='NGO' column={column} />,
    accessorFn: (row) => row.ngoName,
    id: 'ngoName',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) => <div>{row.original.ngoName}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Observer tags' column={column} />,
    accessorFn: (row) => row.tags,
    id: 'tags',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({
      row: {
        original: { tags },
      },
    }) => <TableTagList tags={tags} />,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Number of submissions' column={column} />,
    accessorFn: (row) => row.numberOfIncidentsSubmitted,
    id: 'numberOfIncidentsSubmitted',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Flagged answers' column={column} />,
    accessorFn: (row) => row.numberOfFlaggedAnswers,
    id: 'numberOfFlaggedAnswers',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Follow-up status' column={column} />,
    accessorFn: (row) => row.followUpStatus,
    id: 'followUpStatus',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({ row }) =>
      row.original.followUpStatus === IncidentReportFollowUpStatus.NeedsFollowUp ? (
        <Badge
          className={cn({
            'text-red-600 bg-red-200': row.original.followUpStatus === IncidentReportFollowUpStatus.NeedsFollowUp,
          })}>
          Needs followup
        </Badge>
      ) : (
        <span>-</span>
      ),
  },
  {
    header: '',
    id: 'action',
    enableSorting: false,
    cell: ({ row }) => (
      <div className='text-right'>
        <Link
          className='inline-flex items-center justify-center w-6 h-6 rounded-full hover:bg-purple-100'
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

export const incidentReportsByFormColumnDefs: ColumnDef<IncidentReportByForm & RowData>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title='Form code' column={column} />,
    accessorFn: (row) => row.formCode,
    id: 'formCode',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Form name' column={column} />,
    accessorFn: (row) => row.formName,
    id: 'formName',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => <div className='break-words'>{row.original.formName[row.original.formDefaultLanguage]}</div>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Responses' column={column} />,
    accessorFn: (row) => row.numberOfSubmissions,
    id: 'numberOfSubmissions',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Flagged answers' column={column} />,
    accessorFn: (row) => row.numberOfFlaggedAnswers,
    id: 'numberOfFlaggedAnswers',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Question notes' column={column} />,
    accessorFn: (row) => row.numberOfNotes,
    id: 'numberOfNotes',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Media files' column={column} />,
    accessorFn: (row) => row.numberOfMediaFiles,
    id: 'numberOfMediaFiles',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: '',
    id: 'action',
    enableSorting: false,
    cell: ({ row }) => (
      <div className='text-right'>
        <Link
          className='inline-flex items-center justify-center w-6 h-6 rounded-full hover:bg-purple-100'
          params={{ formId: row.original.formId }}
          to='/responses/form-submissions/$formId/aggregated'>
          <ChevronRightIcon className='w-4 text-purple-600' />
        </Link>
      </div>
    ),
  },
];
