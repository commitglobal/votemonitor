import { DataTableToolbar } from "@/components/data-table-toolbar";
import { DataTable } from "@/components/ui/data-table";
import { useDataTable } from "@/hooks/use-data-table";
import type { PageResponse } from "@/types/common";
import type { DataTableRowAction } from "@/types/data-table";
import type { MonitoringObserverModel } from "@/types/monitoring-observer";
import React from "react";
import { getMonitoringObserversTableColumns } from "./TableColumns";
import TableFilters from "./TableFilters";
import { Route } from "@/routes/(app)/elections/$electionRoundId/observers";
export interface TableProps {
  data?: PageResponse<MonitoringObserverModel>;
}
function Table({ data }: TableProps) {
  const [rowAction, setRowAction] =
    React.useState<DataTableRowAction<MonitoringObserverModel> | null>(null);

  const search = Route.useSearch();
  const navigate = Route.useNavigate();
  const columns = React.useMemo(
    () =>
      getMonitoringObserversTableColumns({
        setRowAction,
      }),
    [setRowAction]
  );

  const { table } = useDataTable({
    data: data?.items || [],
    columns,
    pageCount: data ? Math.ceil(data.totalCount / data.pageSize) : 0,
    initialState: {
      sorting: [{ id: "displayName", desc: false }],
      columnPinning: { right: ["actions"] },
    },
    getRowId: (originalRow) => originalRow.id,
    search,
    navigate,
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
