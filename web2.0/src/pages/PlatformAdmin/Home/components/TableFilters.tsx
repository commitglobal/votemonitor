import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { type ElectionModel } from "@/types/election";
import { getRouteApi } from "@tanstack/react-router";
import type { Table } from "@tanstack/react-table";
import { X } from "lucide-react";
import React from "react";

interface DataTableToolbarProps extends React.ComponentProps<"div"> {
  table: Table<ElectionModel>;
}

// const electionRoundStatusOptions: Option[] = [
//   {
//     value: ElectionRoundStatus.NotStarted,
//     label: ElectionRoundStatus.NotStarted,
//   },

//   {
//     value: ElectionRoundStatus.Started,
//     label: ElectionRoundStatus.Started,
//   },

//   {
//     value: ElectionRoundStatus.Archived,
//     label: ElectionRoundStatus.Archived,
//   },
// ];

const route = getRouteApi("/(app)/");

function TableFilters({ table }: DataTableToolbarProps) {
  const search = route.useSearch();
  const navigate = route.useNavigate();

  const isFiltered = table.getState().columnFilters.length > 0;

  //   const onReset = React.useCallback(() => {
  //     table.resetColumnFilters();
  //   }, [table]);

  const onReset = React.useCallback(() => {
    console.log("reset");
  }, []);

  return (
    <div className="flex flex-1 flex-wrap items-center gap-2">
      <Input
        placeholder="Search"
        value={search.searchText ?? ""}
        onChange={(event) =>
          navigate({
            search: (prev) => ({ ...prev, searchText: event.target.value }),
            replace: true,
          })
        }
        className="h-8 w-40 lg:w-56"
      />

      {isFiltered && (
        <Button
          aria-label="Reset filters"
          variant="outline"
          size="sm"
          className="border-dashed"
          onClick={onReset}
        >
          <X />
          Reset
        </Button>
      )}
    </div>
  );
}

export default TableFilters;
