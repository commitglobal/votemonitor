import { DataTableColumnHeader } from "@/components/ui/DataTable/DataTableColumnHeader";
import { Button } from "@/components/ui/button";
import { EllipsisVerticalIcon } from "@heroicons/react/24/outline";
import { DropdownMenu, DropdownMenuTrigger, DropdownMenuContent, DropdownMenuItem } from "@radix-ui/react-dropdown-menu";
import { Link, useNavigate } from "@tanstack/react-router";
import { ColumnDef } from "@tanstack/react-table";

export interface ElectionRound {
    id: string,
    countryId: string,
    country: string,
    title: string,
    englishTitle: string,
    startDate: string,
    status: "Archived" | "NotStarted" | "Started",
    createdOn: string,
    lastModifiedOn: string
}


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
                            <DropdownMenuItem onClick={() => navigate({ to: '/election-rounds/$electionRoundId', params: { electionRoundId: row.original.id } })}>Edit</DropdownMenuItem>
                            <DropdownMenuItem>Deactivate</DropdownMenuItem>
                            <DropdownMenuItem>Delete</DropdownMenuItem>
                        </DropdownMenuContent>
                    </DropdownMenu>
                </div>
            );
        },
    },
];
