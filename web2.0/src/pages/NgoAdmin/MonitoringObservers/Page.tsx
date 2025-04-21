import { DataTable } from "@/components/ui/data-table";
import { useDataTable } from "@/hooks/use-data-table";
import { listMonitoringObserversQueryOptions } from "@/query-options/monitoring-observers";
import type { DataTableRowAction } from "@/types/data-table";
import type { MonitoringObserver } from "@/types/monitoring-observers";
import { useSuspenseQuery } from "@tanstack/react-query";
import React from "react";
import { getMonitoringObserversTableColumns } from "./components/columns";
import { Route } from "@/routes/(app)/elections/$electionRoundId/observers";

function Page() {
  const { electionRoundId } = Route.useParams();
  const search = Route.useSearch();
  const { data } = useSuspenseQuery(
    listMonitoringObserversQueryOptions(electionRoundId, search)
  );
  const [rowAction, setRowAction] =
    React.useState<DataTableRowAction<MonitoringObserver> | null>(null);

  const columns = React.useMemo(
    () =>
      getMonitoringObserversTableColumns({
        setRowAction,
      }),
    [setRowAction]
  );

  const { table } = useDataTable({
    data: data.items,
    columns,
    pageCount: Math.ceil(data.totalCount / data.pageSize),
    initialState: {
      sorting: [{ id: "displayName", desc: false }],
      columnPinning: { right: ["actions"] },
    },
    getRowId: (originalRow) => originalRow.id,
    clearOnDefault: true,
  });
  return (
    <>
      <DataTable table={table}></DataTable>

      {/* <DeleteTasksDialog
        open={rowAction?.variant === "delete"}
        onOpenChange={() => setRowAction(null)}
        tasks={rowAction?.row.original ? [rowAction?.row.original] : []}
        showTrigger={false}
        onSuccess={() => rowAction?.row.toggleSelected(false)}
      /> */}
    </>
  );
}

export default Page;
