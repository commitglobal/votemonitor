import { Button } from '@/components/ui/button';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { EllipsisVerticalIcon } from '@heroicons/react/24/outline';
import { Link } from '@tanstack/react-router';
import { ColumnDef, createColumnHelper } from '@tanstack/react-table';
import { ElectionRoundModel } from '../../models/types';
import { cn } from '@/lib/utils';
import { Badge } from '@/components/ui/badge';
import { ElectionRoundStatus } from '@/common/types';
import ElectionRoundStatusBadge from '../../../../components/ElectionRoundStatusBadge/ElectionRoundStatusBadge';

const columnHelper = createColumnHelper<ElectionRoundModel>();

export const electionRoundColDefs: ColumnDef<ElectionRoundModel>[] = [
  columnHelper.display({
    id: 'title',
    enableResizing: true,
    enableSorting: true,
    header: ({ column }) => <DataTableColumnHeader title='Title' column={column} />,
    cell: ({ row }) => row.original.title,
  }),
  columnHelper.display({
    id: 'englishTitle',
    enableResizing: true,
    enableSorting: true,
    header: ({ column }) => <DataTableColumnHeader title='English title' column={column} />,
    cell: ({ row }) => row.original.englishTitle,
  }),
  columnHelper.display({
    id: 'countryName',
    enableSorting: true,
    header: ({ column }) => <DataTableColumnHeader title='Country' column={column} />,
    cell: ({ row }) => row.original.countryName,
  }),
  columnHelper.display({
    id: 'startDate',
    enableSorting: true,
    header: ({ column }) => <DataTableColumnHeader title='Start date' column={column} />,
    cell: ({ row }) => row.original.startDate,
  }),
  columnHelper.display({
    id: 'status',
    enableSorting: true,
    header: ({ column }) => <DataTableColumnHeader title='Status' column={column} />,
    cell: ({
      row: {
        original: { status },
      },
    }) => <ElectionRoundStatusBadge status={status} />,
  }),
  columnHelper.display({
    id: 'numberOfNgosMonitoring',
    enableSorting: true,
    header: ({ column }) => <DataTableColumnHeader title='NGOs' column={column} />,
    cell: ({ row }) => row.original.numberOfNgosMonitoring,
  }),
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
              <DropdownMenuItem>Archive</DropdownMenuItem>
              <DropdownMenuItem className='text-red-600'>Delete</DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      );
    },
  },
];
