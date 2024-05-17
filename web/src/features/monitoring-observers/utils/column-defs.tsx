import { DataTableParameters, PageResponse } from '@/common/types';
import { ColumnDef } from '@tanstack/react-table';
import { MonitoringObserver } from '../models/MonitoringObserver';
import TableTagList from '@/components/table-tag-list/TableTagList';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { Badge } from 'lucide-react';

export const targetedMonitoringObserverColDefs: ColumnDef<MonitoringObserver>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title='Name' column={column} />,
    accessorKey: 'name',
    enableSorting: false,
    enableGlobalFilter: false,
    cell: ({
      row: {
        original: { firstName, lastName },
      },
    }) => (
      <p>
        {firstName} {lastName}
      </p>
    ),
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Email' column={column} />,
    accessorKey: 'email',
    enableSorting: false,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Tags' column={column} />,
    accessorKey: 'tags',
    cell: ({
      row: {
        original: { tags },
      },
    }) => <TableTagList tags={tags} />,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Phone' column={column} />,
    accessorKey: 'phoneNumber',
    enableSorting: false,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Observer status' column={column} />,
    accessorKey: 'status',
    enableSorting: false,
    cell: ({
      row: {
        original: { status },
      },
    }) => <Badge className={'badge-' + status}>{status}</Badge>,
  },
];