"use client";

import type {DataTableRowAction} from "@/types/data-table";
import type {ColumnDef} from "@tanstack/react-table";
import {Ellipsis} from "lucide-react";
import * as React from "react";

import {DataTableColumnHeader} from "@/components/data-table-column-header";
import {Button} from "@/components/ui/button";
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import type {GuidesObserverModel} from "@/types/guides-observer.ts";

interface GetTasksTableColumnsProps {
    setRowAction: React.Dispatch<
        React.SetStateAction<DataTableRowAction<GuidesObserverModel> | null>
    >;
}

export function getGuidesObserversTableColumns({
                                                   setRowAction,
                                               }: GetTasksTableColumnsProps): ColumnDef<GuidesObserverModel>[] {
    return [
        {
            id: "title",
            accessorKey: "title",
            header: ({column}) => (
                <DataTableColumnHeader column={column} title="Title"/>
            ),
            cell: ({row}) => (
                <div className="truncate">{row.original.title}</div>
            ),
            meta: {
                label: "Title",
            },
            enableSorting: true,
            enableHiding: true,
        },
        {
            id: "createdOn",
            accessorKey: "createdOn",
            header: ({column}) => (
                <DataTableColumnHeader column={column} title="Uploaded On"/>
            ),
            cell: ({row}) => <div className="truncate">{row.original.createdOn}</div>,
            meta: {
                label: "Uploaded On",
            },
            enableSorting: true,
            enableHiding: true,
        },
        {
            id: "createdBy",
            accessorKey: "createdBy",
            header: ({ column }) => (
                <DataTableColumnHeader column={column} title="Created By" />
            ),
            cell: ({ row }) => <div className="truncate">{row.original.createdBy}</div>,
            meta: {
                label: "Created By",
            },
            enableSorting: true,
            enableHiding: true,
        },
        {
            id: "actions",
            cell: function Cell({row}) {
                return (
                    <DropdownMenu>
                        <DropdownMenuTrigger asChild>
                            <Button
                                aria-label="Open menu"
                                variant="ghost"
                                className="flex size-8 p-0 data-[state=open]:bg-muted"
                            >
                                <Ellipsis className="size-4" aria-hidden="true"/>
                            </Button>
                        </DropdownMenuTrigger>
                        <DropdownMenuContent align="end" className="w-40">
                            <DropdownMenuItem
                                onSelect={() => setRowAction({row, variant: "update"})}
                            >
                                Edit
                            </DropdownMenuItem>

                            <DropdownMenuSeparator/>
                            <DropdownMenuItem
                                onSelect={() => setRowAction({row, variant: "delete"})}
                            >
                                Delete
                            </DropdownMenuItem>
                        </DropdownMenuContent>
                    </DropdownMenu>
                );
            },
            size: 40,
        },
    ];
}
