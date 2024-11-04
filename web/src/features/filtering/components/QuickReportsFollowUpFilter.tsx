import { QuickReportFollowUpStatus } from '@/common/types';
import { SelectFilter, SelectFilterOption } from '@/features/filtering/components/SelectFilter';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { FC } from 'react';
import { mapQuickReportFollowUpStatus } from '../../responses/utils/helpers';

interface QuickReportsFollowUpFilterProps {
  placeholder?: string;
}

export const QuickReportsFollowUpFilter: FC<QuickReportsFollowUpFilterProps> = ({ placeholder = 'Follow-up status' }) => {
  const { queryParams, navigateHandler } = useFilteringContainer();

  const onChange = (value: string) => {
    navigateHandler({ [FILTER_KEY.QuickReportFollowUpStatus]: value });
  };
  const options: SelectFilterOption[] = [
    {
      value: QuickReportFollowUpStatus.NotApplicable,
      label: mapQuickReportFollowUpStatus(QuickReportFollowUpStatus.NotApplicable),
    },

    {
      value: QuickReportFollowUpStatus.NeedsFollowUp,
      label: mapQuickReportFollowUpStatus(QuickReportFollowUpStatus.NeedsFollowUp),
    },
    {
      value: QuickReportFollowUpStatus.Resolved,
      label: mapQuickReportFollowUpStatus(QuickReportFollowUpStatus.Resolved),
    },
  ];

  return (
    <SelectFilter
      value={(queryParams as any)[FILTER_KEY.QuickReportFollowUpStatus]}
      onChange={onChange}
      options={options}
      placeholder={placeholder}
    />
  );
};
