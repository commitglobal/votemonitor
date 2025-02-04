import Layout from '@/components/layout/Layout';
import { Card, CardContent, CardHeader } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Separator } from '@/components/ui/separator';
import { FunnelIcon, PlusIcon } from '@heroicons/react/24/outline';

import { useDebounce } from '@/components/ui/multiple-selector';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { Route } from '@/routes/election-rounds';
import { useNavigate } from '@tanstack/react-router';
import { ChangeEvent, ReactElement, useState } from 'react';
import ElectionRoundsTable from './ElectionRoundsTable';
import ElectionsRoundFilter from './ElectionsRoundFilter';
import { Button } from '@/components/ui/button';
import { useDialog } from '@/components/ui/use-dialog';
import CreateElectionRoundDialog from '../CreateElectionRoundDialog/CreateElectionRoundDialog';

export default function ElectionRoundsDashboard(): ReactElement {
  const search = Route.useSearch();

  const { filteringIsActive, navigateHandler } = useFilteringContainer();
  const [filtersExpanded, setFiltersExpanded] = useState<boolean>(false);
  const [searchText, setSearchText] = useState<string>(search.searchText ?? '');

  const debouncedSearchText = useDebounce(searchText, 300);
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
          <ElectionRoundsTable />
        </CardContent>
      </Card>
      {createElectionEventDialog.dialogProps.open && (
        <CreateElectionRoundDialog {...createElectionEventDialog.dialogProps} />
      )}
    </Layout>
  );
}
