import {Route} from "@/routes/(app)/elections/$electionRoundId/guides";
import {useDebounce} from "@/hooks/use-debounce.ts";
import {useListMonitoringObservers} from "@/queries/monitoring-observers.ts";
import Table from "@/pages/NgoAdmin/GuidesObservers/components/Table.tsx";

export default function Page() {
    const { electionRoundId } = Route.useParams();
    const search = Route.useSearch();
    const debouncedSearch = useDebounce(search, 200);
    const { data } = useListMonitoringObservers(electionRoundId, debouncedSearch);

    // @ts-ignore
    return <Table data={data} />;
}