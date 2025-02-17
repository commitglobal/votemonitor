import { FormType } from '@/common/types';
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
        value: FormType.Opening,
        label: mapFormType(FormType.Opening),
      },
      {
        value: FormType.Voting,
        label: mapFormType(FormType.Voting),
      },
      {
        value: FormType.ClosingAndCounting,
        label: mapFormType(FormType.ClosingAndCounting),
      },
      {
        value: FormType.PSI,
        label: mapFormType(FormType.PSI),
      },
      {
        value: FormType.CitizenReporting,
        label: mapFormType(FormType.CitizenReporting),
      },
    ];

    if (electionRound?.isMonitoringNgoForCitizenReporting) {
      options.push({
        value: FormType.IncidentReporting,
        label: mapFormType(FormType.IncidentReporting),
      });
    }
    options.push({
      value: FormType.Other,
      label: mapFormType(FormType.Other),
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
