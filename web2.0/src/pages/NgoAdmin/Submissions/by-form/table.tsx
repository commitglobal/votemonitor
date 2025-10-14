import React from 'react'
import { useListFormSubmissionsAggregated } from '@/queries/form-submissions-aggregated'
import { Route } from '@/routes/(app)/elections/$electionRoundId/submissions/by-form'
import { useDataTable } from '@/hooks/use-data-table'
import { DataTable } from '@/components/ui/data-table'
import { DataTableSkeleton } from '@/components/data-table-skeleton'
import { DataTableToolbar } from '@/components/data-table-toolbar'
import { SubmissionsFilters } from '../components/submissions-filters'
import { getAggregatedFormSubmissionsColumns } from './columns'

export function Table() {
  const { electionRoundId } = Route.useParams()
  const search = Route.useSearch()
  const navigate = Route.useNavigate()
  const { data, isPending } = useListFormSubmissionsAggregated(
    electionRoundId,
    search
  )

  const columns = React.useMemo(
    () => getAggregatedFormSubmissionsColumns(electionRoundId),
    [electionRoundId]
  )

  const { table } = useDataTable({
    tableName: 'submissions-by-form',
    data: data || [],
    columns,
    pageCount: 1,
    initialState: {
      columnPinning: { right: ['actions'] },
    },

    getRowId: (originalRow) => originalRow.formId,
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
        <SubmissionsFilters
          navigate={navigate}
          search={search}
          electionRoundId={electionRoundId}
        />
      </DataTableToolbar>
    </DataTable>
  )
}
