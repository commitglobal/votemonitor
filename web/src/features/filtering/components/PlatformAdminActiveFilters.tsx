import { DateTimeFormat } from '@/common/formats';
import { FilterBadge } from '@/components/ui/badge';
import { isNotNilOrWhitespace, mapFormStatus, mapFormType, toBoolean } from '@/lib/utils';
import { useNavigate } from '@tanstack/react-router';
import { format } from 'date-fns/format';
import { FC, useCallback } from 'react';
import { FILTER_KEY, FILTER_LABEL } from '../filtering-enums';
import {
  ActiveFilterProps,
  defaultLocalizator,
  isBooleanType,
  isDateType,
  SearchParams,
} from './NgoAdminActiveFilters';

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
  [FILTER_KEY.FormTypeFilter, FILTER_LABEL.FormType],
  [FILTER_KEY.LocationL1, FILTER_LABEL.LocationL1],
  [FILTER_KEY.LocationL2, FILTER_LABEL.LocationL2],
  [FILTER_KEY.LocationL3, FILTER_LABEL.LocationL3],
  [FILTER_KEY.LocationL4, FILTER_LABEL.LocationL4],
  [FILTER_KEY.LocationL5, FILTER_LABEL.LocationL5],
  [FILTER_KEY.PollingStationNumber, FILTER_LABEL.PollingStationNumber],
  [FILTER_KEY.FormId, FILTER_LABEL.FormId],
  [FILTER_KEY.FormStatusFilter, FILTER_LABEL.FormStatus],
  [FILTER_KEY.FromDate, FILTER_LABEL.FromDate],
  [FILTER_KEY.ToDate, FILTER_LABEL.ToDate],
  [FILTER_KEY.SearchText, FILTER_LABEL.SearchText],
  [FILTER_KEY.FormTemplateStatusFilter, FILTER_LABEL.FormTemplateStatus],
  [FILTER_KEY.FormTemplateTypeFilter, FILTER_LABEL.FormTemplateType],
]);

const FILTER_VALUE_LOCALIZATORS = new Map<string, (value: any) => string>([
  [FILTER_KEY.FormTemplateTypeFilter, mapFormType],
  [FILTER_KEY.FormTemplateStatusFilter, mapFormStatus],
]);

const ActiveFilter: FC<ActiveFilterProps> = ({ filterId, value, isArray }) => {
  const label = FILTER_LABELS.get(filterId) ?? filterId;

  const navigate = useNavigate();
  const onClearFilter = useCallback(
    (filter: string, value?: string) => () => {
      if (!isArray) {
        return navigate({ to: '.', replace: true, search: (prev: SearchParams) => ({ ...prev, [filter]: undefined }) });
      }
      return navigate({
        to: '.',
        replace: true,
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

interface PlatformAdminActiveFiltersProps {
  queryParams: Record<string, string | Date | number | string[] | undefined>;
}

export const PlatformAdminActiveFilters: FC<PlatformAdminActiveFiltersProps> = ({ queryParams }) => {

  return (
    <div className='flex flex-wrap gap-2 col-span-full'>
      {Object.entries(queryParams)
        .filter(([filterId, value]) => !!value)
        .filter(([filterId, value]) => isNotNilOrWhitespace(value?.toString()))
        .map(([filterId, value]) => {
          let key = '';
          const isArray = Array.isArray(value);
          const isDate = isDateType(value);
          const isBoolean = isBooleanType(value);
          const localizator = FILTER_VALUE_LOCALIZATORS.get(filterId) ?? defaultLocalizator;

          if (HIDDEN_FILTERS.includes(filterId)) return;

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
