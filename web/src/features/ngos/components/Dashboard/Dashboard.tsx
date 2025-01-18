import { authApi } from '@/common/auth-api';
import type { DataTableParameters, PageResponse } from '@/common/types';
import Layout from '@/components/layout/Layout';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { Input } from '@/components/ui/input';
import { Separator } from '@/components/ui/separator';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { Route } from '@/routes/ngos';
import { Cog8ToothIcon, FunnelIcon } from '@heroicons/react/24/outline';
import { useQuery, type UseQueryResult } from '@tanstack/react-query';
import { useNavigate, useRouter } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { useCallback, useEffect, useMemo, useState, type ReactElement } from 'react';
import { ngoColDefs, type NGO } from '../../models/NGO';
import { NGOsListFilters } from '../filtering/NGOsListFilters';

function useNGOs(p: DataTableParameters): UseQueryResult<PageResponse<NGO>, Error> {
  console.log(p);
  return useQuery({
    queryKey: ['ngos', { ...p }],
    queryFn: async () => {
      const response = await authApi.get<PageResponse<NGO>>('/ngos', {
        params: {
          ...p.otherParams,
          PageNumber: p.pageNumber,
          PageSize: p.pageSize,
          SortColumnName: p.sortColumnName,
          SortOrder: p.sortOrder,
        },
      });

      if (response.status !== 200) {
        throw new Error('Failed to fetch ngos');
      }

      return response.data;
    },
  });
}

export default function NGOsDashboard(): ReactElement {
  const navigate = useNavigate();

  const router = useRouter();
  const { isFilteringContainerVisible, navigateHandler, toggleFilteringContainerVisibility } = useFilteringContainer();

  const search = Route.useSearch();
  const [searchText, setSearchText] = useState(search.searchText);
  const handleSearchInput = (ev: React.FormEvent<HTMLInputElement>) => {
    setSearchText(ev.currentTarget.value);
  };

  const debouncedSearch = useDebounce(search, 300);
  const debouncedSearchText = useDebounce(searchText, 300);

  useEffect(() => {
    navigateHandler({
      [FILTER_KEY.SearchText]: debouncedSearchText,
    });
  }, [debouncedSearchText]);

  useEffect(() => {
    setSearchText(search.searchText ?? '');
  }, [search.searchText]);

  const queryParams = useMemo(() => {
    const params = [
      ['searchText', debouncedSearch.searchText],
      ['status', debouncedSearch.status],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params);
  }, [debouncedSearch]);

  const navigateToNgo = useCallback(
    (ngoId: string) => {
      void navigate({ to: '/ngos/$ngoId', params: { ngoId } });
    },
    [navigate]
  );

  return (
    <Layout title={'Organizations'} subtitle='Manage'>
      <Card className='w-full pt-0'>
        <CardHeader className='flex gap-2 flex-column'>
          <div className='flex flex-row items-center justify-between'>
            <CardTitle className='text-xl'>All organizations</CardTitle>
          </div>
          <Separator />

          <div className='flex flex-row justify-end gap-4  filters'>
            <Input value={searchText} onChange={handleSearchInput} className='max-w-md' placeholder='Search' />
            <FunnelIcon
              onClick={toggleFilteringContainerVisibility}
              className='w-[20px] text-purple-900 cursor-pointer'
              fill={isFilteringContainerVisible ? '#5F288D' : 'rgba(0,0,0,0)'}
            />
            <Cog8ToothIcon className='w-[20px] text-purple-900' />
          </div>
          {isFilteringContainerVisible && <NGOsListFilters />}
        </CardHeader>
        <CardContent>
          <QueryParamsDataTable
            columns={ngoColDefs}
            useQuery={(params) => useNGOs(params)}
            queryParams={queryParams}
            onRowClick={navigateToNgo}
          />
        </CardContent>
      </Card>
    </Layout>
  );
}
