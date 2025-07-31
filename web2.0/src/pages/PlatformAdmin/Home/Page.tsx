import { useDebounce } from "@/hooks/use-debounce";
import { listElectionsQueryOptions } from "@/query-options/elections";
import { Route } from "@/routes/(app)/";
import { useQuery } from "@tanstack/react-query";
import Table from "./components/Table";

function Page() {
  const search = Route.useSearch();
  const debouncedSearch = useDebounce(search, 200);
  const { data } = useQuery(listElectionsQueryOptions(debouncedSearch));

  return <Table data={data} />;
}

export default Page;
