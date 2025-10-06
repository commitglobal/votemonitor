import { SingleSelectDataTableFacetedFilter } from "@/components/data-table-faceted-filter";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Separator } from "@/components/ui/separator";
import { useDebounce } from "@/hooks/use-debounce";
import { useListMonitoringElections } from "@/queries/monitoring-elections";
import { Route } from "@/routes/(app)";
import { SortOrder } from "@/types/common";
import { ElectionRoundStatus } from "@/types/election";
import countries from "i18n-iso-countries";
import { ArrowDownAZ, ArrowUpAZ, SlidersHorizontal } from "lucide-react";
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
  const { data } = useListMonitoringElections();
  const [sort, setSort] = useState(initSort);
  const [electionRoundStatus, setElectionRoundStatus] = useState(
    initElectionRoundStatus
  );
  const [country, setCountry] = useState(initCountry);
  const [searchTerm, setSearchTerm] = useState(searchText);
  const debouncedSearchTerm = useDebounce(searchTerm, 300);

  const { i18n } = useTranslation();

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
  }, [data]);

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
          country === "" ? true : election.countryId === country
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
      <div className="faded-bottom no-scrollbar grid gap-4 overflow-auto pt-4 pb-16 md:grid-cols-2 lg:grid-cols-3">
        {filteredElections?.map((electionRound) => (
          <ElectionRoundCard
            key={electionRound.id}
            electionRound={electionRound}
          />
        ))}
      </div>
    </>
  );
}

export default Page;
