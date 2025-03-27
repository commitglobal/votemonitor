import { useSetPrevSearch } from '@/common/prev-search-store';
import { useNavigate, useSearch } from '@tanstack/react-router';
import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { HIDDEN_FILTERS } from '../common';
import { FILTER_KEY } from '../filtering-enums';

function filterObject<T extends object>(obj: T, keysToRemove: FILTER_KEY[]): Partial<T> {
  return Object.fromEntries(Object.entries(obj).filter(([key]) => keysToRemove.includes(key))) as Partial<T>;
}

export function useFilteringContainer() {
  const navigate = useNavigate();
  const queryParams = useSearch({ strict: false });
  const hasRenderedBefore = useRef(false);

  const setPrevSearch = useSetPrevSearch();
  const filteringIsActive = useMemo(() => {
    return Object.entries(queryParams)
      .filter(([key, _]) => !HIDDEN_FILTERS.includes(key))
      .some(([_, value]) => !!value);
  }, [queryParams]);

  const [isFilteringContainerVisible, setIsFilteringContainerVisible] = useState(filteringIsActive);

  useEffect(() => {
    if (hasRenderedBefore.current) return;
    hasRenderedBefore.current = true;
    setIsFilteringContainerVisible(filteringIsActive);
  }, [filteringIsActive]);

  const navigateHandler = useCallback(
    (search: Record<string, any | undefined>) => {
      navigate({
        // @ts-ignore
        search: (prev) => {
          const newSearch: Record<string, string | undefined | string[] | number | Date | boolean> = {
            ...prev,
            [FILTER_KEY.PageNumber]: 1,
            ...search,
          };
          setPrevSearch(newSearch);
          return newSearch;
        },
      });
    },
    [navigate, setPrevSearch]
  );

  const toggleFilteringContainerVisibility = () => setIsFilteringContainerVisible((prev) => !prev);

  const resetFilters = () => {
    navigate({
      to: '.',
      replace: true,
      search: filterObject(queryParams, HIDDEN_FILTERS),
    });
    setPrevSearch(filterObject(queryParams, HIDDEN_FILTERS));
  };

  return {
    queryParams,
    filteringIsActive,
    isFilteringContainerVisible,
    toggleFilteringContainerVisibility,
    navigate,
    navigateHandler,
    resetFilters,
  };
}
