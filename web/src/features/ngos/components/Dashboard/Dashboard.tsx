import Layout from '@/components/layout/Layout';
import { useConfirm } from '@/components/ui/alert-dialog-provider';
import { Button, buttonVariants } from '@/components/ui/button';
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
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { Route } from '@/routes/ngos';
import { Cog8ToothIcon, FunnelIcon } from '@heroicons/react/24/outline';
import { EllipsisVerticalIcon } from '@heroicons/react/24/solid';
import { useNavigate } from '@tanstack/react-router';
import type { ColumnDef } from '@tanstack/react-table';
import { useDebounce } from '@uidotdev/usehooks';
import { Plus } from 'lucide-react';
import { useCallback, useEffect, useMemo, useState, type ReactElement } from 'react';
import { useActivateNGO, useDeactivateNGO, useDeteleteNGO, useNGOs } from '../../hooks/ngos-queriess';
import { NGO, NGOStatus } from '../../models/NGO';
import CreateNGODialog from '../CreateNGODialog';
import { NGOsListFilters } from '../filtering/NGOsListFilters';
import { NgoStatusBadge } from '../NgoStatusBadges';

export default function NGOsDashboard(): ReactElement {
  const navigate = useNavigate();
  const confirm = useConfirm();
  const { isFilteringContainerVisible, navigateHandler, toggleFilteringContainerVisibility } = useFilteringContainer();
  const search = Route.useSearch();
  const createNgoDialog = useDialog();
  const [searchText, setSearchText] = useState(search.searchText);
  const handleSearchInput = (ev: React.FormEvent<HTMLInputElement>) => {
    setSearchText(ev.currentTarget.value);
  };

  const debouncedSearch = useDebounce(search, 300);
  const debouncedSearchText = useDebounce(searchText, 300);

  const { deactivateNgoMutation } = useDeactivateNGO();
  const { activateNgoMutation } = useActivateNGO();
  const { deleteNgoMutation } = useDeteleteNGO();

  useEffect(() => {
    navigateHandler({
      [FILTER_KEY.SearchText]: debouncedSearchText,
    });
  }, [debouncedSearchText]);

  useEffect(() => {
    setSearchText(search.searchText ?? '');
  }, [search.searchText]);

  const queryParams = useMemo(() => {
    const params = [
      ['searchText', debouncedSearch.searchText],
      ['status', debouncedSearch.status],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params);
  }, [debouncedSearch]);

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
                    if (
                      await confirm({
                        title: `Delete ${row.original.name}?`,
                        body: 'This action is permanent and cannot be undone. Once deleted, this organization cannot be retrieved.',
                        actionButton: 'Delete',
                        actionButtonClass: buttonVariants({ variant: 'destructive' }),
                        cancelButton: 'Cancel',
                      })
                    ) {
                      deleteNgoMutation.mutate(row.original.id);
                    }
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
