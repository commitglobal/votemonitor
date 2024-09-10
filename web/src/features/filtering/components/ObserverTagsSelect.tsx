import {
  DropdownMenu,
  DropdownMenuCheckboxItem,
  DropdownMenuContent,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useMonitoringObserversTags } from '@/hooks/tags-queries';
import { FC, useEffect, useState } from 'react';
import { useFilteringContainer } from '../hooks/useFilteringContainer';

export const ObserverTagsSelect: FC = () => {
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: tags } = useMonitoringObserversTags(currentElectionRoundId);
  const [tagsFilter, setTagsFilter] = useState<string[]>([]);

  const { queryParams, navigateHandler } = useFilteringContainer();

  const onStatusChange = (value: string) => {};

  useEffect(() => {
    navigateHandler({ tags: tagsFilter as any });
  }, [tagsFilter]);

  const toggleTagsFilter = (tag: string) => {
    setTagsFilter((prevTags: any) => {
      if (!prevTags.includes(tag)) {
        return [...prevTags, tag];
      } else {
        return prevTags.filter((tagText: string) => tagText !== tag);
      }
    });
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
            checked={tagsFilter.includes(tag)}
            onCheckedChange={() => toggleTagsFilter(tag)}
            key={tag}>
            {tag}
          </DropdownMenuCheckboxItem>
        ))}
      </DropdownMenuContent>
    </DropdownMenu>
  );
};
