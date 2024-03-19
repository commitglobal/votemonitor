/* eslint-disable unicorn/prefer-top-level-await */
import type { ColumnDef } from '@tanstack/react-table';
import { z } from 'zod';
import { EllipsisVerticalIcon } from '@heroicons/react/24/solid';
import { Button } from '@/components/ui/button';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { ArrowUpDown } from 'lucide-react';
import { SortOrder } from '@/common/types';

export interface NGO {
  id: string;
  name: string;
  status: string;
}

export const ngoRouteSearchSchema = z.object({
  nameFilter: z.string().catch(''),
  pageNumber: z.number().catch(1),
  pageSize: z.number().catch(10),
  sortColumnName: z.string().catch(''),
  sortOrder: z.enum([SortOrder.asc, SortOrder.desc]).catch(SortOrder.asc),
  status: z.enum(['Active', 'Inactive']).catch('Active'),
});

export const ngoColDefs: ColumnDef<NGO>[] = [
  {
    header: 'ID',
    accessorKey: 'id',
    enableSorting: true,
  },
  {
    accessorKey: 'name',
    enableSorting: true,
    header: ({ column }) => {
      return (
        <Button variant='ghost' size='none' onClick={() => column.toggleSorting(column.getIsSorted() === 'asc')}>
          Name
          <ArrowUpDown className='w-4 h-4 ml-2' />
        </Button>
      );
    },
  },
  {
    header: 'Status',
    accessorKey: 'status',
  },
  {
    id: 'actions',
    cell: ({ row }) => {
      return (
        <div className='text-right'>
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button variant='ghost-primary' size='icon'>
                <span className='sr-only'>Open menu</span>
                <EllipsisVerticalIcon className='w-6 h-6' />
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align='end'>
              <DropdownMenuLabel>Actions</DropdownMenuLabel>
              <DropdownMenuItem onClick={() => navigator.clipboard.writeText(row.id)}>Copy row ID</DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem>View customer</DropdownMenuItem>
              <DropdownMenuItem>View payment details</DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      );
    },
  },
];
