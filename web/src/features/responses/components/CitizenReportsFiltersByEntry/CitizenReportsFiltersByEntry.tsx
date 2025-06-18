import { useSetPrevSearch } from '@/common/prev-search-store';
import { CitizenReportFollowUpStatus, FunctionComponent } from '@/common/types';
import { FilterBadge } from '@/components/ui/badge';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { Route } from '@/routes/(app)/responses';
import { useNavigate } from '@tanstack/react-router';
import { useCallback, useState } from 'react';
import type { FormSubmissionsSearchParams } from '../../models/search-params';
import { mapCitizenReportFollowUpStatus } from '../../utils/helpers';
import { ResetFiltersButton } from '../ResetFiltersButton/ResetFiltersButton';

export function CitizenReportsFiltersByEntry(): FunctionComponent {
  const navigate = useNavigate({ from: '/responses' });
  const search = Route.useSearch();
  const setPrevSearch = useSetPrevSearch();
  const { filteringIsActive } = useFilteringContainer();
  const [isFiltering, setIsFiltering] = useState(filteringIsActive);

  const navigateHandler = useCallback(
    (search: Record<string, string | undefined>) => {
      navigate({
        // @ts-ignore
        search: (prev) => {
          const newSearch: Record<string, string | undefined | string[] | number | Date> = {
            ...prev,
            ...search,
          };
          setPrevSearch(newSearch);
          return newSearch;
        },
      });
    },
    [navigate, setPrevSearch]
  );

  const onClearFilter = useCallback(
    (filter: keyof FormSubmissionsSearchParams | (keyof FormSubmissionsSearchParams)[]) => () => {
      const filters = Array.isArray(filter)
        ? Object.fromEntries(filter.map((key) => [key, undefined]))
        : { [filter]: undefined };
      navigateHandler(filters);
    },
    [navigateHandler]
  );

  return (
    <>
      <Select
        onValueChange={(value) => {
          navigateHandler({ hasFlaggedAnswers: value });
        }}
        value={search.hasFlaggedAnswers ?? ''}>
        <SelectTrigger>
          <SelectValue placeholder='Flagged answers' />
        </SelectTrigger>
        <SelectContent>
          <SelectGroup>
            <SelectItem key={'true'} value='true'>
              Yes
            </SelectItem>
            <SelectItem key={'false'} value='false'>
              No
            </SelectItem>
          </SelectGroup>
        </SelectContent>
      </Select>

      <Select
        onValueChange={(value) => {
          navigateHandler({ citizenReportFollowUpStatus: value });
        }}
        value={search.citizenReportFollowUpStatus ?? ''}>
        <SelectTrigger>
          <SelectValue placeholder='Follow up status' />
        </SelectTrigger>
        <SelectContent>
          <SelectGroup>
            <SelectItem value={CitizenReportFollowUpStatus.NotApplicable}>
              {mapCitizenReportFollowUpStatus(CitizenReportFollowUpStatus.NotApplicable)}
            </SelectItem>
            <SelectItem value={CitizenReportFollowUpStatus.NeedsFollowUp}>
              {mapCitizenReportFollowUpStatus(CitizenReportFollowUpStatus.NeedsFollowUp)}
            </SelectItem>
            <SelectItem value={CitizenReportFollowUpStatus.Resolved}>
              {mapCitizenReportFollowUpStatus(CitizenReportFollowUpStatus.Resolved)}
            </SelectItem>
          </SelectGroup>
        </SelectContent>
      </Select>

      <ResetFiltersButton
        disabled={!isFiltering}
        params={{ [FILTER_KEY.ViewBy]: 'byEntry', [FILTER_KEY.Tab]: 'citizen-reports' }}
      />

      {isFiltering && (
        <div className='flex flex-wrap gap-2 col-span-full'>
          {search.formTypeFilter && (
            <FilterBadge label={`Form type: ${search.formTypeFilter}`} onClear={onClearFilter('formTypeFilter')} />
          )}

          {search.citizenReportFollowUpStatus && (
            <FilterBadge
              label={`Follow-up status: ${mapCitizenReportFollowUpStatus(search.citizenReportFollowUpStatus)}`}
              onClear={onClearFilter('citizenReportFollowUpStatus')}
            />
          )}

          {search.hasFlaggedAnswers && (
            <FilterBadge
              label={`Flagged answers: ${search.hasFlaggedAnswers ? 'yes' : 'no'}`}
              onClear={onClearFilter('hasFlaggedAnswers')}
            />
          )}
        </div>
      )}
    </>
  );
}
