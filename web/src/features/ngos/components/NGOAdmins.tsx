import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { Input } from '@/components/ui/input';
import { Separator } from '@/components/ui/separator';
import { useDialog } from '@/components/ui/use-dialog';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { Route } from '@/routes/ngos/view/$ngoId.$tab';
import { Cog8ToothIcon, FunnelIcon } from '@heroicons/react/24/outline';
import { useNavigate } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { Plus } from 'lucide-react';
import { FC, useCallback, useEffect, useMemo, useState } from 'react';
import { ngoAdminsColDefs } from '../models/NGO';
import AddNgoAdminDialog from './AddNgoAdminDialog';
import { NGOsListFilters } from './filtering/NGOsListFilters';
import { useNgoAdmins } from '../hooks/ngos-queriess';

interface NGOAdminsViewProps {
  ngoId: string;
}

export const NGOAdminsView: FC<NGOAdminsViewProps> = ({ ngoId }) => {
  const navigate = useNavigate();

  const { isFilteringContainerVisible, navigateHandler, toggleFilteringContainerVisibility } = useFilteringContainer();
  const search = Route.useSearch();
  const [searchText, setSearchText] = useState(search.searchText);
  const handleSearchInput = (ev: React.FormEvent<HTMLInputElement>) => {
    setSearchText(ev.currentTarget.value);
  };

  const debouncedSearch = useDebounce(search, 300);
  const debouncedSearchText = useDebounce(searchText, 300);
  const addNgoAdminDialog = useDialog();

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
      void navigate({ to: '/ngos/view/$ngoId/$tab', params: { ngoId, tab: 'details' } });
    },
    [navigate]
  );

  return (
    <Card className='w-full pt-0'>
      <CardHeader className='flex gap-2 flex-column'>
        <div className='flex flex-row items-center justify-between'>
          <CardTitle className='text-xl'>All admins</CardTitle>
          <div className='flex md:flex-row-reverse gap-4 table-actions'>
            <Button title='Add admin' onClick={() => addNgoAdminDialog.trigger()}>
              <Plus className='mr-2' width={18} height={18} />
              Add admin
            </Button>
            <AddNgoAdminDialog ngoId={ngoId} {...addNgoAdminDialog.dialogProps} />
          </div>
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
          columns={ngoAdminsColDefs}
          useQuery={(params) => useNgoAdmins(ngoId, params)}
          queryParams={queryParams}
          onRowClick={navigateToNgo}
        />
      </CardContent>
    </Card>
  );
};
