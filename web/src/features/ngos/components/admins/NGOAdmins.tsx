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
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { Route } from '@/routes/ngos/view.$ngoId.$tab';
import { Cog8ToothIcon, FunnelIcon } from '@heroicons/react/24/outline';
import { EllipsisVerticalIcon } from '@heroicons/react/24/solid';
import { useNavigate } from '@tanstack/react-router';
import type { ColumnDef } from '@tanstack/react-table';
import { useDebounce } from '@uidotdev/usehooks';
import { Plus } from 'lucide-react';
import { FC, useEffect, useMemo, useState } from 'react';
import { useNgoAdminMutations, useNgoAdmins } from '../../hooks/ngo-admin-queries';
import { NgoAdmin, NgoAdminStatus } from '../../models/NgoAdmin';
import { NGOsListFilters } from '../filtering/NGOsListFilters';
import { NgoAdminStatusBadge } from '../NgoStatusBadges';
import AddNgoAdminDialog from './AddNgoAdminDialog';

interface NGOAdminsViewProps {
  ngoId: string;
}

export const NGOAdminsView: FC<NGOAdminsViewProps> = ({ ngoId }) => {
  const navigate = useNavigate();
  const { deleteNgoAdminWithConfirmation, deactivateNgoAdminMutation, activateNgoAdminMutation } =
    useNgoAdminMutations(ngoId);
  const { isFilteringContainerVisible, navigateHandler, toggleFilteringContainerVisibility } = useFilteringContainer();
  const search = Route.useSearch();
  const [searchText, setSearchText] = useState(search.searchText);
  const handleSearchInput = (ev: React.FormEvent<HTMLInputElement>) => {
    setSearchText(ev.currentTarget.value);
  };
  const debouncedSearch = useDebounce(search, 300);
  const debouncedSearchText = useDebounce(searchText, 300);
  const addNgoAdminDialog = useDialog();

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

  const navigateToNgoAdmin = (ngoId: string, adminId: string) =>
    navigate({ to: '/ngos/admin/$ngoId/$adminId/view', params: { ngoId, adminId } });

  const ngoAdminsColDefs: ColumnDef<NgoAdmin>[] = [
    {
      header: 'ID',
      accessorKey: 'id',
    },
    {
      accessorKey: 'email',
      enableSorting: true,
      header: ({ column }) => <DataTableColumnHeader title='Email' column={column} />,
    },
    {
      accessorKey: 'firstName',
      enableSorting: true,
      header: ({ column }) => <DataTableColumnHeader title='First name' column={column} />,
    },
    {
      accessorKey: 'lastName',
      enableSorting: true,
      header: ({ column }) => <DataTableColumnHeader title='Last name' column={column} />,
    },

    {
      accessorKey: 'status',
      enableSorting: false,
      header: ({ column }) => <DataTableColumnHeader title='Status' column={column} />,
      cell: ({
        row: {
          original: { status },
        },
      }) => {
        return <NgoAdminStatusBadge status={status} />;
      },
    },
    {
      id: 'actions',
      cell: ({ row }) => {
        const adminId = row.original.id;
        const adminName = `${row.original.firstName} ${row.original.lastName}`;
        const isAdminActive = row.original.status === NgoAdminStatus.Active;

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
                <DropdownMenuItem onClick={() => navigateToNgoAdmin(ngoId, row.original.id)}>Edit</DropdownMenuItem>
                <DropdownMenuItem
                  onClick={(e) => {
                    e.stopPropagation();
                    isAdminActive
                      ? deactivateNgoAdminMutation.mutate(adminId)
                      : activateNgoAdminMutation.mutate(adminId);
                  }}>
                  {!isAdminActive ? 'Activate' : 'Deactivate'}
                </DropdownMenuItem>
                <DropdownMenuItem
                  className='text-red-600'
                  onClick={async (e) => {
                    e.stopPropagation();
                    await deleteNgoAdminWithConfirmation({ adminId, name: adminName });
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
    <Card className='w-full pt-0'>
      <CardHeader className='flex gap-2 flex-column'>
        <div className='flex flex-row items-center justify-between'>
          <CardTitle className='text-xl'>All admins</CardTitle>
          <div className='flex md:flex-row-reverse gap-4 table-actions'>
            <Button title='Add admin' onClick={() => addNgoAdminDialog.trigger()}>
              <Plus className='mr-2' width={18} height={18} />
              Add admin
            </Button>
            <AddNgoAdminDialog ngoId={ngoId} {...addNgoAdminDialog.dialogProps} />
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
          columns={ngoAdminsColDefs}
          useQuery={(params) => useNgoAdmins(ngoId, params)}
          queryParams={queryParams}
          onRowClick={(id) => navigateToNgoAdmin(ngoId, id)}
        />
      </CardContent>
    </Card>
  );
};
