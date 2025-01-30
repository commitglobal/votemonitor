import Layout from '@/components/layout/Layout';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Input } from '@/components/ui/input';
import { Separator } from '@/components/ui/separator';
import { useDialog } from '@/components/ui/use-dialog';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { useDebouncedSearch } from '@/hooks/debounced-search';
import { ngoRouteSearchSchema, Route } from '@/routes/ngos';
import { Cog8ToothIcon, FunnelIcon } from '@heroicons/react/24/outline';
import { EllipsisVerticalIcon } from '@heroicons/react/24/solid';
import { useNavigate } from '@tanstack/react-router';
import type { ColumnDef } from '@tanstack/react-table';
import { Plus } from 'lucide-react';
import { useCallback, type ReactElement } from 'react';
import { useNgoMutations, useNGOs } from '../../hooks/ngos-queriess';
import { NGO, NGOStatus } from '../../models/NGO';
import CreateNGODialog from '../CreateNGODialog';
import { NGOsListFilters } from '../filtering/NGOsListFilters';
import { NgoStatusBadge } from '../NgoStatusBadges';

export default function NGOsDashboard(): ReactElement {
  const navigate = useNavigate();
  const { isFilteringContainerVisible, toggleFilteringContainerVisibility } = useFilteringContainer();
  const { searchText, handleSearchInput, queryParams } = useDebouncedSearch(Route.id, ngoRouteSearchSchema);
  const createNgoDialog = useDialog();

  const { deactivateNgoMutation, activateNgoMutation, deleteNgoWithConfirmation } = useNgoMutations();

  const navigateToNgo = useCallback(
    (ngoId: string) => {
      void navigate({ to: '/ngos/view/$ngoId/$tab', params: { ngoId, tab: 'details' } });
    },
    [navigate]
  );

  const ngoColDefs: ColumnDef<NGO>[] = [
    {
      header: 'ID',
      accessorKey: 'id',
    },
    {
      accessorKey: 'name',
      enableSorting: true,
      header: ({ column }) => <DataTableColumnHeader title='Name' column={column} />,
    },

    {
      accessorKey: 'numberOfNgoAdmins',
      enableSorting: true,
      header: ({ column }) => <DataTableColumnHeader title='Admins' column={column} />,
    },

    {
      accessorKey: 'numberOfElectionsMonitoring',
      enableSorting: true,
      header: ({ column }) => <DataTableColumnHeader title='Election events' column={column} />,
    },

    {
      accessorKey: 'dateOfLastElection',
      enableSorting: false,
      header: ({ column }) => <DataTableColumnHeader title='Date of last event' column={column} />,
      cell: ({
        row: {
          original: { dateOfLastElection },
        },
      }) => dateOfLastElection ?? 'N/A',
    },

    {
      accessorKey: 'status',
      enableSorting: false,
      header: ({ column }) => <DataTableColumnHeader title='Status' column={column} />,
      cell: ({
        row: {
          original: { status },
        },
      }) => <NgoStatusBadge status={status} />,
    },

    {
      id: 'actions',
      cell: ({ row }) => {
        const navigate = useNavigate();
        const isNGOActive = row.original.status === NGOStatus.Activated;

        return (
          <div className='text-right'>
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant='ghost-primary' size='icon'>
                  <span className='sr-only'>Actions</span>
                  <EllipsisVerticalIcon className='w-6 h-6' />
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align='end'>
                <DropdownMenuItem
                  onClick={() =>
                    navigate({ to: '/ngos/view/$ngoId/$tab', params: { ngoId: row.original.id, tab: 'details' } })
                  }>
                  Edit
                </DropdownMenuItem>
                <DropdownMenuItem
                  onClick={(e) => {
                    e.stopPropagation();
                    isNGOActive
                      ? deactivateNgoMutation.mutate(row.original.id)
                      : activateNgoMutation.mutate(row.original.id);
                  }}>
                  {isNGOActive ? 'Deactivate' : 'Activate'}
                </DropdownMenuItem>
                <DropdownMenuItem
                  className='text-red-600'
                  onClick={async (e) => {
                    e.stopPropagation();

                    await deleteNgoWithConfirmation({ ngoId: row.original.id, name: row.original.name });
                  }}>
                  Delete
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
          </div>
        );
      },
    },
  ];

  return (
    <Layout title={'Organizations'} subtitle='Manage'>
      <Card className='w-full pt-0'>
        <CardHeader className='flex gap-2 flex-column'>
          <div className='flex flex-row items-center justify-between'>
            <CardTitle className='text-xl'>All organizations</CardTitle>
            <div className='flex md:flex-row-reverse gap-4 table-actions'>
              <Button title='Add NGO' onClick={() => createNgoDialog.trigger()}>
                <Plus className='mr-2' width={18} height={18} />
                Add NGO
              </Button>
              <CreateNGODialog {...createNgoDialog.dialogProps} />
            </div>
          </div>
          <Separator />

          <div className='flex flex-row justify-end gap-4  filters'>
            <Input value={searchText} onChange={handleSearchInput} className='max-w-md' placeholder='Search' />
            <FunnelIcon
              onClick={toggleFilteringContainerVisibility}
              className='w-[20px] text-purple-900 cursor-pointer'
              fill={isFilteringContainerVisible ? '#5F288D' : 'rgba(0,0,0,0)'}
            />
            <Cog8ToothIcon className='w-[20px] text-purple-900' />
          </div>
          {isFilteringContainerVisible && <NGOsListFilters />}
        </CardHeader>
        <CardContent>
          <QueryParamsDataTable
            columns={ngoColDefs}
            useQuery={(params) => useNGOs(params)}
            queryParams={queryParams}
            onRowClick={navigateToNgo}
          />
        </CardContent>
      </Card>
    </Layout>
  );
}
