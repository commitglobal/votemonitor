import { useSetPrevSearch } from '@/common/prev-search-store';
import { FunctionComponent, IncidentReportFollowUpStatus, QuestionsAnswered } from '@/common/types';
import { PollingStationsFilters } from '@/components/PollingStationsFilters/PollingStationsFilters';
import { FilterBadge } from '@/components/ui/badge';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { Route } from '@/routes/responses';
import { useNavigate } from '@tanstack/react-router';
import { useCallback, useState } from 'react';
import { IncidentReportLocationType } from '../../models/incident-report';
import type { FormSubmissionsSearchParams } from '../../models/search-params';
import {
  mapIncidentReportFollowUpStatus,
  mapIncidentReportLocationType,
  mapQuestionsAnswered,
} from '../../utils/helpers';
import { ResetFiltersButton } from '../ResetFiltersButton/ResetFiltersButton';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';

export function IncidentReportsFiltersByEntry(): FunctionComponent {
  const navigate = useNavigate({ from: '/responses' });
  const search = Route.useSearch();
  const setPrevSearch = useSetPrevSearch();
  const { filteringIsActive } = useFilteringContainer();
  const [isFiltering, setIsFiltering] = useState(filteringIsActive);

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
          navigateHandler({ incidentReportFollowUpStatus: value });
        }}
        value={search.incidentReportFollowUpStatus ?? ''}>
        <SelectTrigger>
          <SelectValue placeholder='Follow up status' />
        </SelectTrigger>
        <SelectContent>
          <SelectGroup>
            <SelectItem
              key={IncidentReportFollowUpStatus.NotApplicable}
              value={IncidentReportFollowUpStatus.NotApplicable}>
              {mapIncidentReportFollowUpStatus(IncidentReportFollowUpStatus.NotApplicable)}
            </SelectItem>
            <SelectItem
              key={IncidentReportFollowUpStatus.NeedsFollowUp}
              value={IncidentReportFollowUpStatus.NeedsFollowUp}>
              {mapIncidentReportFollowUpStatus(IncidentReportFollowUpStatus.NeedsFollowUp)}
            </SelectItem>
            <SelectItem key={IncidentReportFollowUpStatus.Resolved} value={IncidentReportFollowUpStatus.Resolved}>
              {mapIncidentReportFollowUpStatus(IncidentReportFollowUpStatus.Resolved)}
            </SelectItem>
          </SelectGroup>
        </SelectContent>
      </Select>

      <Select
        onValueChange={(value) => {
          navigateHandler({ incidentReportLocationType: value });
        }}
        value={search.incidentReportLocationType ?? ''}>
        <SelectTrigger>
          <SelectValue placeholder='Location type' />
        </SelectTrigger>
        <SelectContent>
          <SelectGroup>
            <SelectItem
              key={IncidentReportLocationType.PollingStation}
              value={IncidentReportLocationType.PollingStation}>
              {mapIncidentReportLocationType(IncidentReportLocationType.PollingStation)}
            </SelectItem>
            <SelectItem key={IncidentReportLocationType.OtherLocation} value={IncidentReportLocationType.OtherLocation}>
              {mapIncidentReportLocationType(IncidentReportLocationType.OtherLocation)}
            </SelectItem>
          </SelectGroup>
        </SelectContent>
      </Select>

      <Select
        onValueChange={(value) => {
          navigateHandler({ questionsAnswered: value });
        }}
        value={search.questionsAnswered ?? ''}>
        <SelectTrigger>
          <SelectValue placeholder='Questions answered' />
        </SelectTrigger>
        <SelectContent>
          <SelectGroup>
            <SelectItem value={QuestionsAnswered.None}>{QuestionsAnswered.None}</SelectItem>
            <SelectItem value={QuestionsAnswered.Some}>{QuestionsAnswered.Some}</SelectItem>
            <SelectItem value={QuestionsAnswered.All}>{QuestionsAnswered.All}</SelectItem>
          </SelectGroup>
        </SelectContent>
      </Select>

      <Select
        onValueChange={(value) => {
          navigateHandler({ hasNotes: value });
        }}
        value={search.hasNotes ?? ''}>
        <SelectTrigger>
          <SelectValue placeholder='Question notes' />
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
          navigateHandler({ hasAttachments: value });
        }}
        value={search.hasAttachments ?? ''}>
        <SelectTrigger>
          <SelectValue placeholder='Media files' />
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

      <PollingStationsFilters />

      <ResetFiltersButton
        disabled={!isFiltering}
        params={{ [FILTER_KEY.ViewBy]: 'byEntry', [FILTER_KEY.Tab]: 'incident-reports' }}
      />

      {isFiltering && (
        <div className='flex flex-wrap gap-2 col-span-full'>
          {search.incidentReportLocationType && (
            <FilterBadge
              label={`Follow-up status: ${mapIncidentReportLocationType(search.incidentReportLocationType)}`}
              onClear={onClearFilter('incidentReportLocationType')}
            />
          )}

          {search.incidentReportFollowUpStatus && (
            <FilterBadge
              label={`Follow-up status: ${mapIncidentReportFollowUpStatus(search.incidentReportFollowUpStatus)}`}
              onClear={onClearFilter('incidentReportFollowUpStatus')}
            />
          )}

          {search.hasFlaggedAnswers && (
            <FilterBadge
              label={`Flagged answers: ${search.hasFlaggedAnswers === 'true' ? 'yes' : 'no'}`}
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

          {search.questionsAnswered && (
            <FilterBadge
              label={`Questions answered: ${mapQuestionsAnswered(search.questionsAnswered)}`}
              onClear={onClearFilter('questionsAnswered')}
            />
          )}

          {search.hasNotes && (
            <FilterBadge
              label={`Question notes: ${search.hasNotes === 'true' ? 'yes' : 'no'}`}
              onClear={onClearFilter('hasNotes')}
            />
          )}
        </div>
      )}
    </>
  );
}
