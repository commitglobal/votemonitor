import React, { useMemo } from 'react'
import i18n from '@/i18n'
import { useFormSubmissionsFilters } from '@/queries/form-submissions'
import { useListMonitoringObserversTags } from '@/queries/monitoring-observers'
import { DataSource } from '@/types/common'
import type { Option } from '@/types/data-table'
import { FormType, FormTypeList } from '@/types/form'
import {
  FormSubmissionFollowUpStatus,
  FormSubmissionFollowUpStatusList,
  FormSubmissionsSearch,
  QuestionsAnswered,
  QuestionsAnsweredList,
} from '@/types/forms-submission'
import { X } from 'lucide-react'
import {
  mapFormSubmissionFollowUpStatus,
  mapFormType,
  mapQuestionsAnswered,
} from '@/lib/i18n'
import { useDebouncedCallback } from '@/hooks/use-debounced-callback'
import { NavigateFn } from '@/hooks/use-table-url-state'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import CoalitionMemberFilter from '@/components/CoalitionMemberFilter'
import { PollingStationFilteruseFilters } from '@/components/PollingStationFilter'
import { DateRangeFilter } from '@/components/data-table-date-filter'
import {
  MultiSelectDataTableFacetedFilter,
  SingleSelectDataTableFacetedFilter,
} from '@/components/data-table-faceted-filter'

const formSubmissionFollowUpStatusOptions: Option[] =
  FormSubmissionFollowUpStatusList.map((fs) => ({
    label: mapFormSubmissionFollowUpStatus(fs),
    value: fs,
  }))

const formTypeOptions: Option[] = FormTypeList.map((ft) => ({
  label: mapFormType(ft),
  value: ft,
}))

const questionsAnswerdOptions: Option[] = QuestionsAnsweredList.map((qa) => ({
  value: qa,
  label: mapQuestionsAnswered(qa),
}))

const binaryOptions: Option[] = [
  {
    value: 'true',
    label: i18n.t('common.yes'),
  },
  {
    value: 'false',
    label: i18n.t('common.no'),
  },
]

export function SubmissionsFilters({
  navigate,
  search,
  electionRoundId,
}: {
  navigate: NavigateFn
  search: FormSubmissionsSearch
  electionRoundId: string
}) {
  const { data: formFilters } = useFormSubmissionsFilters(
    electionRoundId,
    search.dataSource
  )
  const { data: observerTags } = useListMonitoringObserversTags(electionRoundId)
  const observerTagsOptions = useMemo(() => {
    return (
      observerTags?.map((ot) => ({
        label: ot,
        value: ot,
      })) || []
    )
  }, [observerTags])
  const formFilterOptions = useMemo(() => {
    return (
      formFilters?.formFilterOptions.map((fo) => ({
        label: fo.formName, // TODO: add form code and badge for status
        value: fo.formId,
      })) || []
    )
  }, [formFilters?.formFilterOptions])

  const isFiltered = Object.entries(search).some(
    ([key, value]) =>
      !['pageNumber', 'pageSize'].includes(key) &&
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

      <CoalitionMemberFilter />

      <SingleSelectDataTableFacetedFilter
        title='Form type'
        options={formTypeOptions}
        value={search.formTypeFilter as string}
        onValueChange={(value) =>
          navigate({
            search: (prev) => ({
              ...prev,
              formTypeFilter: value as FormType,
            }),
            replace: true,
          })
        }
      />

      <SingleSelectDataTableFacetedFilter
        title='Form'
        options={formFilterOptions}
        value={search.formId as string}
        onValueChange={(value) =>
          navigate({
            search: (prev) => ({ ...prev, formId: value }),
            replace: true,
          })
        }
      />

      <SingleSelectDataTableFacetedFilter
        title='Followup status'
        options={formSubmissionFollowUpStatusOptions}
        value={search.followUpStatus as string}
        onValueChange={(value) =>
          navigate({
            search: (prev) => ({
              ...prev,
              followUpStatus: value as FormSubmissionFollowUpStatus,
            }),
            replace: true,
          })
        }
      />

      <SingleSelectDataTableFacetedFilter
        title='Questions answered'
        options={questionsAnswerdOptions}
        value={search.questionsAnswered as string}
        onValueChange={(value) =>
          navigate({
            search: (prev) => ({
              ...prev,
              questionsAnswered: value as QuestionsAnswered,
            }),
            replace: true,
          })
        }
      />
      <SingleSelectDataTableFacetedFilter
        title='Has attachments'
        options={binaryOptions}
        value={search.hasNotes as string}
        onValueChange={(value) =>
          navigate({
            search: (prev) => ({ ...prev, hasNotes: value }),
            replace: true,
          })
        }
      />
      <SingleSelectDataTableFacetedFilter
        title='Has notes'
        options={binaryOptions}
        value={search.hasAttachments as string}
        onValueChange={(value) =>
          navigate({
            search: (prev) => ({ ...prev, hasAttachments: value }),
            replace: true,
          })
        }
      />

      <MultiSelectDataTableFacetedFilter
        title='Tags'
        options={observerTagsOptions}
        value={search.tagsFilter}
        onValueChange={(value) =>
          navigate({
            search: (prev) => ({ ...prev, tagsFilter: value }),
            replace: true,
          })
        }
      />
      <PollingStationFilteruseFilters search={search} navigate={navigate} />

      <DateRangeFilter
        value={{
          from: search.submissionsFromDate,
          to: search.submissionsToDate,
        }}
        onValueChange={(value) =>
          navigate({
            search: (prev) => ({
              ...prev,
              submissionsFromDate: value?.from,
              submissionsToDate: value?.to,
            }),
            replace: true,
          })
        }
        label='Submissions dates'
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
