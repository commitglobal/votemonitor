import { useDataSource } from '@/common/data-source-store';
import { DateTimeFormat } from '@/common/formats';
import { DataSources } from '@/common/types';
import { FilterBadge } from '@/components/ui/badge';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useCoalitionDetails } from '@/features/election-event/hooks/coalition-hooks';
import { useFormSubmissionsFilters } from '@/features/responses/hooks/form-submissions-queries';
import {
  mapFormSubmissionFollowUpStatus,
  mapIncidentCategory,
  mapQuickReportFollowUpStatus,
} from '@/features/responses/utils/helpers';
import { isNotNilOrWhitespace, toBoolean } from '@/lib/utils';
import { useNavigate } from '@tanstack/react-router';
import { format } from 'date-fns/format';
import { FC, useCallback } from 'react';
import { FILTER_KEY, FILTER_LABEL } from '../filtering-enums';

interface ActiveFilterProps {
  filterId: string;
  value: string;
  isArray?: boolean;
}

type SearchParams = {
  [key: string]: any;
};

export const HIDDEN_FILTERS = [
  FILTER_KEY.PageSize,
  FILTER_KEY.PageNumber,
  FILTER_KEY.ViewBy,
  FILTER_KEY.Tab,
  FILTER_KEY.SortOrder,
  FILTER_KEY.SortColumnName,
  FILTER_KEY.DataSource,
];

const FILTER_LABELS = new Map<string, string>([
  [FILTER_KEY.MonitoringObserverStatus, FILTER_LABEL.MonitoringObserverStatus],
  [FILTER_KEY.MonitoringObserverTags, FILTER_LABEL.MonitoringObserverTags],
  [FILTER_KEY.FormTypeFilter, FILTER_LABEL.FormTypeFilter],
  [FILTER_KEY.HasFlaggedAnswers, FILTER_LABEL.HasFlaggedAnswers],
  [FILTER_KEY.FormSubmissionFollowUpStatus, FILTER_LABEL.FollowUpStatus],
  [FILTER_KEY.HasNotes, FILTER_LABEL.HasNotes],
  [FILTER_KEY.MediaFiles, FILTER_LABEL.MediaFiles],
  [FILTER_KEY.QuestionsAnswered, FILTER_LABEL.QuestionsAnswered],
  [FILTER_KEY.LocationL1, FILTER_LABEL.LocationL1],
  [FILTER_KEY.LocationL2, FILTER_LABEL.LocationL2],
  [FILTER_KEY.LocationL3, FILTER_LABEL.LocationL3],
  [FILTER_KEY.LocationL4, FILTER_LABEL.LocationL4],
  [FILTER_KEY.LocationL5, FILTER_LABEL.LocationL5],
  [FILTER_KEY.TagsFilter, FILTER_LABEL.TagsFilter],
  [FILTER_KEY.PollingStationNumber, FILTER_LABEL.PollingStationNumber],
  [FILTER_KEY.FormId, FILTER_LABEL.FormId],
  [FILTER_KEY.FormStatusFilter, FILTER_LABEL.FormStatus],
  [FILTER_KEY.FromDate, FILTER_LABEL.FromDate],
  [FILTER_KEY.ToDate, FILTER_LABEL.ToDate],
  [FILTER_KEY.SearchText, FILTER_LABEL.SearchText],
  [FILTER_KEY.QuickReportIncidentCategory, FILTER_LABEL.QuickReportIncidentCategory],
  [FILTER_KEY.QuickReportFollowUpStatus, FILTER_LABEL.QuickReportFollowUpStatus],
  [FILTER_KEY.HasQuickReports, FILTER_LABEL.HasQuickReports],
  [FILTER_KEY.CoalitionMemberId, FILTER_LABEL.CoalitionMemberId],
]);

const FILTER_VALUE_LOCALIZATORS = new Map<string, (value: any) => string>([
  [FILTER_KEY.QuickReportFollowUpStatus, mapQuickReportFollowUpStatus],
  [FILTER_KEY.FormSubmissionFollowUpStatus, mapFormSubmissionFollowUpStatus],
  [FILTER_KEY.QuickReportIncidentCategory, mapIncidentCategory],
]);

