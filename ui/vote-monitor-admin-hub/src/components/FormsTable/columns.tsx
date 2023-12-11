import { ColumnDef } from "@tanstack/react-table"

import { DataTableColumnHeader } from "./data-table-column-header"
import { DataTableRowActions } from "./data-table-row-actions"
import { FormModel } from "@/redux/api/types"

export const columns: ColumnDef<FormModel>[] = [

  {
    accessorKey: "code",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Code" />
    ),
    cell: ({ row }) => <div className="w-[80px]">{row.getValue("code")}</div>
  },
  {
    accessorKey: "languageCode",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Language Code" />
    ),
    cell: ({ row }) => <div className="w-[80px]">{row.getValue("languageCode")}</div>
  },
  {
    accessorKey: "status",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Status" />
    ),
    cell: ({ row }) => <div className="w-[80px]">{row.getValue("status")}</div>
  },
  {
    accessorKey: "description",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Description" />
    ),
    cell: ({ row }) => <div className="w-[80px]">{row.getValue("description")}</div>
  },
  {
    id: "actions",
    cell: ({ row }) => <DataTableRowActions row={row} />,
  },
]
