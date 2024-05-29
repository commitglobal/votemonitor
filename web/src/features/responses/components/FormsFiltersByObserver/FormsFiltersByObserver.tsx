import { ChevronDownIcon } from '@heroicons/react/24/outline';
import { useNavigate } from '@tanstack/react-router';
import { useCallback } from 'react';
import type { FunctionComponent } from '@/common/types';
import { FilterBadge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import {
  DropdownMenu,
  DropdownMenuCheckboxItem,
  DropdownMenuContent,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import type { FormSubmissionsSearchParams } from '../../models/search-params';
import { Route } from '@/routes/responses';
import { useMonitoringObserversTags } from '@/hooks/tags-queries';
import { ResetFiltersButton } from '../ResetFiltersButton/ResetFiltersButton';

export function FormsFiltersByObserver(): FunctionComponent {
  const navigate = useNavigate({ from: '/responses/' });
  const search = Route.useSearch();

  const { data: tags } = useMonitoringObserversTags();

  const onTagsFilterChange = useCallback(
    (tag: string) => () => {
      void navigate({
        // @ts-ignore
        search: (prev: FormSubmissionsSearchParams) => {
          const prevTagsFilter = prev.tagsFilter ?? [];
          const newTags = prevTagsFilter.includes(tag)
            ? prevTagsFilter.filter((t) => t !== tag)
            : [...prevTagsFilter, tag];

          return { ...prev, tagsFilter: newTags.length > 0 ? newTags : undefined };
        },
      });
    },
    [navigate]
  );

  const isFiltered = Object.keys(search).some((key) => key !== 'tab' && key !== 'viewBy');

  return (
    <>
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button className='w-full inline-flex border-gray-200 gap-1 hover:bg-white text-black' variant='outline'>
            <span>Observer tags</span>
            {search.tagsFilter && (
              <span className='bg-purple-50 text-purple-600 rounded-full inline-block px-2'>
                {search.tagsFilter.length}
              </span>
            )}
            <ChevronDownIcon className='w-[20px] ml-auto' />
          </Button>
        </DropdownMenuTrigger>

        <DropdownMenuContent>
          {tags?.map((tag) => (
            <DropdownMenuCheckboxItem
              checked={search.tagsFilter?.includes(tag)}
              onCheckedChange={onTagsFilterChange(tag)}
              key={tag}>
              {tag}
            </DropdownMenuCheckboxItem>
          ))}
        </DropdownMenuContent>
      </DropdownMenu>

      <ResetFiltersButton disabled={!isFiltered} />

      {isFiltered && (
        <div className='col-span-full flex gap-2 flex-wrap'>
          {search.tagsFilter?.map((tag) => (
            <FilterBadge label={`Observer tags: ${tag}`} onClear={onTagsFilterChange(tag)} />
          ))}
        </div>
      )}
    </>
  );
}
