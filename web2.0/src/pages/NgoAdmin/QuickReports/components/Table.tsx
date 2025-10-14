import React from 'react'
import { useListQuickReports } from '@/queries/quick-reports'
import { Route } from '@/routes/(app)/elections/$electionRoundId/quick-reports'
import { useDataTable } from '@/hooks/use-data-table'
import { DataTable } from '@/components/ui/data-table'
import { DataTableSkeleton } from '@/components/data-table-skeleton'
import { DataTableToolbar } from '@/components/data-table-toolbar'
import TableFilters from './Filters'
import { getQuickReportsTableColumns } from './TableColumns'

export default function Table() {
  const { electionRoundId } = Route.useParams()
  const search = Route.useSearch()
  const navigate = Route.useNavigate()
  const { data, isPending } = useListQuickReports(electionRoundId, search)

  const columns = React.useMemo(
    () =>
      getQuickReportsTableColumns({
        electionRoundId,
      }),
    [electionRoundId]
  )

  const { table } = useDataTable({
    tableName: 'quick-reports',
    data: data?.items || [],
    columns,
    pageCount: data ? Math.ceil(data.totalCount / data.pageSize) : 0,
    initialState: {
      sorting: [{ id: 'timestamp', desc: true }],
      columnPinning: { right: ['actions'] },
    },
    getRowId: (originalRow) => originalRow.id,
    search,
    navigate,
  })

  if (isPending) {
    return (
      <DataTableSkeleton
        columnCount={columns.length}
        filterCount={1}
        withViewOptions={true}
        withPagination={true}
      />
    )
  }
  return (
    <DataTable table={table}>
      <DataTableToolbar table={table}>
        <TableFilters />
      </DataTableToolbar>
    </DataTable>
  )
}
