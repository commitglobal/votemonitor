import {
  SingleSelectDataTableFacetedFilter,
  MultiSelectDataTableFacetedFilter,
} from "@/components/data-table-faceted-filter";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { listMonitoringObserversTagsQueryOptions } from "@/queries/monitoring-observers";
import type { Option } from "@/types/data-table";
import {
  MonitoringObserverStatus,
  type MonitoringObserverModel,
} from "@/types/monitoring-observer";
import { useQuery } from "@tanstack/react-query";
import { getRouteApi } from "@tanstack/react-router";
import type { Table } from "@tanstack/react-table";
import { X } from "lucide-react";
import React, { useMemo } from "react";

interface DataTableToolbarProps extends React.ComponentProps<"div"> {
  table: Table<MonitoringObserverModel>;
}

const monitoringObserverStatusOptions: Option[] = [
  {
    value: MonitoringObserverStatus.Active,
    label: MonitoringObserverStatus.Active,
  },

  {
    value: MonitoringObserverStatus.Pending,
    label: MonitoringObserverStatus.Pending,
  },

  {
    value: MonitoringObserverStatus.Suspended,
    label: MonitoringObserverStatus.Suspended,
  },
];

const route = getRouteApi(
  "/(app)/elections/$electionRoundId/observers/" as const
);

function TableFilters({ table }: DataTableToolbarProps) {
  const { electionRoundId } = route.useParams();
  const search = route.useSearch();
  const navigate = route.useNavigate();

  const { data: tags } = useQuery(
    listMonitoringObserversTagsQueryOptions(electionRoundId)
  );

  const tagsOptions = useMemo(
    () => tags?.map((t) => ({ value: t, label: t })) ?? [],
    [tags]
  );

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

      <SingleSelectDataTableFacetedFilter
        title="Observer status"
        options={monitoringObserverStatusOptions}
        value={search.status as string}
        onValueChange={(value) =>
          navigate({
            search: (prev) => ({
              ...prev,
              status: value as MonitoringObserverStatus,
            }),
            replace: true,
          })
        }
      />

      <MultiSelectDataTableFacetedFilter
        title="Tags"
        options={tagsOptions}
        value={search.tags}
        onValueChange={(value) =>
          navigate({
            search: (prev) => ({
              ...prev,
              tags: value,
            }),
            replace: true,
          })
        }
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
