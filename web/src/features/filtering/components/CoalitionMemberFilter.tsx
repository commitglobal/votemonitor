import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useCoalitionDetails } from '@/features/election-event/hooks/coalition-hooks';
import { SelectFilter } from '@/features/filtering/components/SelectFilter';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { FC, useMemo } from 'react';

export const CoalitionMemberFilter: FC = () => {
  const { queryParams, navigateHandler } = useFilteringContainer();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  
  const { data } = useCoalitionDetails(currentElectionRoundId);

  const options = useMemo(() => {
    return (
      data?.members.map((ngo) => ({
        value: ngo.id,
        label: ngo.name,
      })) ?? []
    );
  }, [data]);

  const onChange = (value: string) => {
    navigateHandler({ [FILTER_KEY.CoalitionMemberId]: value });
  };

  return (
    <SelectFilter
      value={(queryParams as any)[FILTER_KEY.CoalitionMemberId]}
      onChange={onChange}
      options={options}
      placeholder='NGO'
    />
  );
};
