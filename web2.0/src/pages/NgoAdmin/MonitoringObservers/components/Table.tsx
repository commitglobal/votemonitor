import React from 'react'
import { useSuspenseListMonitoringObservers } from '@/queries/monitoring-observers'
import { Route } from '@/routes/(app)/elections/$electionRoundId/observers'
import { useDataTable } from '@/hooks/use-data-table'
import { DataTable } from '@/components/ui/data-table'
import { DataTableToolbar } from '@/components/data-table/data-table-toolbar'
import { ObserversDialogs } from './Dialogs'
import { ObserversProvider } from './ObserversProvider'
import { getMonitoringObserversTableColumns } from './TableColumns'
import TableFilters from './TableFilters'

function Table() {
  const { electionRoundId } = Route.useParams()
  const search = Route.useSearch()
  const navigate = Route.useNavigate()

  const { data } = useSuspenseListMonitoringObservers(electionRoundId, search)

  const columns = React.useMemo(() => getMonitoringObserversTableColumns(), [])

  const { table } = useDataTable({
    tableName: 'monitoring-observers',
    data: data.items,
    columns,
    pageCount:
      data.pageSize > 0 ? Math.ceil(data.totalCount / data.pageSize) : 0,
    initialState: {
      // Keeps the menu reachable when the table scrolls sideways.
      columnPinning: { right: ['actions'] },
    },
    getRowId: (originalRow) => originalRow.id,
    // The search params are named `pageNumber`/`pageSize`, and the hook writes
    // `page` unless told otherwise. Without this the page control would put a
    // key in the url that `validateSearch` drops, so paging silently did nothing.
    pagination: { pageKey: 'pageNumber', pageSizeKey: 'pageSize' },
    // Searching is handled by the toolbar's own `searchText` param, not by the
    // table's built-in global filter.
    globalFilter: { enabled: false },
    search,
    navigate,
  })

  return (
    <ObserversProvider>
      <DataTable table={table}>
        <DataTableToolbar table={table}>
          <TableFilters />
        </DataTableToolbar>
      </DataTable>
      <ObserversDialogs />
    </ObserversProvider>
  )
}

export default Table
