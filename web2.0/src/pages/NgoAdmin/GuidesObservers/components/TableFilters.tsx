import React from 'react'
import { Route } from '@/routes/(app)/elections/$electionRoundId/guides'
import type { Option } from '@/types/data-table'
import {
  GuideTypeLabels,
  GuideTypeList,
  type GuideType,
} from '@/types/guides-observer'
import { X } from 'lucide-react'
import { useDebouncedCallback } from '@/hooks/use-debounced-callback'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { SingleSelectDataTableFacetedFilter } from '@/components/data-table/data-table-faceted-filter'

const guideTypeOptions: Option[] = GuideTypeList.map((guideType) => ({
  label: GuideTypeLabels[guideType],
  value: guideType,
}))

function TableFilters() {
  const search = Route.useSearch()
  const navigate = Route.useNavigate()

  const isFiltered =
    Boolean(search.searchText) || Boolean(search.guideTypeFilter)

  const onReset = React.useCallback(() => {
    navigate({
      search: {
        pageNumber: 1,
        pageSize: search.pageSize,
      },
      replace: true,
    })
  }, [navigate, search.pageSize])

  // The input is kept in local state so typing stays responsive while the url,
  // and with it the filtered list, is only rewritten once the user pauses.
  const [searchInput, setSearchInput] = React.useState(search.searchText ?? '')

  React.useEffect(() => {
    setSearchInput(search.searchText ?? '')
  }, [search.searchText])

  const debouncedSearch = useDebouncedCallback((value: string) => {
    navigate({
      // Back to the first page: the current one may no longer exist once the
      // list shrinks.
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
        title='Guide type'
        options={guideTypeOptions}
        value={search.guideTypeFilter as string}
        onValueChange={(value) =>
          navigate({
            search: (prev) => ({
              ...prev,
              guideTypeFilter: value as GuideType,
              pageNumber: 1,
            }),
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
