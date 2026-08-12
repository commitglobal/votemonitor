import React, { useMemo } from 'react'
import { useQuery } from '@tanstack/react-query'
import { listMonitoringObserversTagsQueryOptions } from '@/queries/monitoring-observers'
import { Route } from '@/routes/(app)/elections/$electionRoundId/observers'
import type { Option } from '@/types/data-table'
import {
  MonitoringObserverStatusList,
  type MonitoringObserverStatus,
} from '@/types/monitoring-observer'
import { X } from 'lucide-react'
import { useDebouncedCallback } from '@/hooks/use-debounced-callback'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import {
  MultiSelectDataTableFacetedFilter,
  SingleSelectDataTableFacetedFilter,
} from '@/components/data-table/data-table-faceted-filter'

const statusOptions: Option[] = MonitoringObserverStatusList.map((status) => ({
  value: status,
  label: status,
}))

function TableFilters() {
  const { electionRoundId } = Route.useParams()
  const search = Route.useSearch()
  const navigate = Route.useNavigate()

  const { data: tags } = useQuery(
    listMonitoringObserversTagsQueryOptions(electionRoundId)
  )

  const tagsOptions = useMemo(
    () => tags?.map((tag) => ({ value: tag, label: tag })) ?? [],
    [tags]
  )

  // Read from the url, not from the table: every filter here writes to the
  // search params, so the table's own column filters are always empty and the
  // reset button would never appear.
  const isFiltered =
    Boolean(search.searchText) ||
    Boolean(search.status) ||
    (search.tags?.length ?? 0) > 0

  const onReset = React.useCallback(() => {
    navigate({
      search: { pageNumber: 1, pageSize: search.pageSize },
      replace: true,
    })
  }, [navigate, search.pageSize])

  // The input keeps its own value so typing stays instant, while the url — and
  // with it the request — is only rewritten once the user pauses.
  const [searchInput, setSearchInput] = React.useState(search.searchText ?? '')

  React.useEffect(() => {
    setSearchInput(search.searchText ?? '')
  }, [search.searchText])

  const debouncedSearch = useDebouncedCallback((value: string) => {
    navigate({
      // Back to the first page: the current one may not exist once the list
      // shrinks.
      search: (prev) => ({ ...prev, searchText: value, pageNumber: 1 }),
      replace: true,
    })
  }, 500)

  const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSearchInput(event.target.value)
    debouncedSearch(event.target.value)
  }

  return (
    <div className='flex flex-1 flex-wrap items-center gap-2'>
      <Input
        placeholder='Search'
        value={searchInput}
        onChange={handleInputChange}
        className='h-8 w-40 lg:w-56'
      />

      <SingleSelectDataTableFacetedFilter
        title='Observer status'
        options={statusOptions}
        value={search.status as string}
        onValueChange={(value) =>
          navigate({
            search: (prev) => ({
              ...prev,
              status: value as MonitoringObserverStatus,
              pageNumber: 1,
            }),
            replace: true,
          })
        }
      />

      <MultiSelectDataTableFacetedFilter
        title='Tags'
        options={tagsOptions}
        value={search.tags}
        onValueChange={(value) =>
          navigate({
            search: (prev) => ({ ...prev, tags: value, pageNumber: 1 }),
            replace: true,
          })
        }
      />

      {isFiltered && (
        <Button
          aria-label='Reset filters'
          variant='outline'
          size='sm'
          className='border-dashed'
          onClick={onReset}
        >
          <X />
          Reset
        </Button>
      )}
    </div>
  )
}

export default TableFilters
