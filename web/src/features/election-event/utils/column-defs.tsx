// import TableTagList from '@/components/table-tag-list/TableTagList';
// import { Badge } from '@/components/ui/badge';
// import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
// import { cn } from '@/lib/utils';
// import { ChevronRightIcon } from '@heroicons/react/24/outline';
// import { Link } from '@tanstack/react-router';
// import { format } from 'date-fns';

// import { MediaFilesCell } from '../components/MediaFilesCell/MediaFilesCell';

// import type { ColumnDef } from '@tanstack/react-table';
// import type { FormSubmissionByEntry, FormSubmissionByForm, FormSubmissionByObserver } from '../models/form-submission';
// import type { QuestionExtraData } from '../types';

// export const formSubmissionsByObserverColumnDefs: ColumnDef<FormSubmissionByObserver>[] = [
//   {
//     header: ({ column }) => <DataTableColumnHeader title='Observer name' column={column} />,
//     accessorKey: 'observerName',
//     enableSorting: true,
//     enableGlobalFilter: true,
//     cell: ({ row }) => (
//       <div>
//         {row.original.observerName}
//       </div>
//     ),
//   },
//   {
//     header: ({ column }) => <DataTableColumnHeader title='Observer contact' column={column} />,
//     accessorKey: 'phoneNumber',
//     enableSorting: true,
//     enableGlobalFilter: true,
//   },
//   {
//     header: ({ column }) => <DataTableColumnHeader title='Observer tags' column={column} />,
//     accessorKey: 'tags',
//     enableSorting: true,
//     enableGlobalFilter: true,
//     cell: ({
//       row: {
//         original: { tags },
//       },
//     }) => <TableTagList tags={tags} />,
//   },
//   {
//     header: ({ column }) => <DataTableColumnHeader title='Locations' column={column} />,
//     accessorKey: 'numberOfLocations',
//     enableSorting: true,
//     enableGlobalFilter: true,
//   },
//   {
//     header: ({ column }) => <DataTableColumnHeader title='Forms' column={column} />,
//     accessorKey: 'numberOfFormsSubmitted',
//     enableSorting: true,
//     enableGlobalFilter: true,
//   },
//   {
//     header: ({ column }) => <DataTableColumnHeader title='Flagged answers' column={column} />,
//     accessorKey: 'numberOfFlaggedAnswers',
//     enableSorting: true,
//     enableGlobalFilter: true,
//   },
//   {
//     header: ({ column }) => <DataTableColumnHeader title='Status' column={column} />,
//     accessorKey: 'status',
//     enableSorting: false,
//     enableGlobalFilter: true,
//     cell: ({ row }) => {
//       return row.original?.needsFollowUp === true ? <Badge
//         className={cn({
//           'text-red-600 bg-red-200': row.original?.needsFollowUp === true,
//         })}>Needs followup</Badge> : <span>-</span>
//     }
//   },
//   {
//     header: '',
//     accessorKey: 'action',
//     enableSorting: false,
//     cell: ({ row }) => (
//       <Link
//         className='hover:bg-purple-100 inline-flex h-6 w-6 rounded-full items-center justify-center'
//         params={{ monitoringObserverId: row.original.monitoringObserverId }}
//         to='/monitoring-observers/$monitoringObserverId'>
//         <ChevronRightIcon className='w-4 text-purple-600' />
//       </Link>
//     ),
//   },
// ];

// export const formSubmissionsByFormColumnDefs: ColumnDef<FormSubmissionByForm>[] = [
//   {
//     header: ({ column }) => <DataTableColumnHeader title='Form code' column={column} />,
//     accessorKey: 'formCode',
//     enableSorting: true,
//     enableGlobalFilter: true,
//   },
//   {
//     header: ({ column }) => <DataTableColumnHeader title='Form type' column={column} />,
//     accessorKey: 'formType',
//     enableSorting: true,
//     enableGlobalFilter: true,
//   },
//   {
//     header: ({ column }) => <DataTableColumnHeader title='Responses' column={column} />,
//     accessorKey: 'numberOfSubmissions',
//     enableSorting: true,
//     enableGlobalFilter: true,
//   },
//   {
//     header: ({ column }) => <DataTableColumnHeader title='Flagged answers' column={column} />,
//     accessorKey: 'numberOfFlaggedAnswers',
//     enableSorting: true,
//     enableGlobalFilter: true,
//   },
//   {
//     header: ({ column }) => <DataTableColumnHeader title='Question notes' column={column} />,
//     accessorKey: 'numberOfNotes',
//     enableSorting: true,
//     enableGlobalFilter: true,
//   },
//   {
//     header: ({ column }) => <DataTableColumnHeader title='Media files' column={column} />,
//     accessorKey: 'numberOfMediaFiles',
//     enableSorting: true,
//     enableGlobalFilter: true,
//   },
//   {
//     header: '',
//     accessorKey: 'action',
//     enableSorting: false,
//     cell: ({ row }) => (
//       <Link
//         className='hover:bg-purple-100 inline-flex h-6 w-6 rounded-full items-center justify-center'
//         params={{ formId: row.original.formId }}
//         to='/responses/$formId/aggregated'>
//         <ChevronRightIcon className='w-4 text-purple-600' />
//       </Link>
//     ),
//   },
// ];

// export const questionExtraInfoColumnDefs: ColumnDef<QuestionExtraData>[] = [
//   {
//     header: ({ column }) => <DataTableColumnHeader title='Time submitted' column={column} />,
//     accessorKey: 'timeSubmitted',
//     enableSorting: true,
//     enableGlobalFilter: true,
//     cell: ({ row }) => <div>{format(row.original.timeSubmitted, 'u-MM-dd KK:mm')}</div>
//   },
//   {
//     header: ({ column }) => <DataTableColumnHeader title='Note' column={column} />,
//     accessorKey: 'text',
//     enableSorting: true,
//     enableGlobalFilter: true,
//     minSize: 260,
//   },
//   {
//     header: ({ column }) => <DataTableColumnHeader title='Media files' column={column} />,
//     accessorKey: 'attachments',
//     enableSorting: false,
//     enableGlobalFilter: false,
//     cell: MediaFilesCell,
//     size: 200,
//   },
// ];
