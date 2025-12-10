import React from 'react'
import { useListForms } from '@/queries/forms'
import { Route } from '@/routes/(app)/elections/$electionRoundId/forms'
import { useDataTable } from '@/hooks/use-data-table'
import { DataTable } from '@/components/ui/data-table'
import { DataTableSkeleton } from '@/components/data-table/data-table-skeleton'
import { DataTableToolbar } from '@/components/data-table/data-table-toolbar'
import TableFilters from './Filters'
import { FormsProvider } from './FormsProvider'
import { getFormsTableColumns } from './TableColumns'

export default function Table() {
  const { electionRoundId } = Route.useParams()
  const search = Route.useSearch()
  const navigate = Route.useNavigate()
  const { data, isPending } = useListForms(electionRoundId, search)
  const columns = React.useMemo(() => getFormsTableColumns(), [])

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
    <FormsProvider>
      <DataTable table={table}>
        <DataTableToolbar table={table}>
          <TableFilters />
        </DataTableToolbar>
      </DataTable>
    </FormsProvider>
  )
}
