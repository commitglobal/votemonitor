import { SingleSelectDataTableFacetedFilter } from "@/components/data-table-faceted-filter";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Separator } from "@/components/ui/separator";
import { Skeleton } from "@/components/ui/skeleton";
import { useDebounce } from "@/hooks/use-debounce";
import { useListMonitoringElections } from "@/queries/monitoring-elections";
import { Route } from "@/routes/(app)";
import { SortOrder } from "@/types/common";
import { ElectionRoundStatus } from "@/types/election";
import countries from "i18n-iso-countries";
import { ArrowDownAZ, ArrowUpAZ, SlidersHorizontal, X } from "lucide-react";
import { useEffect, useMemo, useState, type ChangeEvent } from "react";
import { useTranslation } from "react-i18next";
import { ElectionRoundCard } from "./components/ElectionCard";

function Page() {
  const {
    searchText = "",
    electionRoundStatus: initElectionRoundStatus,
    sortOrder: initSort = SortOrder.Asc,
    countryId: initCountry,
  } = Route.useSearch();

  const navigate = Route.useNavigate();
  const { data, isPending } = useListMonitoringElections();
  const [sort, setSort] = useState(initSort);
  const [electionRoundStatus, setElectionRoundStatus] = useState(
    initElectionRoundStatus
  );
  const [country, setCountry] = useState(initCountry);
  const [searchTerm, setSearchTerm] = useState(searchText);
  const debouncedSearchTerm = useDebounce(searchTerm, 300);

  const { i18n } = useTranslation();

  const totalCount = Array.isArray(data) ? data.length : 0;

  // Get unique countries for filter dropdown
  const countriesOptions = useMemo(() => {
    var countriesMap = new Map<string, string>();
    data?.forEach((election) =>
      countriesMap.set(election.countryId, election.countryIso2)
    );

    return Array.from(countriesMap.entries()).map(([value, iso2]) => ({
      value,
      label:
        countries.getName(iso2, i18n.language.split("-")[0], {
          select: "official",
        }) ?? iso2,
    }));
  }, [data, i18n.language]);

  const filteredElections = useMemo(
    () =>
      data
        ?.sort((a, b) =>
          sort === SortOrder.Asc
            ? a.startDate.localeCompare(b.startDate)
            : b.startDate.localeCompare(a.startDate)
        )
        .filter((election) =>
          electionRoundStatus === "Started"
            ? election.status === ElectionRoundStatus.Started
            : electionRoundStatus === "NotStarted"
            ? election.status === ElectionRoundStatus.NotStarted
            : true
        )
        .filter(
          (election) =>
            election.title
              .toLowerCase()
              .includes(debouncedSearchTerm.toLowerCase()) ||
            election.englishTitle
              .toLowerCase()
              .includes(debouncedSearchTerm.toLowerCase())
        )
        .filter((election) =>
          !country ? true : election.countryId === country
        ),
    [data, sort, electionRoundStatus, debouncedSearchTerm, country]
  );

  const handleSearch = (e: ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(e.target.value);
  };

  useEffect(() => {
    navigate({
      search: (prev) => ({
        ...prev,
        filter: debouncedSearchTerm || undefined,
      }),
    });
  }, [debouncedSearchTerm, navigate]);
  const handleSortChange = (sort: SortOrder) => {
    setSort(sort as SortOrder);
    navigate({ search: (prev) => ({ ...prev, sort }) });
  };

  const handleElectionRoundStatusChange = (value: string | undefined) => {
    setElectionRoundStatus(value as ElectionRoundStatus);
    navigate({
      search: (prev) => ({
        ...prev,
        electionRoundStatus: value as ElectionRoundStatus | undefined,
      }),
    });
  };

  const handleCountryChange = (value: string | undefined) => {
    setCountry(value as string);
    navigate({ search: (prev) => ({ ...prev, countryId: value }) });
  };

  const handleResetFilters = () => {
    setSearchTerm("");
    setElectionRoundStatus(undefined as unknown as ElectionRoundStatus);
    setCountry("");
    navigate({
      search: (prev) => ({
        ...prev,
        filter: undefined,
        electionRoundStatus: undefined,
        countryId: undefined,
      }),
    });
  };

  return (
    <>
      <div>
        <h1 className="text-2xl font-bold tracking-tight">
          Monitored Elections
        </h1>
        <p className="text-muted-foreground">
          Here&apos;s a list of your monitored elections!
        </p>
      </div>
      <div className="my-4 flex items-end justify-between sm:my-0 sm:items-center">
        <div className="flex flex-col gap-4 sm:my-4 sm:flex-row">
          <Input
            placeholder="Filter elections..."
            className="h-9 w-40 lg:w-[250px]"
            value={searchTerm}
            onChange={handleSearch}
          />
          <SingleSelectDataTableFacetedFilter
            title={"Election Round Status"}
            options={[
              {
                value: ElectionRoundStatus.Started,
                label: ElectionRoundStatus.Started,
              },
              {
                value: ElectionRoundStatus.Archived,
                label: ElectionRoundStatus.Archived,
              },
            ]}
            value={electionRoundStatus ?? ""}
            onValueChange={handleElectionRoundStatusChange}
          />
          <SingleSelectDataTableFacetedFilter
            title={"Country"}
            options={countriesOptions}
            value={country ?? ""}
            onValueChange={handleCountryChange}
          />
          {(country || electionRoundStatus || searchTerm) && (
            <Button
              aria-label="Reset filters"
              variant="outline"
              size="sm"
              className="border-dashed"
              onClick={handleResetFilters}
            >
              <X />
              Reset
            </Button>
          )}
        </div>

        <Select value={sort} onValueChange={handleSortChange}>
          <SelectTrigger className="w-16">
            <SelectValue>
              <SlidersHorizontal size={18} />
            </SelectValue>
          </SelectTrigger>
          <SelectContent align="end">
            <SelectItem value={SortOrder.Asc}>
              <div className="flex items-center gap-4">
                <ArrowUpAZ size={16} />
                <span>Ascending</span>
              </div>
            </SelectItem>
            <SelectItem value={SortOrder.Desc}>
              <div className="flex items-center gap-4">
                <ArrowDownAZ size={16} />
                <span>Descending</span>
              </div>
            </SelectItem>
          </SelectContent>
        </Select>
      </div>
      <Separator className="shadow-sm" />

      {/* Pending state */}
      {isPending && totalCount === 0 ? (
        <div className="grid gap-4 pt-4 pb-16 md:grid-cols-2 lg:grid-cols-3">
          {Array.from({ length: 6 }).map((_, i) => (
            <div key={i} className="rounded-lg border p-4 space-y-3">
              <div className="flex items-center justify-between">
                <Skeleton className="h-10 w-16 rounded-md" />
                <Skeleton className="h-6 w-24" />
              </div>
              <Skeleton className="h-6 w-3/4" />
              <Skeleton className="h-4 w-1/2" />
              <div className="flex items-center gap-2">
                <Skeleton className="h-4 w-24" />
                <Skeleton className="h-6 w-28" />
              </div>
            </div>
          ))}
        </div>
      ) : totalCount === 0 ? (
        <div className="pt-8 pb-16 text-center text-muted-foreground">
          <p className="text-sm">No monitored elections available.</p>
        </div>
      ) : filteredElections && filteredElections.length === 0 ? (
        // Empty state: filters yield no results
        <div className="pt-8 pb-16 text-center">
          <p className="mb-3 text-sm text-muted-foreground">
            No elections match your current filters.
          </p>
          <Button variant="secondary" onClick={handleResetFilters}>
            Reset filters
          </Button>
        </div>
      ) : (
        <div className="faded-bottom no-scrollbar grid gap-4 overflow-auto pt-4 pb-16 md:grid-cols-2 lg:grid-cols-3">
          {filteredElections?.map((electionRound) => (
            <ElectionRoundCard
              key={electionRound.id}
              electionRound={electionRound}
            />
          ))}
        </div>
      )}
    </>
  );
}

export default Page;
