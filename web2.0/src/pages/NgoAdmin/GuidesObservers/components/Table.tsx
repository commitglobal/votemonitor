import type {PageResponse} from "@/types/common.ts";
import type {GuidesObserverModel} from "@/types/guides-observer.ts";
import {DataTable} from "@/components/ui/data-table.tsx";
import {DataTableToolbar} from "@/components/data-table-toolbar.tsx";
import TableFilters from "@/pages/NgoAdmin/GuidesObservers/components/TableFilters.tsx";
import {useDataTable} from "@/hooks/use-data-table.ts";
import {useMemo, useState} from "react";
import {getGuidesObserversTableColumns} from "@/pages/NgoAdmin/GuidesObservers/components/TableColumns.tsx";
import type {DataTableRowAction} from "@/types/data-table.ts";
import { Route } from '@/routes/(app)/elections/$electionRoundId/guides'

export interface TableProps {
    data?: PageResponse<GuidesObserverModel>;
}

export default function Table({ data } : TableProps) {
    const [rowAction, setRowAction] = useState<DataTableRowAction<GuidesObserverModel> | null>(null);

    const search = Route.useSearch();
    const navigate = Route.useNavigate();

    const columns = useMemo(
        () =>
            getGuidesObserversTableColumns({
                setRowAction
            }), [setRowAction]
    )

    const { table } = useDataTable({
        tableName: "guides-observer",
        data: data?.items || [],
        columns,
        pageCount: data ? Math.ceil(data.totalCount / data.pageSize) : 0,
        initialState: {
            sorting: [{ id: "displayName", desc: false }],
            columnPinning: { right: ["actions"] },
        },
        getRowId: (originalRow) => originalRow.id,
        search,
        navigate,
    });
    return (
        <DataTable table={table}>
            <DataTableToolbar table={table}>
                <TableFilters table={table} />
            </DataTableToolbar>
        </DataTable>
    )
}