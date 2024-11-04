import { SelectFilter } from '@/features/filtering/components/SelectFilter';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { IncidentCategoryList } from '@/features/responses/models/quick-report';
import { FC, useMemo } from 'react';
import { mapIncidentCategory } from '../../responses/utils/helpers';

export const QuickReportsIncidentCategoryFilter: FC = () => {
  const { queryParams, navigateHandler } = useFilteringContainer();

  const onChange = (value: string) => {
    navigateHandler({ [FILTER_KEY.QuickReportIncidentCategory]: value });
  };

  const options = useMemo(() => {
    return IncidentCategoryList.map((incidentCategory) => ({
      value: incidentCategory,
      label: mapIncidentCategory(incidentCategory),
    }));
  }, [IncidentCategoryList]);

  return (
    <SelectFilter
      value={(queryParams as any)[FILTER_KEY.QuickReportIncidentCategory]}
      onChange={onChange}
      options={options}
      placeholder='Incident category'
    />
  );
};
