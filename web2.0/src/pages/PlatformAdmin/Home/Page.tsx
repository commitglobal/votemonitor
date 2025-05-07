import { listMonitoringObserversQueryOptions } from "@/query-options/monitoring-observers";
import { Route } from "@/routes/(app)/";
import { useQuery } from "@tanstack/react-query";
import Table from "./components/Table";
import { useDebounce } from "@/hooks/use-debounce";
import { listElections } from "@/services/api/monitoring-observers/list-elections.api";
import { listElectionsQueryOptions } from "@/query-options/elections";

function Page() {
  const search = Route.useSearch();
  const debouncedSearch = useDebounce(search, 200);
  const { data } = useQuery(listElectionsQueryOptions(search));

  return <Table data={data} />;
}

export default Page;
