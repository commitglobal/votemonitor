import {
  DropdownMenu,
  DropdownMenuCheckboxItem,
  DropdownMenuContent,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { useMonitoringObserversTags } from '@/hooks/tags-queries';
import { FC } from 'react';

export const MonitoringObserverTagsSelect: FC = () => {
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: tags } = useMonitoringObserversTags(currentElectionRoundId);
  const { queryParams, navigateHandler } = useFilteringContainer();
  const currentTags = (queryParams as any)?.[FILTER_KEY.MonitoringObserverTags] ?? [];

  const toggleTagsFilter = (tag: string) => {
    if (!currentTags.includes(tag)) return navigateHandler({ tags: [...currentTags, tag] });

    const filteredTags = currentTags.filter((tagText: string) => tagText !== tag);

    return navigateHandler({ [FILTER_KEY.MonitoringObserverTags]: filteredTags });
  };

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <div className='flex h-10 w-48 rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50'>
          Observer tags
        </div>
      </DropdownMenuTrigger>
      <DropdownMenuContent className='w-56'>
        {tags?.map((tag) => (
          <DropdownMenuCheckboxItem
            checked={currentTags.includes(tag)}
            onCheckedChange={() => toggleTagsFilter(tag)}
            key={tag}>
            {tag}
          </DropdownMenuCheckboxItem>
        ))}
      </DropdownMenuContent>
    </DropdownMenu>
  );
};
