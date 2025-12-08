import React from 'react'
import { Route } from '@/routes/(app)/elections/$electionRoundId/forms'
import { DataSource } from '@/types/common'
import type { Option } from '@/types/data-table'
import {
  FormStatus,
  FormStatusList,
  FormType,
  FormTypeList,
} from '@/types/form'
import { X } from 'lucide-react'
import { mapFormStatus, mapFormType } from '@/lib/i18n'
import { useDebouncedCallback } from '@/hooks/use-debounced-callback'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { SingleSelectDataTableFacetedFilter } from '@/components/data-table-faceted-filter'

const formStatusOptions: Option[] = FormStatusList.map((fs) => ({
  label: mapFormStatus(fs),
  value: fs,
}))

const formTypeOptions: Option[] = FormTypeList.map((ft) => ({
  label: mapFormType(ft),
  value: ft,
}))

function TableFilters() {
  const search = Route.useSearch()
  const navigate = Route.useNavigate()

  const isFiltered = Object.entries(search).some(
    ([key, value]) =>
      !['pageNumber', 'pageSize', 'sortColumnName', 'sortOrder'].includes(
        key
      ) &&
      Boolean(value) &&
      !(key === 'dataSource' && value === DataSource.Ngo)
  )

  const onReset = React.useCallback(() => {
    navigate({
      search: {
        pageNumber: 1,
        pageSize: 25,
      },
      replace: true,
    })
  }, [navigate])

  const [searchInput, setSearchInput] = React.useState(search.searchText ?? '')

  React.useEffect(() => {
    setSearchInput(search.searchText ?? '')
  }, [search.searchText])

  const debouncedSearch = useDebouncedCallback((value: string) => {
    navigate({
      search: (prev) => ({ ...prev, searchText: value }),
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
        title='Form status'
        options={formStatusOptions}
        value={search.formStatusFilter as string}
        onValueChange={(value) =>
          navigate({
            search: (prev) => ({
              ...prev,
              formStatusFilter: value as FormStatus,
            }),
            replace: true,
          })
        }
      />

      <SingleSelectDataTableFacetedFilter
        title='Form type'
        options={formTypeOptions}
        value={search.typeFilter as string}
        onValueChange={(value) =>
          navigate({
            search: (prev) => ({
              ...prev,
              typeFilter: value as FormType,
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
