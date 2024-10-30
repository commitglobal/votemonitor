import { SelectFilter, SelectFilterOption } from '@/features/filtering/components/SelectFilter';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { IncidentReportLocationType } from '@/features/responses/models/incident-report';
import { QuickReportLocationType } from '@/features/responses/models/quick-report';
import { mapIncidentReportLocationType, mapQuickReportLocationType } from '@/features/responses/utils/helpers';
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

export const QuickReportsLocationTypeFilter: FC = () => {
  const { queryParams, navigateHandler } = useFilteringContainer();

  const onChange = (value: string) => {
    navigateHandler({ [FILTER_KEY.QuickReportLocationType]: value });
  };
  const options: SelectFilterOption[] = [
    {
      value: QuickReportLocationType.NotRelatedToAPollingStation,
      label: mapQuickReportLocationType(QuickReportLocationType.NotRelatedToAPollingStation),
    },

    {
      value: QuickReportLocationType.OtherPollingStation,
      label: mapQuickReportLocationType(QuickReportLocationType.OtherPollingStation),
    },

    {
      value: QuickReportLocationType.VisitedPollingStation,
      label: mapQuickReportLocationType(QuickReportLocationType.VisitedPollingStation),
    },
  ];

  return (
    <SelectFilter
      value={(queryParams as any)[FILTER_KEY.QuickReportLocationType]}
      onChange={onChange}
      options={options}
      placeholder='Location Type'
    />
  );
};
