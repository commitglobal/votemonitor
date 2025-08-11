import CoalitionMemberFilter from "@/components/CoalitionMemberFilter";
import { SingleSelectDataTableFacetedFilter } from "@/components/data-table-faceted-filter";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  mapQuickReportFollowUpStatus,
  mapQuickReportIncidentCategory,
  mapQuickReportLocationType,
} from "@/lib/i18n";
import { Route } from "@/routes/(app)/elections/$electionRoundId/incidents";
import type { Option } from "@/types/data-table";
import {
  QuickReportFollowUpStatus,
  QuickReportFollowUpStatusList,
  QuickReportIncidentCategory,
  QuickReportIncidentCategoryList,
  QuickReportLocationType,
  QuickReportLocationTypeList,
  type QuickReportModel,
} from "@/types/quick-reports";
import type { Table } from "@tanstack/react-table";
import { X } from "lucide-react";
import React from "react";

interface DataTableToolbarProps extends React.ComponentProps<"div"> {
  table: Table<QuickReportModel>;
}

const locationOptions: Option[] = QuickReportLocationTypeList.map((lt) => ({
  label: mapQuickReportLocationType(lt),
  value: lt,
}));

const quickReportIncidentCategoryOptions: Option[] =
  QuickReportIncidentCategoryList.map((qic) => ({
    label: mapQuickReportIncidentCategory(qic),
    value: qic,
  }));

const quickReportFollowUpStatusOptions: Option[] =
  QuickReportFollowUpStatusList.map((fs) => ({
    label: mapQuickReportFollowUpStatus(fs),
    value: fs,
  }));

function TableFilters({ table }: DataTableToolbarProps) {
  const search = Route.useSearch();
  const navigate = Route.useNavigate();

  const isFiltered = table.getState().columnFilters.length > 0;

  const onReset = React.useCallback(() => {
    navigate({
      search: {
        pageNumber: 1,
        pageSize: 25,
      },
    });
  }, [navigate]);

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

      <CoalitionMemberFilter />

      <SingleSelectDataTableFacetedFilter
        title="Location type"
        options={locationOptions}
        value={search.quickReportLocationType as string}
        onValueChange={(value) =>
          navigate({
            search: (prev) => ({
              ...prev,
              quickReportLocationType: value as QuickReportLocationType,
            }),
            replace: true,
          })
        }
      />

      <SingleSelectDataTableFacetedFilter
        title="Incident category type"
        options={quickReportIncidentCategoryOptions}
        value={search.incidentCategory as string}
        onValueChange={(value) =>
          navigate({
            search: (prev) => ({
              ...prev,
              incidentCategory: value as QuickReportIncidentCategory,
            }),
            replace: true,
          })
        }
      />

      <SingleSelectDataTableFacetedFilter
        title="Followup status"
        options={quickReportFollowUpStatusOptions}
        value={search.quickReportFollowUpStatus as string}
        onValueChange={(value) =>
          navigate({
            search: (prev) => ({
              ...prev,
              followUpStatus: value as QuickReportFollowUpStatus,
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
