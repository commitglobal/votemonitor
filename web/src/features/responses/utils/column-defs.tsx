import type { ColumnDef, VisibilityState } from '@tanstack/react-table';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import type { FormSubmissionByEntry } from '../models/form-submission';

export const formSubmissionsByEntryColumnDefs: ColumnDef<FormSubmissionByEntry>[] = [
  // {
  //   header: ({ column }) => <DataTableColumnHeader title='Entry ID' column={column} />,
  //   accessorKey: 'entryId',
  //   enableSorting: true,
  //   enableGlobalFilter: true,
  // },
  {
    header: ({ column }) => <DataTableColumnHeader title='Time submitted' column={column} />,
    accessorKey: 'submittedAt',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  // {
  //   header: ({ column }) => <DataTableColumnHeader title='Form name' column={column} />,
  //   accessorKey: 'formName',
  //   enableSorting: true,
  //   enableGlobalFilter: true,
  // },
  {
    header: ({ column }) => <DataTableColumnHeader title='Form type' column={column} />,
    accessorKey: 'formType',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  // {
  //   header: ({ column }) => <DataTableColumnHeader title='Language' column={column} />,
  //   accessorKey: 'language',
  //   enableSorting: true,
  //   enableGlobalFilter: true,
  // },
  {
    header: ({ column }) => <DataTableColumnHeader title='Station number' column={column} />,
    accessorKey: 'pollingStationNumber',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L1' column={column} />,
    accessorKey: 'pollingStationLevel1',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L2' column={column} />,
    accessorKey: 'pollingStationLevel2',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Location - L3' column={column} />,
    accessorKey: 'pollingStationLevel3',
    enableSorting: true,
    enableGlobalFilter: true,
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
  // {
  //   header: ({ column }) => <DataTableColumnHeader title='Tags' column={column} />,
  //   accessorKey: 'tags',
  //   enableSorting: true,
  //   enableGlobalFilter: true,
  // },
  {
    header: ({ column }) => <DataTableColumnHeader title='Responses' column={column} />,
    accessorKey: 'numberOfQuestionAnswered',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Flagged answers' column={column} />,
    accessorKey: 'numberOfFlaggedAnswers',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  // {
  //   header: ({ column }) => <DataTableColumnHeader title='Question notes' column={column} />,
  //   accessorKey: 'questionNotes',
  //   enableSorting: true,
  //   enableGlobalFilter: true,
  // },
  // {
  //   header: ({ column }) => <DataTableColumnHeader title='Medial files' column={column} />,
  //   accessorKey: 'mediaFiles',
  //   enableSorting: true,
  //   enableGlobalFilter: true,
  // },
  // {
  //   header: ({ column }) => <DataTableColumnHeader title='Status' column={column} />,
  //   accessorKey: 'formStatus',
  //   enableSorting: true,
  //   enableGlobalFilter: true,
  // },
];

export const formSubmissionsByEntryDefaultColumns: VisibilityState = {
  submittedAt: true,
  formType: true,
  pollingStationNumber: true,
  pollingStationLevel1: false,
  pollingStationLevel2: false,
  pollingStationLevel3: false,
  observerName: false,
  numberOfQuestionAnswered: true,
  numberOfFlaggedAnswers: true,
};
