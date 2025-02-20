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
import { ngoAdminsSearchParamsSchema, Route } from '@/routes/ngos/view.$ngoId.$tab';
import { Cog8ToothIcon, FunnelIcon } from '@heroicons/react/24/outline';
import { EllipsisVerticalIcon } from '@heroicons/react/24/solid';
import { useNavigate } from '@tanstack/react-router';
import type { ColumnDef } from '@tanstack/react-table';
import { Plus } from 'lucide-react';
import { FC, useCallback } from 'react';
import { useNgoAdminMutations, useNgoAdmins } from '../../hooks/ngo-admin-queries';
import { NgoAdmin, NgoAdminStatus } from '../../models/NgoAdmin';
import { NgoAdminStatusBadge } from '../NgoStatusBadges';
import AddNgoAdminDialog from './AddNgoAdminDialog';

interface NGOAdminsViewProps {
  ngoId: string;
}

export const NGOAdminsView: FC<NGOAdminsViewProps> = ({ ngoId }) => {
  const navigate = useNavigate();
  const { deleteNgoAdminWithConfirmation, deactivateNgoAdminMutation, activateNgoAdminMutation } =
    useNgoAdminMutations(ngoId);
  const { isFilteringContainerVisible, toggleFilteringContainerVisibility } = useFilteringContainer();
  const addNgoAdminDialog = useDialog();
  const { queryParams, searchText, handleSearchInput } = useDebouncedSearch(Route.id, ngoAdminsSearchParamsSchema);

  const navigateToViewNgoAdmin = useCallback(
    (adminId: string) => navigate({ to: '/ngos/admin/$ngoId/$adminId/view', params: { ngoId, adminId } }),
    [ngoId]
  );

  const navigateToEditNgoAdmin = useCallback(
    (adminId: string) => navigate({ to: '/ngos/admin/$ngoId/$adminId/edit', params: { ngoId, adminId } }),
    [ngoId]
  );

  const ngoAdminsColDefs: ColumnDef<NgoAdmin>[] = [
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
      accessorKey: 'phoneNumber',
      enableSorting: true,
      header: ({ column }) => <DataTableColumnHeader title='Phone number' column={column} />,
      cell: ({ row: { original } }) => original.phoneNumber || '-',
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
                <DropdownMenuItem onClick={() => navigateToViewNgoAdmin(adminId)}>View</DropdownMenuItem>
                <DropdownMenuItem onClick={() => navigateToEditNgoAdmin(adminId)}>Edit</DropdownMenuItem>
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
      </CardHeader>
      <CardContent>
        <QueryParamsDataTable
          columns={ngoAdminsColDefs}
          useQuery={(params) => useNgoAdmins(ngoId, params)}
          queryParams={queryParams}
          // onRowClick={(id) => navigateToViewNgoAdmin(id)}
        />
      </CardContent>
    </Card>
  );
};
