import Layout from '@/components/layout/Layout';
import { Card, CardContent, CardHeader } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Separator } from '@/components/ui/separator';
import { FunnelIcon } from '@heroicons/react/24/outline';

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
              <svg
                className='mr-1.5'
                xmlns='http://www.w3.org/2000/svg'
                width='18'
                height='18'
                viewBox='0 0 18 18'
                fill='none'>
                <path
                  d='M3 12L3 12.75C3 13.9926 4.00736 15 5.25 15L12.75 15C13.9926 15 15 13.9926 15 12.75L15 12M12 6L9 3M9 3L6 6M9 3L9 12'
                  stroke='white'
                  strokeWidth='2'
                  strokeLinecap='round'
                  strokeLinejoin='round'
                />
              </svg>
              Create new election event
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
