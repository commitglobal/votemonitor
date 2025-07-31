import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { useDebounce } from "@/hooks/use-debounce";
import { listMonitoringElections } from "@/query-options/monitoring-elections";
import { Route } from "@/routes/(app)";
import { ElectionStatus } from "@/types/election";
import { useQuery } from "@tanstack/react-query";
import { ChevronDown, ChevronUp, X } from "lucide-react";
import { useMemo, useState } from "react";
import ElectionCard from "./components/ElectionCard";
import ElectionList from "./components/ElectionsList";

function Page() {
  const search = Route.useSearch();
  const debouncedSearch = useDebounce(search, 200);
  const { data } = useQuery(listMonitoringElections(debouncedSearch));

  const [countryFilter, setCountryFilter] = useState<string>("");
  const [statusFilter, setStatusFilter] = useState<string>("");

  const [filtersCollapsed, setFiltersCollapsed] = useState(false);

  // Get unique countries for filter dropdown
  const countries = useMemo(() => {
    const uniqueCountries = Array.from(
      new Set(data?.map((election) => election.countryName))
    ).sort();
    return uniqueCountries;
  }, [data]);

  // Get all status values for filter dropdown
  const statuses = Object.values(ElectionStatus);

  // Apply filters and sorting
  const filteredElections = useMemo(() => {
    let filtered = data;

    if (countryFilter) {
      filtered = filtered?.filter(
        (election) => election.countryName === countryFilter
      );
    }

    if (statusFilter) {
      filtered = filtered?.filter(
        (election) => election.status === statusFilter
      );
    }
    // Sort by start date descending (most recent first)
    return (
      filtered?.sort(
        (a, b) =>
          new Date(b.startDate).getTime() - new Date(a.startDate).getTime()
      ) ?? []
    );
  }, [data, countryFilter, statusFilter]);

  // Check if any filters are active
  const hasActiveFilters = countryFilter || statusFilter;

  // Filter elections into active and archived (only when no filters are active)
  const activeElections = useMemo(() => {
    if (hasActiveFilters) return [];
    return (
      data
        ?.filter(
          (election) =>
            election.status === ElectionStatus.NotStarted ||
            election.status === ElectionStatus.Started
        )
        .sort(
          (a, b) =>
            new Date(b.startDate).getTime() - new Date(a.startDate).getTime()
        ) ?? []
    );
  }, [data, hasActiveFilters]);

  const archivedElections = useMemo(() => {
    if (hasActiveFilters) return [];
    return (
      data
        ?.filter((election) => election.status === ElectionStatus.Archived)
        .sort(
          (a, b) =>
            new Date(b.startDate).getTime() - new Date(a.startDate).getTime()
        ) ?? []
    );
  }, [data, hasActiveFilters]);

  const clearFilters = () => {
    setCountryFilter("");
    setStatusFilter("");
  };

  return (
    <div className="container mx-auto p-6 max-w-4xl">
      <div className="mb-8">
        <h1 className="text-3xl font-bold tracking-tight">
          Monitored Elections
        </h1>
      </div>

      {/* Filters */}
      <div className="mb-8 space-y-4">
        <div className="flex items-center justify-between">
          <h3 className="text-lg font-medium">Filters</h3>
          <Button
            variant="ghost"
            size="sm"
            onClick={() => setFiltersCollapsed(!filtersCollapsed)}
            className="flex items-center gap-2"
          >
            {filtersCollapsed ? (
              <>
                Show Filters
                <ChevronDown className="h-4 w-4" />
              </>
            ) : (
              <>
                Hide Filters
                <ChevronUp className="h-4 w-4" />
              </>
            )}
          </Button>
        </div>

        {!filtersCollapsed && (
          <div className="space-y-4 p-4 border rounded-lg">
            <div className="flex flex-wrap gap-4 items-center">
              <div className="flex items-center gap-2">
                <label htmlFor="country-filter" className="text-sm font-  ">
                  Country:
                </label>
                <Select value={countryFilter} onValueChange={setCountryFilter}>
                  <SelectTrigger className="w-48" id="country-filter">
                    <SelectValue placeholder="All countries" />
                  </SelectTrigger>
                  <SelectContent>
                    {countries.map((country) => (
                      <SelectItem key={country} value={country}>
                        {country}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              <div className="flex items-center gap-2">
                <label htmlFor="status-filter" className="text-sm font-medium">
                  Status:
                </label>
                <Select value={statusFilter} onValueChange={setStatusFilter}>
                  <SelectTrigger className="w-40" id="status-filter">
                    <SelectValue placeholder="All statuses" />
                  </SelectTrigger>
                  <SelectContent>
                    {statuses.map((status) => (
                      <SelectItem key={status} value={status}>
                        {status}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
            </div>

            <div className="flex flex-wrap gap-6 items-center">
              {hasActiveFilters && (
                <Button
                  variant="outline"
                  size="sm"
                  onClick={clearFilters}
                  className="flex items-center gap-2 bg-transparent"
                >
                  <X className="h-4 w-4" />
                  Clear All Filters
                </Button>
              )}
            </div>
          </div>
        )}

        {hasActiveFilters && (
          <div className="text-sm text-muted-foreground">
            Showing {filteredElections.length} election
            {filteredElections.length !== 1 ? "s" : ""}
            {countryFilter && ` in ${countryFilter}`}
            {statusFilter && ` with status ${statusFilter}`}
          </div>
        )}
      </div>

      {/* Election Lists */}
      {hasActiveFilters ? (
        <div className="space-y-4">
          <h2 className="text-2xl font-semibold tracking-tight">
            Filtered Elections
          </h2>
          {filteredElections.length === 0 ? (
            <div className="text-center py-8 text-muted-foreground">
              No elections match the selected filters.
            </div>
          ) : (
            <div className="space-y-4">
              {filteredElections?.map((election) => (
                <ElectionCard key={election.id} election={election} />
              ))}
            </div>
          )}
        </div>
      ) : (
        <div className="space-y-12">
          <ElectionList elections={activeElections} title="Active Elections" />
          <ElectionList
            elections={archivedElections}
            title="Archived Elections"
          />
        </div>
      )}
    </div>
  );
}

export default Page;
