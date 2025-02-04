import Layout from '@/components/layout/Layout';
import { Card, CardContent, CardHeader } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Separator } from '@/components/ui/separator';
import { FunnelIcon, PlusIcon } from '@heroicons/react/24/outline';

import { Button } from '@/components/ui/button';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { useDebounce } from '@/components/ui/multiple-selector';
import { useDialog } from '@/components/ui/use-dialog';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { Route } from '@/routes/election-rounds';
import { ChangeEvent, ReactElement, useEffect, useMemo, useState } from 'react';
import { ElectionsRoundsQueryParams, useElectionRounds } from '../../queries';
import CreateElectionRoundDialog from '../CreateElectionRoundDialog/CreateElectionRoundDialog';
import { electionRoundColDefs } from './columns-defs';
import ElectionsRoundFilter from './ElectionsRoundFilter';

export default function ElectionRoundsDashboard(): ReactElement {
  const search = Route.useSearch();
  const navigate = Route.useNavigate();

  const { filteringIsActive, navigateHandler } = useFilteringContainer();
  const [filtersExpanded, setFiltersExpanded] = useState<boolean>(false);
  const [searchText, setSearchText] = useState<string>(search.searchText ?? '');

  const debouncedSearchText = useDebounce(searchText, 300);
  const queryParams = useMemo(() => {
    const params: ElectionsRoundsQueryParams = {
      countryId: search.countryId,
      electionRoundStatus: search.electionRoundStatus,
      searchText: search.searchText,
    };

    return params;
  }, [debouncedSearchText, search]);

  useEffect(() => {
    navigateHandler({
      [FILTER_KEY.SearchText]: debouncedSearchText,
    });
  }, [debouncedSearchText]);

  const createElectionEventDialog = useDialog();

  const handleSearchInput = (ev: ChangeEvent<HTMLInputElement>): void => {
    setSearchText(ev.currentTarget.value);
  };
  return (
    <Layout title='Election rounds' subtitle='View all election rounds on the platform.' enableBreadcrumbs={false}>
      <Card>
        <CardHeader>
          <div className='flex justify-end gap-4 px-6'>
            <Button className='bg-purple-900 hover:bg-purple-600' onClick={() => createElectionEventDialog.trigger()}>
              <div className='flex items-center gap-2'>
                <PlusIcon className='size-4 text-white' aria-hidden='true' />
                Create new election event
              </div>
            </Button>
          </div>
          <div className='flex justify-end gap-4 px-6'>
            <>
              <Input className='max-w-md' onChange={handleSearchInput} value={searchText} placeholder='Search' />
              <FunnelIcon
                className='w-[20px] text-purple-900 cursor-pointer'
                fill={filteringIsActive ? '#5F288D' : 'rgba(0,0,0,0)'}
                onClick={() => {
                  setFiltersExpanded((prev) => !prev);
                }}
              />
            </>
          </div>

          <Separator />

          {filtersExpanded && <ElectionsRoundFilter />}
        </CardHeader>
        <CardContent>
          <QueryParamsDataTable
            columns={electionRoundColDefs}
            useQuery={(params) => useElectionRounds(params)}
            queryParams={queryParams}
            onRowClick={(electionRoundId: string) =>
              navigate({ to: `/election-rounds/$electionRoundId`, params: { electionRoundId } })
            }
          />
        </CardContent>
      </Card>
      {createElectionEventDialog.dialogProps.open && (
        <CreateElectionRoundDialog {...createElectionEventDialog.dialogProps} />
      )}
    </Layout>
  );
}
