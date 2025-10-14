import React from 'react'
import { useListFormSubmissions } from '@/queries/form-submissions'
import { Route } from '@/routes/(app)/elections/$electionRoundId/submissions'
import { useDataTable } from '@/hooks/use-data-table'
import { DataTable } from '@/components/ui/data-table'
import { DataTableSkeleton } from '@/components/data-table-skeleton'
import { DataTableToolbar } from '@/components/data-table-toolbar'
import { SubmissionsFilters } from '../components/submissions-filters'
import { getFormSubmissionsColumns } from './columns'

export function Table() {
  const { electionRoundId } = Route.useParams()
  const search = Route.useSearch()
  const navigate = Route.useNavigate()
  const { data, isPending } = useListFormSubmissions(electionRoundId, search)

  const columns = React.useMemo(
    () => getFormSubmissionsColumns(electionRoundId),
    [electionRoundId]
  )

  const { table } = useDataTable({
    tableName: 'submissions-by-entry',
    data: data?.items || [],
    columns,
    pageCount: data ? Math.ceil(data.totalCount / data.pageSize) : 0,
    initialState: {
      sorting: [{ id: 'timeSubmitted', desc: true }],
      columnPinning: { right: ['actions'] },
    },

    getRowId: (originalRow) => originalRow.submissionId,
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
