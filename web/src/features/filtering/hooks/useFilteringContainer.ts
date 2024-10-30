import { useSetPrevSearch } from '@/common/prev-search-store';
import { useNavigate, useSearch } from '@tanstack/react-router';
import { useCallback, useMemo, useState } from 'react';
import { HIDDEN_FILTERS } from '../components/ActiveFilters';
import { FILTER_KEY } from '../filtering-enums';

function filterObject<T extends object>(obj: T, keysToRemove: FILTER_KEY[]): Partial<T> {
  return Object.fromEntries(Object.entries(obj).filter(([key]) => keysToRemove.includes(key))) as Partial<T>;
}

export function useFilteringContainer() {
  const navigate = useNavigate();
  const queryParams = useSearch({ strict: false });

  const setPrevSearch = useSetPrevSearch();
  const filteringIsActive = useMemo(() => {
    return Object.entries(queryParams)
      .filter(([key, _]) => !HIDDEN_FILTERS.includes(key))
      .some(([_, value]) => !!value);
  }, [queryParams]);

  const [filteringIsExpanded, setFilteringIsExpanded] = useState(filteringIsActive ?? false);

  const navigateHandler = useCallback(
    (search: Record<string, any | undefined>) => {
      void navigate({
        // @ts-ignore
        search: (prev) => {
          const newSearch: Record<string, string | undefined | string[] | number | Date | boolean> = {
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

  return {
    queryParams,
    filteringIsActive,
    filteringIsExpanded,
    setFilteringIsExpanded,
    navigate,
    navigateHandler,
    resetFilters,
  };
}