const ActiveFilter: FC<ActiveFilterProps> = ({ filterId, value, isArray }) => {
  const label = FILTER_LABELS.get(filterId) ?? filterId;

  const navigate = useNavigate();
  const onClearFilter = useCallback(
    (filter: string, value?: string) => () => {
      if (!isArray) {
        return navigate({ search: (prev) => ({ ...prev, [filter]: undefined }) });
      }
      return navigate({
        search: (prev: SearchParams) => ({
          ...prev,
          [filter]: prev[filter]?.filter((item: string) => item !== value), // Remove the value from the array
        }),
      });
    },
    [navigate]
  );
  return <FilterBadge label={`${label}: ${value}`} onClear={onClearFilter(filterId, value)} />;
};

interface ActiveFiltersProps {
  queryParams: Record<string, string | Date | number | string[] | undefined>;
}

function isDateType(value: any): boolean {
  return value instanceof Date && !isNaN(value.getTime());
}

function isBooleanType(value: any): boolean {
  const trimmedValue = value.toString().toLowerCase().trim();

  return trimmedValue === 'true' || trimmedValue === 'false';
}
function defaultLocalizator(value: any): string {
  return (value ?? '').toString();
}

export const ActiveFilters: FC<ActiveFiltersProps> = ({ queryParams }) => {
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const dataSource = useDataSource();
  const { data: formSubmissionsFilters } = useFormSubmissionsFilters(currentElectionRoundId, dataSource);
  const { data: coalition } = useCoalitionDetails(currentElectionRoundId);

  return (
    <div className='flex flex-wrap gap-2 col-span-full'>
      {Object.entries(queryParams)
        .filter(([filterId, value]) => !!value)
        .filter(([filterId, value]) => isNotNilOrWhitespace(value?.toString()))
        .filter(
          ([filterId, value]) =>
            filterId !== FILTER_KEY.CoalitionMemberId ||
            (dataSource === DataSources.Coalition && filterId === FILTER_KEY.CoalitionMemberId)
        )
        .map(([filterId, value]) => {
          let key = '';
          const isArray = Array.isArray(value);
          const isDate = isDateType(value);
          const isBoolean = isBooleanType(value);
          const localizator = FILTER_VALUE_LOCALIZATORS.get(filterId) ?? defaultLocalizator;

          if (HIDDEN_FILTERS.includes(filterId)) return;

          if (filterId === FILTER_KEY.FormId) {
            key = `active-filter-${filterId}`;
            const form = formSubmissionsFilters?.formFilterOptions.find((f) => f.formId === value);

            if (form) {
              return <ActiveFilter key={key} filterId={filterId} value={`${form.formCode} - ${form.formName}`} />;
            }
          }

          if (filterId === FILTER_KEY.CoalitionMemberId) {
            key = `active-filter-${filterId}`;
            const ngo = coalition?.members.find((ngo) => ngo.id === value);

            if (ngo) {
              return <ActiveFilter key={key} filterId={filterId} value={ngo.name} />;
            }
          }

          if (!isArray && !isDate && !isBoolean) {
            key = `active-filter-${filterId}`;
            return <ActiveFilter key={key} filterId={filterId} value={localizator(value!.toString())} />;
          }

          if (isBoolean) {
            key = `active-filter-${filterId}`;
            return (
              <ActiveFilter
                key={key}
                filterId={filterId}
                value={toBoolean(value!.toString()) === true ? 'Yes' : 'No'}
              />
            );
          }

          if (isDate) {
            key = `active-filter-${filterId}`;
            return <ActiveFilter key={key} filterId={filterId} value={format(value!.toString(), DateTimeFormat)} />;
          }

          return (value as unknown[]).map((item: any) => {
            key = `active-filter-${filterId}-${item}`;

            return <ActiveFilter key={key} filterId={filterId} value={item as string} isArray />;
          });
        })}
    </div>
  );
};
