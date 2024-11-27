import { ZFormType } from '@/common/types';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useElectionRoundDetails } from '@/features/election-event/hooks/election-event-hooks';
import { SelectFilter, SelectFilterOption } from '@/features/filtering/components/SelectFilter';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { mapFormType } from '@/lib/utils';
import { FC, useMemo } from 'react';

export const FormTypeFilter: FC = () => {
  const { queryParams, navigateHandler } = useFilteringContainer();

  const onChange = (value: string) => {
    navigateHandler({ [FILTER_KEY.FormTypeFilter]: value });
  };

  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: electionRound } = useElectionRoundDetails(currentElectionRoundId);

  const selectOptions = useMemo(() => {
    const options: SelectFilterOption[] = [
      {
        value: ZFormType.Values.Opening,
        label: mapFormType(ZFormType.Values.Opening),
      },
      {
        value: ZFormType.Values.Voting,
        label: mapFormType(ZFormType.Values.Voting),
      },
      {
        value: ZFormType.Values.ClosingAndCounting,
        label: mapFormType(ZFormType.Values.ClosingAndCounting),
      },
      {
        value: ZFormType.Values.PSI,
        label: mapFormType(ZFormType.Values.PSI),
      },
      {
        value: ZFormType.Values.CitizenReporting,
        label: mapFormType(ZFormType.Values.CitizenReporting),
      },
    ];

    if (electionRound?.isMonitoringNgoForCitizenReporting) {
      options.push({
        value: ZFormType.Values.IncidentReporting,
        label: mapFormType(ZFormType.Values.IncidentReporting),
      });
    }
    options.push({
      value: ZFormType.Values.Other,
      label: mapFormType(ZFormType.Values.Other),
    });

    return options;
  }, [electionRound?.isMonitoringNgoForCitizenReporting]);

  return (
    <SelectFilter
      value={(queryParams as any)[FILTER_KEY.FormTypeFilter]}
      onChange={onChange}
      options={selectOptions}
      placeholder='Form type'
    />
  );
};
