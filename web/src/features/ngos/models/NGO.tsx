/* eslint-disable unicorn/prefer-top-level-await */
import type { ColumnDef } from '@tanstack/react-table';
import { EllipsisVerticalIcon } from '@heroicons/react/24/solid';
import { Button } from '@/components/ui/button';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { useNavigate } from '@tanstack/react-router';

export interface NGO {
  id: string;
  name: string;
  status: string;
}

export const ngoColDefs: ColumnDef<NGO>[] = [
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
              <DropdownMenuItem onClick={() =>  navigate({ to: '/ngos/$ngoId', params: { ngoId: row.original.id } })}>Edit</DropdownMenuItem>
              <DropdownMenuItem>Deactivate</DropdownMenuItem>
              <DropdownMenuItem>Delete</DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      );
    },
  },
];
