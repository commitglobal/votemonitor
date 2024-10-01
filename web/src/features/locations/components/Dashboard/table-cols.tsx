import { ColumnDef } from '@tanstack/react-table';
import { Location } from '../../models/Location';

import TableTagList from '@/components/table-tag-list/TableTagList';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import i18n from '@/i18n';

export const locationColDefs: ColumnDef<Location>[] = [
  {
    header: ({ column }) => (
      <DataTableColumnHeader title={i18n.t('electionEvent.locations.headers.level1')} column={column} />
    ),
    accessorKey: 'level1',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => (
      <DataTableColumnHeader title={i18n.t('electionEvent.locations.headers.level2')} column={column} />
    ),
    accessorKey: 'level2',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => (
      <DataTableColumnHeader title={i18n.t('electionEvent.locations.headers.level3')} column={column} />
    ),
    accessorKey: 'level3',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => (
      <DataTableColumnHeader title={i18n.t('electionEvent.locations.headers.level4')} column={column} />
    ),
    accessorKey: 'level4',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => (
      <DataTableColumnHeader title={i18n.t('electionEvent.locations.headers.level5')} column={column} />
    ),
    accessorKey: 'level5',
    enableSorting: true,
    enableGlobalFilter: true,
  },
  {
    header: ({ column }) => (
      <DataTableColumnHeader title={i18n.t('electionEvent.locations.headers.tags')} column={column} />
    ),
    accessorKey: 'tags',
    enableSorting: false,
    enableGlobalFilter: true,
    cell: ({
      row: {
        original: { tags },
      },
    }) => <TableTagList tags={Object.entries(tags).map(([key, value]) => `${key} : ${value}`)} />,
  },
];
