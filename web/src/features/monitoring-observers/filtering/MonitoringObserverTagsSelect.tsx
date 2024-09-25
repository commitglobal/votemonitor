import { Button } from '@/components/ui/button';
import {
  DropdownMenu,
  DropdownMenuCheckboxItem,
  DropdownMenuContent,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Input } from '@/components/ui/input';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { useMonitoringObserversTags } from '@/hooks/tags-queries';
import { Check, ChevronDown, ChevronUp } from 'lucide-react';

import { FC, useState } from 'react';

export const MonitoringObserverTagsSelect: FC = () => {
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: tags } = useMonitoringObserversTags(currentElectionRoundId);
  const { queryParams, navigateHandler } = useFilteringContainer();
  const currentTags = (queryParams as any)?.[FILTER_KEY.MonitoringObserverTags] ?? [];
  const [query, setQuery] = useState<string>('');

  const toggleTagsFilter = (tag: string) => {
    if (!currentTags.includes(tag))
      return navigateHandler({ [FILTER_KEY.MonitoringObserverTags]: [...currentTags, tag] });

    const filteredTags = currentTags.filter((tagText: string) => tagText !== tag);

    return navigateHandler({ [FILTER_KEY.MonitoringObserverTags]: filteredTags });
  };

  return (
    <DropdownMenu onOpenChange={() => setQuery('')}>
      <DropdownMenuTrigger asChild>
        <Button className='border-gray-200 gap-1 hover:bg-white w-[180px]' variant='outline'>
          <span className='text-sm font-normal text-slate-700'>Observer tags</span>
          {currentTags && currentTags.length && (
            <span className='inline-block px-2 text-purple-600 rounded-full bg-purple-50'>{currentTags.length}</span>
          )}
          <ChevronDown className='w-4 h-4 opacity-50' />
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent className='w-56'>
        <div className='sticky top-0 p-2 bg-background'>
          <Input
            placeholder='Search...'
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            onKeyDown={(e: React.KeyboardEvent<HTMLInputElement>) => e.stopPropagation()}
          />
        </div>
        <ScrollArea className='h-[300px]'>
          {tags?.map((tag) => (
            <DropdownMenuCheckboxItem
              checked={currentTags.includes(tag)}
              onCheckedChange={() => toggleTagsFilter(tag)}
              key={tag}>
              {tag}
            </DropdownMenuCheckboxItem>
          ))}
        </ScrollArea>
      </DropdownMenuContent>
    </DropdownMenu>
  );
};
