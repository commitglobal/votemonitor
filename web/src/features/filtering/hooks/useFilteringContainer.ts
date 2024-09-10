import { useSetPrevSearch } from '@/common/prev-search-store';
import { useNavigate, useSearch } from '@tanstack/react-router';
import { useCallback } from 'react';

export function useFilteringContainer() {
  const navigate = useNavigate();
  const queryParams = useSearch({ strict: false });
  const setPrevSearch = useSetPrevSearch();
  const filteringIsActive = Object.keys(queryParams).some((key) => key !== 'tab' && key !== 'viewBy');

  const navigateHandler = useCallback(
    (search: Record<string, string | undefined>) => {
      void navigate({
        // @ts-ignore
        search: (prev) => {
          const newSearch: Record<string, string | undefined | string[] | number> = {
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
    navigate({});
    setPrevSearch({});
  };

  return { queryParams, filteringIsActive, navigate, navigateHandler, resetFilters };
}
