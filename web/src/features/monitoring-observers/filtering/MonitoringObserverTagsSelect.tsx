import { MultiSelectDropdown } from '@/components/ui/multiple-select-dropdown';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { useMonitoringObserversTags } from '@/hooks/tags-queries';

import { FC } from 'react';

interface MonitoringObserverTagsSelectProps {
  isFilteringFormSubmissions?: boolean;
}

export const MonitoringObserverTagsSelect: FC<MonitoringObserverTagsSelectProps> = ({ isFilteringFormSubmissions }) => {
  const COMPONENT_FILTER_KEY = isFilteringFormSubmissions
    ? FILTER_KEY.FormSubmissionsMonitoringObserverTags
    : FILTER_KEY.MonitoringObserverTags;

  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: tags } = useMonitoringObserversTags(currentElectionRoundId);
  const { queryParams, navigateHandler } = useFilteringContainer();
  const currentTags = (queryParams as any)?.[COMPONENT_FILTER_KEY] ?? [];

  const toggleTagsFilter = (tags: string[]) => {
    return navigateHandler({ [COMPONENT_FILTER_KEY]: tags });
  };

  return (
    <MultiSelectDropdown
      options={tags?.map((tag) => ({ label: tag, value: tag })) ?? []}
      onValueChange={toggleTagsFilter}
      placeholder='Observer tags'
      defaultValue={currentTags}
      className='text-slate-900'
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
