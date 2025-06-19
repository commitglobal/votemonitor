import { IncidentReportFollowUpStatus, type FunctionComponent } from '@/common/types';
import { FilterBadge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import {
  DropdownMenu,
  DropdownMenuCheckboxItem,
  DropdownMenuContent,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useMonitoringObserversTags } from '@/hooks/tags-queries';
import { Route } from '@/routes/(app)/responses';
import { ChevronDownIcon } from '@heroicons/react/24/outline';
import { useNavigate } from '@tanstack/react-router';
import { useCallback, useState } from 'react';
import type { FormSubmissionsSearchParams } from '../../models/search-params';
import { mapIncidentReportFollowUpStatus } from '../../utils/helpers';
import { ResetFiltersButton } from '../ResetFiltersButton/ResetFiltersButton';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';

export function IncidentReportsFiltersByObserver(): FunctionComponent {
  const navigate = useNavigate({ from: '/responses' });
  const search = Route.useSearch();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { filteringIsActive } = useFilteringContainer();
  const [isFiltering, setIsFiltering] = useState(filteringIsActive);

  const { data: tags } = useMonitoringObserversTags(currentElectionRoundId);

  const onTagsFilterChange = useCallback(
    (tag: string) => () => {
      navigate({
        // @ts-ignore
        search: (prev: FormSubmissionsSearchParams) => {
          const prevTagsFilter = prev.tagsFilter ?? [];
          const newTags = prevTagsFilter.includes(tag)
            ? prevTagsFilter.filter((t) => t !== tag)
            : [...prevTagsFilter, tag];

          return { ...prev, tagsFilter: newTags.length > 0 ? newTags : undefined };
        },
      });
    },
    [navigate]
  );

  const onFollowUpFilterChange = useCallback(
    (followUpStatus: string) => {
      navigate({
        // @ts-ignore
        search: (prev: FormSubmissionsSearchParams) => {
          return { ...prev, incidentReportFollowUpStatus: followUpStatus !== 'ALL' ? followUpStatus : undefined };
        },
      });
    },
    [navigate]
  );

  return (
    <>
      <Select
        onValueChange={(value) => {
          onFollowUpFilterChange(value);
        }}
        value={search.incidentReportFollowUpStatus ?? ''}>
        <SelectTrigger>
          <SelectValue placeholder='Follow up status' />
        </SelectTrigger>
        <SelectContent>
          <SelectGroup>
            <SelectItem value={'ALL'}>All</SelectItem>
            <SelectItem value={IncidentReportFollowUpStatus.NeedsFollowUp}>
              {mapIncidentReportFollowUpStatus(IncidentReportFollowUpStatus.NeedsFollowUp)}
            </SelectItem>
          </SelectGroup>
        </SelectContent>
      </Select>

      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button className='inline-flex w-full font-normal border-gray-200 gap-1hover:bg-white' variant='outline'>
            <span className='text-slate-900 font-small'>Observer tags</span>
            {search.tagsFilter && (
              <span className='inline-block px-2 rounded-full bg-purple-50'>{search.tagsFilter.length}</span>
            )}
            <ChevronDownIcon className='w-[20px] ml-auto' />
          </Button>
        </DropdownMenuTrigger>

        <DropdownMenuContent>
          {tags?.map((tag) => (
            <DropdownMenuCheckboxItem
              checked={search.tagsFilter?.includes(tag)}
              onCheckedChange={onTagsFilterChange(tag)}
              key={tag}>
              {tag}
            </DropdownMenuCheckboxItem>
          ))}
        </DropdownMenuContent>
      </DropdownMenu>

      <ResetFiltersButton
        disabled={!isFiltering}
        params={{ [FILTER_KEY.ViewBy]: 'byObserver', [FILTER_KEY.Tab]: 'incident-reports' }}
      />

      {isFiltering && (
        <div className='flex flex-wrap gap-2 col-span-full'>
          {search.incidentReportFollowUpStatus && (
            <FilterBadge
              label={`Follow-up status: ${mapIncidentReportFollowUpStatus(search.incidentReportFollowUpStatus)}`}
              onClear={() => onFollowUpFilterChange('ALL')}
            />
          )}
          {search.tagsFilter?.map((tag) => (
            <FilterBadge key={tag} label={`Observer tags: ${tag}`} onClear={onTagsFilterChange(tag)} />
          ))}
        </div>
      )}
    </>
  );
}
