import { useSetPrevSearch } from '@/common/prev-search-store';
import { FollowUpStatus, FunctionComponent } from '@/common/types';
import { PollingStationsFilters } from '@/components/PollingStationsFilters/PollingStationsFilters';
import { FilterBadge } from '@/components/ui/badge';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Route } from '@/routes/responses';
import { useNavigate } from '@tanstack/react-router';
import { useCallback } from 'react';
import { FormType } from '../../models/form-submission';
import type { FormSubmissionsSearchParams } from '../../models/search-params';
import { ResetFiltersButton } from '../ResetFiltersButton/ResetFiltersButton';
import { mapFollowUpStatus } from '../../utils/helpers';

export function FormsFiltersByEntry(): FunctionComponent {
  const navigate = useNavigate({ from: '/responses/' });
  const search = Route.useSearch();
  const setPrevSearch = useSetPrevSearch();

  const navigateHandler = useCallback(
    (search: Record<string, string | undefined>) => {
      void navigate({
        // @ts-ignore
        search: (prev) => {
          const newSearch: Record<string, string | undefined | string[] | number> = {
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

  const isFiltered = Object.keys(search).some((key) => key !== 'tab' && key !== 'viewBy');

  return (
    <>
      <Select
        onValueChange={(value) => {
          navigateHandler({ formTypeFilter: value });
        }}
        value={search.formTypeFilter ?? ''}>
        <SelectTrigger>
          <SelectValue placeholder='Form type' />
        </SelectTrigger>
        <SelectContent>
          <SelectGroup>
            {Object.values(FormType).map((value) => (
              <SelectItem value={value} key={value}>
                {value}
              </SelectItem>
            ))}
          </SelectGroup>
        </SelectContent>
      </Select>

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
          navigateHandler({ followUpStatus: value });
        }}
        value={search.followUpStatus ?? ''}>
        <SelectTrigger>
          <SelectValue placeholder='Follow up status' />
        </SelectTrigger>
        <SelectContent>
          <SelectGroup>
            <SelectItem value={FollowUpStatus.NotApplicable}>Not applicable</SelectItem>
            <SelectItem value={FollowUpStatus.NeedsFollowUp}>Needs follow-up</SelectItem>
            <SelectItem value={FollowUpStatus.Resolved}>Resolved</SelectItem>
          </SelectGroup>
        </SelectContent>
      </Select>

      <PollingStationsFilters />

      <ResetFiltersButton disabled={!isFiltered} />

      {isFiltered && (
        <div className='col-span-full flex gap-2 flex-wrap'>
          {search.formTypeFilter && (
            <FilterBadge label={`Form type: ${search.formTypeFilter}`} onClear={onClearFilter('formTypeFilter')} />
          )}

          {search.followUpStatus && (
            <FilterBadge
              label={`Follow-up status: ${mapFollowUpStatus(search.followUpStatus)}`}
              onClear={onClearFilter('followUpStatus')}
            />
          )}

          {search.hasFlaggedAnswers && (
            <FilterBadge
              label={`Flagged answers: ${search.hasFlaggedAnswers ? 'yes' : 'no'}`}
              onClear={onClearFilter('hasFlaggedAnswers')}
            />
          )}

          {search.level1Filter && (
            <FilterBadge
              label={`Location - L1: ${search.level1Filter}`}
              onClear={onClearFilter(['level1Filter', 'level2Filter', 'level3Filter', 'level4Filter', 'level5Filter'])}
            />
          )}

          {search.level2Filter && (
            <FilterBadge
              label={`Location - L2: ${search.level2Filter}`}
              onClear={onClearFilter(['level2Filter', 'level3Filter', 'level4Filter', 'level5Filter'])}
            />
          )}

          {search.level3Filter && (
            <FilterBadge
              label={`Location - L3: ${search.level3Filter}`}
              onClear={onClearFilter(['level3Filter', 'level4Filter', 'level5Filter'])}
            />
          )}

          {search.level4Filter && (
            <FilterBadge
              label={`Location - L4: ${search.level4Filter}`}
              onClear={onClearFilter(['level4Filter', 'level5Filter'])}
            />
          )}

          {search.level5Filter && (
            <FilterBadge label={`Location - L5: ${search.level5Filter}`} onClear={onClearFilter('level5Filter')} />
          )}
        </div>
      )}
    </>
  );
}
