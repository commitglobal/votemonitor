import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { Button } from '@/components/ui/button';
import { EllipsisVerticalIcon } from '@heroicons/react/24/outline';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Link } from '@tanstack/react-router';
import type { ColumnDef } from '@tanstack/react-table';
import { z } from 'zod';

export interface ElectionRound {
  id: string;
  countryId: string;
  country: string;
  title: string;
  englishTitle: string;
  startDate: string;
  status: 'Archived' | 'NotStarted' | 'Started';
  createdOn: string;
  lastModifiedOn: string;
}

// TODO: figure out schema
export const electionRoundFormSchema = z.object({
  title: z.string().min(2).max(255),
  englishTitle: z.string().min(2).max(255),
  countryId: z.string(), // TODO: validate it exists in the database
  startDate: z.date(),
});

export type ElectionRoundFormValues = z.infer<typeof electionRoundFormSchema>;


export const electionRoundColDefs: ColumnDef<ElectionRound>[] = [
  {
    header: 'ID',
    accessorKey: 'id',
  },
  {
    accessorKey: 'title',
    enableSorting: true,
    header: ({ column }) => <DataTableColumnHeader title='Title' column={column} />,
  },
  {
    accessorKey: 'englishTitle',
    enableSorting: true,
    header: ({ column }) => <DataTableColumnHeader title='English title' column={column} />,
  },
  {
    accessorKey: 'country',
    enableSorting: true,
    header: ({ column }) => <DataTableColumnHeader title='Country' column={column} />,
  },
  {
    accessorKey: 'startDate',
    enableSorting: true,
    header: ({ column }) => <DataTableColumnHeader title='Start date' column={column} />,
  },
  {
    accessorKey: 'status',
    enableSorting: false,
    header: ({ column }) => <DataTableColumnHeader title='Status' column={column} />,
  },
  {
    id: 'actions',
    cell: ({ row }) => {
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
              <Link
                to='/election-rounds/$electionRoundId'
                params={{ electionRoundId: row.original.id }}
                preload='intent'>
                <DropdownMenuItem>Edit</DropdownMenuItem>
              </Link>
              <DropdownMenuItem>Deactivate</DropdownMenuItem>
              <DropdownMenuItem>Delete</DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      );
    },
  },
];
