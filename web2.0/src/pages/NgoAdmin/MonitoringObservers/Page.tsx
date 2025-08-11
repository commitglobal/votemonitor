import { useDebounce } from "@/hooks/use-debounce";
import { Route } from "@/routes/(app)/elections/$electionRoundId/observers";
import Table from "./components/Table";
import { useListMonitoringObservers } from "@/queries/monitoring-observers";

function Page() {
  const { electionRoundId } = Route.useParams();
  const search = Route.useSearch();
  const debouncedSearch = useDebounce(search, 200);
  const { data } = useListMonitoringObservers(electionRoundId, debouncedSearch);

  return <Table data={data} />;
}

export default Page;
