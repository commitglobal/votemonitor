import { SelectFilter, SelectFilterOption } from '@/features/filtering/components/SelectFilter';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { IncidentReportLocationType } from '@/features/responses/models/incident-report';
import { mapIncidentReportLocationType } from '@/features/responses/utils/helpers';
import { FC } from 'react';

export const IncidentReportsLocationTypeFilter: FC = () => {
  const { queryParams, navigateHandler } = useFilteringContainer();

  const onChange = (value: string) => {
    navigateHandler({ [FILTER_KEY.IncidentReportLocationType]: value });
  };
  const options: SelectFilterOption[] = [
    {
      value: IncidentReportLocationType.PollingStation,
      label: mapIncidentReportLocationType(IncidentReportLocationType.PollingStation),
    },

    {
      value: IncidentReportLocationType.OtherLocation,
      label: mapIncidentReportLocationType(IncidentReportLocationType.OtherLocation),
    },
  ];

  return (
    <SelectFilter
      value={(queryParams as any)[FILTER_KEY.IncidentReportLocationType]}
      onChange={onChange}
      options={options}
      placeholder='Location Type'
    />
  );
};
