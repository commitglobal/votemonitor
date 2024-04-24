import type { ColumnDef } from '@tanstack/react-table';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import type { FormSubmissionByEntry, FormSubmissionByForm, FormSubmissionByObserver } from '../models/form-submission';

export const formSubmissionsByEntryColumnDefs: ColumnDef<FormSubmissionByEntry>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title='Entry ID' column={column} />,
    accessorKey: 'submissionId',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Time submitted' column={column} />,
    accessorKey: 'timeSubmitted',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Form name' column={column} />,
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
    cell: ({ row }) => (
      <div>
        {row.original.level1 ?? '-'}
      </div>
    ),
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L2' column={column} />,
    accessorKey: 'level2',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => (
      <div>
        {row.original.level2 ?? '-'}

      </div>
    ),
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L3' column={column} />,
    accessorKey: 'level3',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => (
      <div>
        {row.original.level3 ?? '-'}

      </div>
    ),
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L4' column={column} />,
    accessorKey: 'level4',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => (
      <div>
        {row.original.level4 ?? '-'}
      </div>
    ),
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L5' column={column} />,
    accessorKey: 'level5',
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => (
      <div>
        {row.original.level5 ?? '-'}
      </div>
    ),
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
    header: ({ column }) => <DataTableColumnHeader title='Tags' column={column} />,
    accessorKey: 'tags',
    enableSorting: true,
    enableGlobalFilter: true,
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
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => row.original?.status ?? 'N/A',
  },
];

export const formSubmissionsByObserverColumnDefs: ColumnDef<FormSubmissionByObserver>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title='Observer name' column={column} />,
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
    accessorKey: 'phoneNumber',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Observer tags' column={column} />,
    accessorKey: 'tags',
    enableSorting: true,
    enableGlobalFilter: true,
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
    enableSorting: true,
    enableGlobalFilter: true,
    cell: ({ row }) => row.original?.status ?? 'N/A',
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
];
