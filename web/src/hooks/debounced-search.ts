import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { getRouteApi } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { useEffect, useMemo, useState } from 'react';
import { z, ZodSchema } from 'zod';

export const useDebouncedSearch = <T>(routeId: string, schema: ZodSchema<T>) => {
  type SchemaType = z.infer<typeof schema>;
  type SchemaWithSearchText = SchemaType & { searchText: string };

  const { navigateHandler } = useFilteringContainer();
  const routerApi = getRouteApi(routeId as any);
  //@ts-ignore
  const search = routerApi.useSearch<SchemaWithSearchText>();
  const [searchText, setSearchText] = useState(search.searchText);
  const handleSearchInput = (ev: React.FormEvent<HTMLInputElement>) => {
    setSearchText(ev.currentTarget.value);
  };

  const debouncedSearchParams = useDebounce(search, 300);
  const debouncedSearchText = useDebounce(searchText, 300);

  const queryParams = useMemo(() => {
    const params = Object.entries(debouncedSearchParams).filter(([_, value]) => value);
    return Object.fromEntries(params);
  }, [debouncedSearchParams]);

  useEffect(() => {
    navigateHandler({
      [FILTER_KEY.SearchText]: debouncedSearchText,
    });
  }, [debouncedSearchText]);

  return { searchParams: debouncedSearchParams, searchText, queryParams, handleSearchInput };
};
