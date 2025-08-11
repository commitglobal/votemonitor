import { DataTableToolbar } from "@/components/data-table-toolbar";
import { DataTable } from "@/components/ui/data-table";
import { useDataTable } from "@/hooks/use-data-table";
import { useDebounce } from "@/hooks/use-debounce";
import { useQuickReports } from "@/queries/quick-reports";
import { Route } from "@/routes/(app)/elections/$electionRoundId/incidents";
import type { DataTableRowAction } from "@/types/data-table";
import type { QuickReportModel } from "@/types/quick-reports";
import React from "react";
import TableFilters from "./Filters";
import { getQuickReportsTableColumns } from "./TableColumns";

export default function Table() {
  const { electionRoundId } = Route.useParams();
  const search = Route.useSearch();
  const debouncedSearch = useDebounce(search, 200);
  const { data } = useQuickReports(electionRoundId, debouncedSearch);

  const [rowAction, setRowAction] =
    React.useState<DataTableRowAction<QuickReportModel> | null>(null);

  const columns = React.useMemo(
    () =>
      getQuickReportsTableColumns({
        electionRoundId,
        setRowAction,
      }),
    [electionRoundId, setRowAction]
  );

  const { table } = useDataTable({
    data: data?.items || [],
    columns,
    pageCount: data ? Math.ceil(data.totalCount / data.pageSize) : 0,
    initialState: {
      sorting: [{ id: "timestamp", desc: true }],
      columnPinning: { right: ["action"] },
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
