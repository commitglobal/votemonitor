import { DataTableToolbar } from "@/components/data-table-toolbar";
import { DataTable } from "@/components/ui/data-table";
import { useDataTable } from "@/hooks/use-data-table";
import type { PageResponse } from "@/types/common";
import type { DataTableRowAction } from "@/types/data-table";
import type { ElectionModel } from "@/types/election";
import React from "react";
import { getElectionsTableColumns } from "./TableColumns";
import TableFilters from "./TableFilters";
export interface TableProps {
  data?: PageResponse<ElectionModel>;
}
function Table({ data }: TableProps) {
  const [rowAction, setRowAction] =
    React.useState<DataTableRowAction<ElectionModel> | null>(null);

  const columns = React.useMemo(
    () =>
      getElectionsTableColumns({
        setRowAction,
      }),
    [setRowAction]
  );

  const { table } = useDataTable({
    data: data?.items || [],
    columns,
    pageCount: data ? Math.ceil(data.totalCount / data.pageSize) : 0,
    initialState: {
      sorting: [{ id: "title", desc: false }],
      columnPinning: { right: ["actions"] },
    },
    getRowId: (originalRow) => originalRow.id,
  });

  return (
    <DataTable table={table}>
      <DataTableToolbar table={table}>
        <TableFilters table={table} />
      </DataTableToolbar>
    </DataTable>
  );
}

export default Table;
