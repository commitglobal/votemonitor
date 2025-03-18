import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { ColumnDef, createColumnHelper } from '@tanstack/react-table';
import ElectionRoundStatusBadge from '../../../../components/ElectionRoundStatusBadge/ElectionRoundStatusBadge';
import { ElectionRoundModel } from '../../models/types';
import { ElectionRoundDataTableRowActions } from './ElectionRoundDataTableRowActions';

const columnHelper = createColumnHelper<ElectionRoundModel>();

export const electionRoundColDefs: ColumnDef<ElectionRoundModel>[] = [
  columnHelper.display({
    id: 'title',
    enableResizing: true,
    enableSorting: true,
    header: ({ column }) => <DataTableColumnHeader title='Title' column={column} />,
    cell: ({ row }) => row.original.title,
  }),
  columnHelper.display({
    id: 'englishTitle',
    enableResizing: true,
    enableSorting: true,
    header: ({ column }) => <DataTableColumnHeader title='English title' column={column} />,
    cell: ({ row }) => row.original.englishTitle,
  }),
  columnHelper.display({
    id: 'countryName',
    enableSorting: true,
    header: ({ column }) => <DataTableColumnHeader title='Country' column={column} />,
    cell: ({ row }) => row.original.countryName,
  }),
  columnHelper.display({
    id: 'startDate',
    enableSorting: true,
    header: ({ column }) => <DataTableColumnHeader title='Start date' column={column} />,
    cell: ({ row }) => row.original.startDate,
  }),
  columnHelper.display({
    id: 'status',
    enableSorting: true,
    header: ({ column }) => <DataTableColumnHeader title='Status' column={column} />,
    cell: ({
      row: {
        original: { status },
      },
    }) => <ElectionRoundStatusBadge status={status} />,
  }),
  columnHelper.display({
    id: 'monitoringNgos',
    enableSorting: true,
    header: ({ column }) => <DataTableColumnHeader title='NGOs' column={column} />,
    cell: ({ row }) => row.original.monitoringNgos.length,
  }),
  {
    id: 'actions',
    cell: ({ row }) => <ElectionRoundDataTableRowActions electionRound={row.original} />,
  },
];
