import { DataTableRowAction, PollingStation } from '@/common/types';
import i18n from '@/i18n';
import { ColumnDef } from '@tanstack/react-table';
import TableTagList from '../table-tag-list/TableTagList';
import { Button } from '../ui/button';
import { DataTableColumnHeader } from '../ui/DataTable/DataTableColumnHeader';
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from '../ui/dropdown-menu';
import { MoreHorizontal } from 'lucide-react';
export type PollingStationAction = 'edit' | 'delete' | 'add';

export const getPollingStationColDefs = (
  userRole: string | undefined,
  setRowAction: React.Dispatch<React.SetStateAction<DataTableRowAction<PollingStation, PollingStationAction> | null>>
): ColumnDef<PollingStation>[] => {
  const columns: ColumnDef<PollingStation>[] = [
    {
      header: ({ column }) => (
        <DataTableColumnHeader title={i18n.t('electionEvent.pollingStations.headers.level1')} column={column} />
      ),
      accessorKey: 'level1',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({
        row: {
          original: { level1 },
        },
      }) => <p>{level1}</p>,
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title={i18n.t('electionEvent.pollingStations.headers.level2')} column={column} />
      ),
      accessorKey: 'level2',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({
        row: {
          original: { level2 },
        },
      }) => <p>{level2}</p>,
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title={i18n.t('electionEvent.pollingStations.headers.level3')} column={column} />
      ),
      accessorKey: 'level3',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({
        row: {
          original: { level3 },
        },
      }) => <p>{level3}</p>,
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title={i18n.t('electionEvent.pollingStations.headers.level4')} column={column} />
      ),
      accessorKey: 'level4',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({
        row: {
          original: { level4 },
        },
      }) => <p>{level4}</p>,
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title={i18n.t('electionEvent.pollingStations.headers.level5')} column={column} />
      ),
      accessorKey: 'level5',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({
        row: {
          original: { level5 },
        },
      }) => <p>{level5}</p>,
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title={i18n.t('electionEvent.pollingStations.headers.number')} column={column} />
      ),
      accessorKey: 'number',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({
        row: {
          original: { number },
        },
      }) => <p>{number}</p>,
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader
          title={i18n.t('translation:electionEvent.pollingStations.headers.address')}
          column={column}
        />
      ),
      accessorKey: 'address',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({
        row: {
          original: { address },
        },
      }) => <p>{address}</p>,
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader
          title={i18n.t('translation:electionEvent.pollingStations.headers.displayOrder')}
          column={column}
        />
      ),
      accessorKey: 'displayOrder',
      enableSorting: true,
      enableGlobalFilter: true,
      cell: ({
        row: {
          original: { displayOrder },
        },
      }) => <p>{displayOrder}</p>,
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title={i18n.t('electionEvent.pollingStations.headers.coordinates')} column={column} />
      ),
      accessorKey: 'coordinates',
      enableSorting: false,
      enableGlobalFilter: true,
      cell: ({
        row: {
          original: { latitude, longitude },
        },
      }) =>
        latitude && longitude ? (
          <Button asChild variant={'link'}>
            <a
              href={`https://www.google.com/maps?q=${latitude},${longitude}`}
              target='_blank'
              rel='noopener noreferrer'>
              {latitude},{longitude}
            </a>
          </Button>
        ) : (
          '-'
        ),
    },
    {
      header: ({ column }) => (
        <DataTableColumnHeader title={i18n.t('electionEvent.pollingStations.headers.tags')} column={column} />
      ),
      accessorKey: 'tags',
      enableSorting: false,
      enableGlobalFilter: true,
      cell: ({
        row: {
          original: { tags },
        },
      }) => <TableTagList tags={Object.entries(tags ?? {}).map(([key, value]) => `${key} : ${value}`)} />,
    },
  ];

  if (userRole === 'PlatformAdmin') {
    columns.push({
      id: 'actions',
      header: '',
      cell: ({ row }) => (
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button aria-label='Open menu' variant='ghost' className='flex size-8 p-0 data-[state=open]:bg-muted'>
              <MoreHorizontal className='size-4' aria-hidden='true' />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align='end' className='w-40'>
            <DropdownMenuItem onSelect={() => setRowAction({ row, variant: 'edit' })}>Edit</DropdownMenuItem>
            <DropdownMenuItem onSelect={() => setRowAction({ row, variant: 'delete' })}>Delete</DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      ),
    });
  }

  return columns;
};
