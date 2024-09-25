import { MultiSelectDropdown } from '@/components/ui/multiple-select-dropdown';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { useMonitoringObserversTags } from '@/hooks/tags-queries';

import { FC, useEffect } from 'react';

export const MonitoringObserverTagsSelect: FC = () => {
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: tags } = useMonitoringObserversTags(currentElectionRoundId);
  const { queryParams, navigateHandler } = useFilteringContainer();
  const currentTags = (queryParams as any)?.[FILTER_KEY.MonitoringObserverTags] ?? [];

  const toggleTagsFilter = (tags: string[]) => {
    return navigateHandler({ [FILTER_KEY.MonitoringObserverTags]: tags });
  };

  useEffect(() => {}, [currentTags]);

  return (
    <MultiSelectDropdown
      options={tags?.map((tag) => ({ label: tag, value: tag })) ?? []}
      onValueChange={toggleTagsFilter}
      placeholder='Observer tags'
      defaultValue={currentTags}
      className='text-slate-700'
      selectionDisplay={
        <div>
          <span className='text-sm font-normal text-slate-700'>Observer tags</span>
          {currentTags && currentTags.length && (
            <span className='px-2 text-purple-600 rounded-full bg-purple-50'>{currentTags.length}</span>
          )}
        </div>
      }
    />
  );
};
