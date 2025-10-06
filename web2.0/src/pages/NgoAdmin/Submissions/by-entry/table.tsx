import { DataTableSkeleton } from "@/components/data-table-skeleton";
import { useDataTable } from "@/hooks/use-data-table";
import { useListFormSubmissions } from "@/queries/form-submissions";
import { Route } from "@/routes/(app)/elections/$electionRoundId/submissions";
import React from "react";
import { getFormSubmissionsColumns } from "./columns";
import { DataTable } from "@/components/ui/data-table";
import { DataTableToolbar } from "@/components/data-table-toolbar";
import { TableFilters } from "./filters";

export function Table() {
  const { electionRoundId } = Route.useParams();
  const search = Route.useSearch();
  const navigate = Route.useNavigate();
  const { data, isPending } = useListFormSubmissions(electionRoundId, search);

  const columns = React.useMemo(
    () => getFormSubmissionsColumns(electionRoundId),
    [electionRoundId]
  );

  const { table } = useDataTable({
    tableName: "submissions-by-entry",
    data: data?.items || [],
    columns,
    pageCount: data ? Math.ceil(data.totalCount / data.pageSize) : 0,
    initialState: {
      sorting: [{ id: "timeSubmitted", desc: true }],
      columnPinning: { right: ["actions"] },
    },
    columnFilters: [
      {
        columnId: "locationType",
        searchKey: "locationType",
        type: "string",
      },
      {
        columnId: "incidentCategory",
        searchKey: "incidentCategory",
        type: "string",
      },
      {
        columnId: "followUpStatus",
        searchKey: "followUpStatus",
        type: "string",
      },
      {
        columnId: "coalitionMemberId",
        searchKey: "coalitionMemberId",
        type: "string",
      },
    ],
    getRowId: (originalRow) => originalRow.submissionId,
    search,
    navigate,
  });

  if (isPending) {
    return (
      <DataTableSkeleton
        columnCount={columns.length}
        filterCount={1}
        withViewOptions={true}
        withPagination={true}
      />
    );
  }
  return (
    <DataTable table={table}>
      <DataTableToolbar table={table}>
        <TableFilters />
      </DataTableToolbar>
    </DataTable>
  );
}
