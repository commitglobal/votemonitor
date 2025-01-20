/* eslint-disable unicorn/prefer-top-level-await */
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { EllipsisVerticalIcon } from '@heroicons/react/24/solid';
import { useNavigate } from '@tanstack/react-router';
import type { ColumnDef } from '@tanstack/react-table';

export interface NGO {
  id: string;
  name: string;
  status: NGOStatus;
}

export enum NGOStatus {
  Activated = 'Activated',
  Pending = 'Pending',
  Deactivated = 'Deactivated',
}

export interface NGOAdmin {
  id: string;
  email: string;
  firstName: string;
  lastName: string;

  status: NGOStatus;
}

export const ngoAdminsColDefs: ColumnDef<NGOAdmin>[] = [
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
    }) => <Badge className={`badge-${status}`}>{status}</Badge>,
  },
  {
    id: 'actions',
    cell: ({ row }) => {
      const navigate = useNavigate();

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
              <DropdownMenuItem>Deactivate</DropdownMenuItem>
              <DropdownMenuItem>Delete</DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      );
    },
  },
];
