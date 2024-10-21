import { DateTimeFormat } from '@/common/formats';
import TableTagList from '@/components/table-tag-list/TableTagList';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { Badge } from '@/components/ui/badge';
import { ColumnDef } from '@tanstack/react-table';
import { format } from 'date-fns';
import { TargetedMonitoringObserver } from '../models/targeted-monitoring-observer';

export const targetedMonitoringObserverColDefs: ColumnDef<TargetedMonitoringObserver>[] = [
  {
    header: ({ column }) => <DataTableColumnHeader title='Name' column={column} />,
    accessorKey: 'observerName',
    enableSorting: true,
    enableGlobalFilter: false,
    cell: ({
      row: {
        original: { observerName },
      },
    }) => <p>{observerName}</p>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Email' column={column} />,
    accessorKey: 'email',
    enableSorting: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Observer tags' column={column} />,
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
    enableSorting: true,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Observer status' column={column} />,
    accessorKey: 'status',
    enableSorting: true,
    cell: ({
      row: {
        original: { status },
      },
    }) => <Badge className={'badge-' + status}>{status}</Badge>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Latest activity at' column={column} />,
    accessorKey: 'latestActivityAt',
    enableSorting: true,
    cell: ({
      row: {
        original: { latestActivityAt },
      },
    }) => <p>{latestActivityAt ? format(latestActivityAt, DateTimeFormat) : '-'}</p>,
  },
  {
    header: ({ column }) => <DataTableColumnHeader title='Submissions' column={column} />,
    accessorKey: 'numberOfFormsSubmitted',
    enableSorting: true,
  },

  {
    header: ({ column }) => <DataTableColumnHeader title='Locations' column={column} />,
    accessorKey: 'numberOfLocations',
    enableSorting: true,
  },
];
