import { useSetPrevSearch } from '@/common/prev-search-store';
import { useNavigate, useSearch } from '@tanstack/react-router';
import { useCallback } from 'react';
import { FILTER_KEY } from '../filtering-enums';
import { ResponsesPageSearchParamsSchema } from '@/features/responses/models/search-params';

export function useFilteringContainer() {
  const navigate = useNavigate();
  const queryParams = useSearch({ strict: false });

  const result = ResponsesPageSearchParamsSchema.safeParse(queryParams);
  let pageSearchParams: Record<string, string> = {
    [FILTER_KEY.Tab]: 'form-answers',
    [FILTER_KEY.ViewBy]: 'byEntry',
  };

  if (result.success) {
    pageSearchParams = result.data;
  }

  const setPrevSearch = useSetPrevSearch();
  const filteringIsActive = Object.keys(queryParams).some((key) => key !== FILTER_KEY.Tab && key !== FILTER_KEY.ViewBy);

  const navigateHandler = useCallback(
    (search: Record<string, any | undefined>) => {
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
    debugger;
    navigate({
      search: pageSearchParams,
    });
    setPrevSearch(pageSearchParams);
  };

  return { queryParams, filteringIsActive, navigate, navigateHandler, resetFilters };
}
