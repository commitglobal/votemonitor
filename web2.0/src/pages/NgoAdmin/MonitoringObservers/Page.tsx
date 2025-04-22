import { listMonitoringObserversQueryOptions } from "@/query-options/monitoring-observers";
import { Route } from "@/routes/(app)/elections/$electionRoundId/observers";
import { useQuery } from "@tanstack/react-query";
import Table from "./components/Table";
import { useDebounce } from "@/hooks/use-debounce";

function Page() {
  const { electionRoundId } = Route.useParams();
  const search = Route.useSearch();
  const debouncedSearch = useDebounce(search, 200);
  const { data } = useQuery(
    listMonitoringObserversQueryOptions(electionRoundId, debouncedSearch)
  );

  return <Table data={data} />;
}

export default Page;
