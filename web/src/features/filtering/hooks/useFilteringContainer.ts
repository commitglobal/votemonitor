import { useSetPrevSearch } from '@/common/prev-search-store';
import { useNavigate, useSearch } from '@tanstack/react-router';
import { useCallback } from 'react';
import { FILTER_KEY } from '../filtering-enums';
import { ResponsesPageSearchParamsSchema } from '@/features/responses/models/search-params';
import { HIDDEN_FILTERS } from '../components/ActiveFilters';

function filterObject<T extends object>(obj: T, keysToRemove: FILTER_KEY[]): Partial<T> {
  return Object.fromEntries(Object.entries(obj).filter(([key]) => keysToRemove.includes(key))) as Partial<T>;
}

export function useFilteringContainer() {
  const navigate = useNavigate();
  const queryParams = useSearch({ strict: false });

  const setPrevSearch = useSetPrevSearch();
  const filteringIsActive = Object.keys(queryParams)
    .filter((key) => !HIDDEN_FILTERS.includes(key))
    .some((key) => !!key);

  const navigateHandler = useCallback(
    (search: Record<string, any | undefined>) => {
      void navigate({
        // @ts-ignore
        search: (prev) => {
          const newSearch: Record<string, string | undefined | string[] | number | Date> = {
            ...prev,
            ...search,
          };
          setPrevSearch(newSearch);
          return newSearch;
        },
      });
    },
    [navigate, setPrevSearch]
  );

  const resetFilters = () => {
    navigate({
      search: filterObject(queryParams, HIDDEN_FILTERS),
    });
    setPrevSearch(filterObject(queryParams, HIDDEN_FILTERS));
  };

  return { queryParams, filteringIsActive, navigate, navigateHandler, resetFilters };
}
