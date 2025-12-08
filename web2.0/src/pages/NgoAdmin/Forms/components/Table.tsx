import React from 'react'
import { useCurrentElectionRound } from '@/contexts/election-round.context'
import { useListForms } from '@/queries/forms'
import { Route } from '@/routes/(app)/elections/$electionRoundId/forms'
import { useDataTable } from '@/hooks/use-data-table'
import { DataTable } from '@/components/ui/data-table'
import { DataTableSkeleton } from '@/components/data-table-skeleton'
import { DataTableToolbar } from '@/components/data-table-toolbar'
import TableFilters from './Filters'
import { getFormsTableColumns } from './TableColumns'

export default function Table() {
  const { electionRoundId } = Route.useParams()
  const search = Route.useSearch()
  const navigate = Route.useNavigate()
  const { data, isPending } = useListForms(electionRoundId, search)
  const { electionRound } = useCurrentElectionRound()
  const columns = React.useMemo(
    () =>
      getFormsTableColumns({
        electionRoundId,
        electionStatus: electionRound?.status,
      }),
    [electionRoundId]
  )

  const { table } = useDataTable({
    tableName: 'forms',
    data: data?.items || [],
    columns,
    pageCount: data ? Math.ceil(data.totalCount / data.pageSize) : 0,
    initialState: {
      sorting: [{ id: 'lastModifiedOn', desc: true }],
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
