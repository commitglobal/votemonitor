import React from 'react'
import { useSuspenseListGuidesObservers } from '@/queries/guides-observers'
import { Route } from '@/routes/(app)/elections/$electionRoundId/guides'
import { SortOrder } from '@/types/common'
import type { GuidesObserverModel } from '@/types/guides-observer'
import { useDataTable } from '@/hooks/use-data-table'
import { DataTable } from '@/components/ui/data-table'
import { DataTableToolbar } from '@/components/data-table/data-table-toolbar'
import { getGuidesObserversTableColumns } from './TableColumns'
import TableFilters from './TableFilters'

/** Columns the table can be sorted by, mapped to the value used to compare rows. */
const sortAccessors: Record<
  string,
  (guide: GuidesObserverModel) => string | number
> = {
  title: (guide) => guide.title?.toLowerCase() ?? '',
  guideType: (guide) => guide.guideType ?? '',
  createdOn: (guide) => new Date(guide.createdOn).getTime(),
  createdBy: (guide) => guide.createdBy?.toLowerCase() ?? '',
}

export default function Table() {
  const { electionRoundId } = Route.useParams()
  const search = Route.useSearch()
  const navigate = Route.useNavigate()

  // The endpoint hands over every guide of the election round at once, so the
  // three steps below (filter, sort, slice) are what a paginated API would
  // normally do server side.
  const { data: guides } = useSuspenseListGuidesObservers(electionRoundId)

  const columns = React.useMemo(() => getGuidesObserversTableColumns(), [])

  const filteredGuides = React.useMemo(() => {
    const searchText = search.searchText?.trim().toLowerCase()

    return guides.filter((guide) => {
      if (
        search.guideTypeFilter &&
        guide.guideType !== search.guideTypeFilter
      ) {
        return false
      }

      if (!searchText) {
        return true
      }

      // Same two columns the search box hints at: the guide name and whoever
      // uploaded it.
      return (
        guide.title?.toLowerCase().includes(searchText) ||
        guide.createdBy?.toLowerCase().includes(searchText)
      )
    })
  }, [guides, search.searchText, search.guideTypeFilter])

  const sortedGuides = React.useMemo(() => {
    const accessor = search.sortColumnName
      ? sortAccessors[search.sortColumnName]
      : undefined

    if (!accessor) {
      return filteredGuides
    }

    const direction = search.sortOrder === SortOrder.Desc ? -1 : 1

    // Copied first: sort mutates in place and the array belongs to the query cache.
    return [...filteredGuides].sort((left, right) => {
      const leftValue = accessor(left)
      const rightValue = accessor(right)

      if (leftValue === rightValue) {
        return 0
      }

      return (leftValue > rightValue ? 1 : -1) * direction
    })
  }, [filteredGuides, search.sortColumnName, search.sortOrder])

  const pageCount = Math.max(
    1,
    Math.ceil(sortedGuides.length / search.pageSize)
  )

  // Filtering, or deleting the last row of the last page, can leave the url
  // pointing past the end of the list. Clamp the slice so nothing flashes empty
  // and put the url back in range.
  const pageIndex = Math.min(Math.max(0, search.pageNumber - 1), pageCount - 1)

  React.useEffect(() => {
    if (search.pageNumber > pageCount) {
      navigate({
        search: (prev) => ({ ...prev, pageNumber: pageCount }),
        replace: true,
      })
    }
  }, [navigate, pageCount, search.pageNumber])

  const pagedGuides = React.useMemo(
    () =>
      sortedGuides.slice(
        pageIndex * search.pageSize,
        pageIndex * search.pageSize + search.pageSize
      ),
    [sortedGuides, pageIndex, search.pageSize]
  )

  const { table } = useDataTable({
    tableName: 'guides-observers',
    data: pagedGuides,
    columns,
    pageCount,
    initialState: {
      sorting: [{ id: 'createdOn', desc: true }],
      columnPinning: { right: ['actions'] },
    },
    getRowId: (originalRow) => originalRow.id,
    // Filtering is driven by the dedicated filters below, not by the table's
    // own global filter state.
    globalFilter: { enabled: false },
    pagination: { pageKey: 'pageNumber', pageSizeKey: 'pageSize' },
    search,
    navigate,
  })

  return (
    <DataTable table={table}>
      <DataTableToolbar table={table}>
        <TableFilters />
      </DataTableToolbar>
    </DataTable>
  )
}
