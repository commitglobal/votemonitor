import { useSetPrevSearch } from '@/common/prev-search-store';
import { Badge } from '@/components/ui/badge';
import { Card, CardHeader, CardTitle } from '@/components/ui/card';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuRadioGroup,
  DropdownMenuRadioItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Input } from '@/components/ui/input';
import { Separator } from '@/components/ui/separator';
import { ChevronDownIcon, FunnelIcon } from '@heroicons/react/24/outline';
import { useDebounce } from '@uidotdev/usehooks';
import { useEffect, useMemo, useState, type ChangeEvent } from 'react';
import { ExportedDataType } from '../../models/data-export';
import type { FormSubmissionsViewBy } from '../../utils/column-visibility-options';
import { ExportDataButton } from '../ExportDataButton/ExportDataButton';
import { FormSubmissionsAggregatedByFormTable } from '../FormSubmissionsAggregatedByFormTable/FormSubmissionsAggregatedByFormTable';
import {
  FormSubmissionsByEntryTable,
  FormSubmissionsSearchRequest,
} from '../FormSubmissionsByEntryTable/FormSubmissionsByEntryTable';
import { FormSubmissionsColumnsVisibilitySelector } from '../FormSubmissionsColumnsVisibilitySelector/FormSubmissionsColumnsVisibilitySelector';

import { DataSources, FunctionComponent } from '@/common/types';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { FormSubmissionsByObserverTable } from '../FormSubmissionsByObserverTable/FormSubmissionsByObserverTable';
import { FormSubmissionsFiltersByEntry } from '../FormSubmissionsFiltersByEntry/FormSubmissionsFiltersByEntry';
import { FormSubmissionsFiltersByForm } from '../FormSubmissionsFiltersByForm/FormSubmissionsFiltersByForm';
import { FormSubmissionsFiltersByObserver } from '../FormSubmissionsFiltersByObserver/FormSubmissionsFiltersByObserver';

import { Route } from '@/routes/(app)/responses';
import { useNavigate } from '@tanstack/react-router';
import { getValueOrDefault, toBoolean } from '@/lib/utils';

const viewBy: Record<FormSubmissionsViewBy, string> = {
  byEntry: 'View by entry',
  byObserver: 'View aggregated by observer',
  byForm: 'View aggregated by form',
};

export default function FormSubmissionsTab(): FunctionComponent {
  const navigate = useNavigate();
  const search = Route.useSearch();
  const { filteringIsActive, navigateHandler } = useFilteringContainer();
  const [filtersExpanded, setFiltersExpanded] = useState<boolean>(false);

  const { viewBy: byFilter } = search;

  const [searchText, setSearchText] = useState<string>(search.searchText ?? '');

  const debouncedSearchText = useDebounce(searchText, 300);
  const setPrevSearch = useSetPrevSearch();

  const handleSearchInput = (ev: ChangeEvent<HTMLInputElement>): void => {
    setSearchText(ev.currentTarget.value);
  };

  const formSubmissionsFilter = useMemo(() => {
    const params: FormSubmissionsSearchRequest = {
      dataSource: getValueOrDefault(search.dataSource, DataSources.Ngo),
      searchText: searchText,
      formTypeFilter: search.formTypeFilter,
      hasFlaggedAnswers: toBoolean(search.hasFlaggedAnswers),
      level1Filter: search.level1Filter,
      level2Filter: search.level2Filter,
      level3Filter: search.level3Filter,
      level4Filter: search.level4Filter,
      level5Filter: search.level5Filter,
      pollingStationNumberFilter: search.pollingStationNumberFilter,
      followUpStatus: search.followUpStatus,
      questionsAnswered: search.questionsAnswered,
      hasNotes: toBoolean(search.hasNotes),
      hasAttachments: toBoolean(search.hasAttachments),
      tagsFilter: search.tagsFilter,
      formId: search.formId,
      fromDateFilter: search.submissionsFromDate?.toISOString(),
      toDateFilter: search.submissionsToDate?.toISOString(),
      coalitionMemberId: search.coalitionMemberId,
    };

    return params;
  }, [searchText, search]);

  useEffect(() => {
    navigateHandler({
      [FILTER_KEY.SearchText]: debouncedSearchText,
    });
  }, [debouncedSearchText]);

  useEffect(() => {
    setSearchText(search.searchText ?? '');
  }, [search.searchText]);

  return (
    <Card>
      <CardHeader>
        <div className='flex items-center justify-between pr-6'>
          <CardTitle className='text-2xl font-semibold leading-none tracking-tight'>Form submissions</CardTitle>

          <div className='flex items-center gap-4'>
            <ExportDataButton
              exportedDataType={ExportedDataType.FormSubmissions}
              filterParams={formSubmissionsFilter}
            />

            <DropdownMenu>
              <DropdownMenuTrigger>
                <Badge className='h-8 text-purple-900 hover:bg-purple-50 hover:text-purple-500' variant='outline'>
                  {viewBy[byFilter ?? 'byEntry']}

                  <ChevronDownIcon className='w-4 ml-2' />
                </Badge>
              </DropdownMenuTrigger>
              <DropdownMenuContent>
                <DropdownMenuRadioGroup
                  onValueChange={(value) => {
                    setPrevSearch({ [FILTER_KEY.ViewBy]: value, [FILTER_KEY.Tab]: 'form-answers' });
                    navigate({
                      to: '.',
                      replace: true,
                      search: { [FILTER_KEY.ViewBy]: value, [FILTER_KEY.Tab]: 'form-answers' },
                    });
                  }}
                  value={byFilter}>
                  {Object.entries(viewBy).map(([value, label]) => (
                    <DropdownMenuRadioItem key={value} value={value}>
                      {label}
                    </DropdownMenuRadioItem>
                  ))}
                </DropdownMenuRadioGroup>
              </DropdownMenuContent>
            </DropdownMenu>
          </div>
        </div>

        <Separator />

        <div className='flex justify-end gap-4 px-6'>
          <>
            <Input className='max-w-md' onChange={handleSearchInput} value={searchText} placeholder='Search' />
            <FunnelIcon
              className='w-[20px] text-purple-900 cursor-pointer'
              fill={filteringIsActive ? '#5F288D' : 'rgba(0,0,0,0)'}
              onClick={() => {
                setFiltersExpanded((prev) => !prev);
              }}
            />
          </>

          <FormSubmissionsColumnsVisibilitySelector byFilter={byFilter ?? 'byEntry'} />
        </div>

        <Separator />

        {filtersExpanded && (
          <>
            {byFilter === 'byEntry' && <FormSubmissionsFiltersByEntry />}
            {byFilter === 'byObserver' && <FormSubmissionsFiltersByObserver />}
            {byFilter === 'byForm' && <FormSubmissionsFiltersByForm />}
          </>
        )}
      </CardHeader>

      {byFilter === 'byEntry' && <FormSubmissionsByEntryTable searchText={debouncedSearchText} />}

      {byFilter === 'byObserver' && <FormSubmissionsByObserverTable searchText={debouncedSearchText} />}

      {byFilter === 'byForm' && <FormSubmissionsAggregatedByFormTable searchText={debouncedSearchText} />}
    </Card>
  );
}
