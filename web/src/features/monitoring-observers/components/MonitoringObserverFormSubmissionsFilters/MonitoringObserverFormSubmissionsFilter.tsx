import { FormSubmissionFollowUpStatus, FormType, type FunctionComponent } from '@/common/types';
import { PollingStationsFilters } from '@/components/PollingStationsFilters/PollingStationsFilters';
import { FilterBadge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { mapFormType } from '@/lib/utils';
import { useNavigate } from '@tanstack/react-router';
import { useCallback } from 'react';
import type { MonitoringObserverDetailsRouteSearch } from '../../models/monitoring-observer';

import { Route } from '@/routes/monitoring-observers/view/$monitoringObserverId.$tab';

export function MonitoringObserverFormSubmissionsFilters(): FunctionComponent {
  const navigate = useNavigate();
  const search = Route.useSearch();
  const params = Route.useParams();

  const onClearFilter = useCallback(
    (filter: keyof MonitoringObserverDetailsRouteSearch | (keyof MonitoringObserverDetailsRouteSearch)[]) => () => {
      const filters = Array.isArray(filter)
        ? Object.fromEntries(filter.map((key) => [key, undefined]))
        : { [filter]: undefined };
      navigate({ to: '.', params, replace: true, search: (prev: any) => ({ ...prev, ...filters }) });
    },
    [navigate]
  );

  return (
    <>
      <Select
        onValueChange={(value) => {
          navigate({
            to: '.',
            params,
            replace: true,
            search: (prev: any) => ({ ...prev, followUpStatus: value }),
          });
        }}
        value={search.followUpStatus ?? ''}>
        <SelectTrigger>
          <SelectValue placeholder='Follow up status' />
        </SelectTrigger>
        <SelectContent>
          <SelectGroup>
            {Object.values(FormSubmissionFollowUpStatus).map((value) => (
              <SelectItem value={value} key={value}>
                {value}
              </SelectItem>
            ))}
          </SelectGroup>
        </SelectContent>
      </Select>

      <Select
        onValueChange={(value) => {
          navigate({ to: '.', params, replace: true, search: (prev: any) => ({ ...prev, formTypeFilter: value }) });
        }}
        value={search.formTypeFilter ?? ''}>
        <SelectTrigger>
          <SelectValue placeholder='Form type' />
        </SelectTrigger>
        <SelectContent>
          <SelectGroup>
            {Object.values(FormType).map((value) => (
              <SelectItem value={value} key={value}>
                {mapFormType(value)}
              </SelectItem>
            ))}
          </SelectGroup>
        </SelectContent>
      </Select>

      <Select
        onValueChange={(value) => {
          navigate({
            to: '.',
            replace: true,
            params,
            search: (prev: any) => ({ ...prev, hasFlaggedAnswers: value }),
          });
        }}
        value={search.hasFlaggedAnswers?.toString() ?? ''}>
        <SelectTrigger>
          <SelectValue placeholder='Flagged answers' />
        </SelectTrigger>
        <SelectContent>
          <SelectGroup>
            <SelectItem value='true'>Yes</SelectItem>
            <SelectItem value='false'>No</SelectItem>
          </SelectGroup>
        </SelectContent>
      </Select>

      <PollingStationsFilters />

      <Button
        onClick={() => {
          navigate({});
        }}
        variant='ghost-primary'>
        Reset filters
      </Button>

      {Object.entries(search).length > 0 && (
        <div className='flex flex-wrap gap-2 col-span-full'>
          {search.formTypeFilter && (
            <FilterBadge label={`Form type: ${search.formTypeFilter}`} onClear={onClearFilter('formTypeFilter')} />
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
              onClear={onClearFilter([
                'level1Filter',
                'level2Filter',
                'level3Filter',
                'level4Filter',
                'level5Filter',
                'pollingStationNumberFilter',
              ])}
            />
          )}

          {search.level2Filter && (
            <FilterBadge
              label={`Location - L2: ${search.level2Filter}`}
              onClear={onClearFilter([
                'level2Filter',
                'level3Filter',
                'level4Filter',
                'level5Filter',
                'pollingStationNumberFilter',
              ])}
            />
          )}

          {search.level3Filter && (
            <FilterBadge
              label={`Location - L3: ${search.level3Filter}`}
              onClear={onClearFilter(['level3Filter', 'level4Filter', 'level5Filter', 'pollingStationNumberFilter'])}
            />
          )}

          {search.level4Filter && (
            <FilterBadge
              label={`Location - L4: ${search.level4Filter}`}
              onClear={onClearFilter(['level4Filter', 'level5Filter', 'pollingStationNumberFilter'])}
            />
          )}

          {search.level5Filter && (
            <FilterBadge
              label={`Location - L5: ${search.level5Filter}`}
              onClear={onClearFilter(['level5Filter', 'pollingStationNumberFilter'])}
            />
          )}

          {search.pollingStationNumberFilter && (
            <FilterBadge
              label={`PS Number: ${search.pollingStationNumberFilter}`}
              onClear={onClearFilter('pollingStationNumberFilter')}
            />
          )}
        </div>
      )}
    </>
  );
}
