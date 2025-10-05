import CoalitionMemberFilter from "@/components/CoalitionMemberFilter";
import { SingleSelectDataTableFacetedFilter } from "@/components/data-table-faceted-filter";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { useDebouncedCallback } from "@/hooks/use-debounced-callback";
import {
  mapQuickReportFollowUpStatus,
  mapQuickReportIncidentCategory,
  mapQuickReportLocationType,
} from "@/lib/i18n";
import { Route } from "@/routes/(app)/elections/$electionRoundId/quick-reports";
import { DataSource } from "@/types/common";
import type { Option } from "@/types/data-table";
import {
  QuickReportFollowUpStatus,
  QuickReportFollowUpStatusList,
  QuickReportIncidentCategory,
  QuickReportIncidentCategoryList,
  QuickReportLocationType,
  QuickReportLocationTypeList,
} from "@/types/quick-reports";
import { X } from "lucide-react";
import React from "react";

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

function TableFilters() {
  const search = Route.useSearch();
  const navigate = Route.useNavigate();

  const isFiltered = Object.entries(search).some(
    ([key, value]) =>
      !["pageNumber", "pageSize"].includes(key) &&
      Boolean(value) &&
      !(key === "dataSource" && value === DataSource.Ngo)
  );

  const onReset = React.useCallback(() => {
    navigate({
      search: {
        pageNumber: 1,
        pageSize: 25,
      },
      replace: true,
    });
  }, [navigate]);

  const [searchInput, setSearchInput] = React.useState(search.searchText ?? "");

  React.useEffect(() => {
    setSearchInput(search.searchText ?? "");
  }, [search.searchText]);

  const debouncedSearch = useDebouncedCallback((value: string) => {
    navigate({
      search: (prev) => ({ ...prev, searchText: value }),
      replace: true,
    });
  }, 500);

  const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSearchInput(event.target.value);
    debouncedSearch(event.target.value);
  };

  return (
    <div className="flex flex-1 flex-wrap items-center gap-2">
      <Input
        placeholder="Search"
        value={searchInput}
        onChange={handleInputChange}
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
              quickReportFollowUpStatus: value as QuickReportFollowUpStatus,
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
