import { MultiSelectDropdown } from '@/components/ui/multiple-select-dropdown';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { useMonitoringObserversTags } from '@/hooks/tags-queries';
import { FC, useState } from 'react';

export const MonitoringObserverTagsSelect: FC = () => {
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: tags } = useMonitoringObserversTags(currentElectionRoundId);
  const { queryParams, navigateHandler } = useFilteringContainer();
  const currentTags = (queryParams as any)?.[FILTER_KEY.MonitoringObserverTags] ?? [];
  const currentTagsSet = new Set(currentTags);
  const [query, setQuery] = useState<string>('');

  const filteredTags =
    query === ''
      ? tags?.filter((tag) => !currentTagsSet.has(tag))
      : tags
          ?.filter((tag) => !currentTagsSet.has(tag))
          .filter((option) => {
            return option.toLowerCase().includes(query.toLowerCase());
          });

  const toggleTagsFilter = (tags: string[]) => {
    return navigateHandler({ [FILTER_KEY.MonitoringObserverTags]: tags });
  };

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
