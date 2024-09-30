import { useSetPrevSearch } from '@/common/prev-search-store';
import { Badge } from '@/components/ui/badge';
import { Card, CardHeader, CardTitle } from '@/components/ui/card';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuRadioGroup,
  DropdownMenuRadioItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Input } from '@/components/ui/input';
import { Separator } from '@/components/ui/separator';
import { ChevronDownIcon, FunnelIcon } from '@heroicons/react/24/outline';
import { getRouteApi } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { useState, type ChangeEvent } from 'react';
import { ExportedDataType } from '../../models/data-export';
import type { IssueReportsViewBy } from '../../utils/column-visibility-options';
import { ExportDataButton } from '../ExportDataButton/ExportDataButton';

import { FunctionComponent } from '@/common/types';
import { IssueReportsAggregatedByFormTable } from '../IssueReportsAggregatedByFormTable/IssueReportsAggregatedByFormTable';
import { IssueReportsByEntryTable } from '../IssueReportsByEntryTable/IssueReportsByEntryTable';
import { IssueReportsByObserverTable } from '../IssueReportsByObserverTable/IssueReportsByObserverTable';
import { IssueReportsColumnsVisibilitySelector } from '../IssueReportsColumnsVisibilitySelector/IssueReportsColumnsVisibilitySelector';
import { IssueReportsFiltersByEntry } from '../IssueReportsFiltersByEntry/IssueReportsFiltersByEntry';
import { IssueReportsFiltersByObserver } from '../IssueReportsFiltersByObserver/IssueReportsFiltersByObserver';

const routeApi = getRouteApi('/responses/');

const viewBy: Record<IssueReportsViewBy, string> = {
  byEntry: 'View by entry',
  byObserver: 'View aggregated by observer',
  byForm: 'View aggregated by form',
};

export default function IssueReportsTab(): FunctionComponent {
  const navigate = routeApi.useNavigate();
  const search = routeApi.useSearch();

  const { viewBy: byFilter } = search;

  const [isFiltering, setIsFiltering] = useState(() =>
    Object.keys(search).some((key) => key !== 'tab' && key !== 'viewBy')
  );

  const [searchText, setSearchText] = useState<string>('');
  const debouncedSearchText = useDebounce(searchText, 300);
  const setPrevSearch = useSetPrevSearch();
  const handleSearchInput = (ev: ChangeEvent<HTMLInputElement>): void => {
    const value = ev.currentTarget.value;
    if (!value || value.length >= 2) setSearchText(ev.currentTarget.value);
  };

  return (
    <Card>
      <CardHeader>
        <div className='flex items-center justify-between px-6'>
          <CardTitle>Issue reports submissions</CardTitle>

          <div className='flex items-center gap-4'>
            <ExportDataButton exportedDataType={ExportedDataType.IssueReports} />

            <DropdownMenu>
              <DropdownMenuTrigger>
                <Badge className='h-8 text-purple-900 hover:bg-purple-50 hover:text-purple-500' variant='outline'>
                  {viewBy[byFilter ?? 'byEntry']}

                  <ChevronDownIcon className='w-4 ml-2' />
                </Badge>
              </DropdownMenuTrigger>
              <DropdownMenuContent>
                <DropdownMenuRadioGroup
                  onValueChange={(value) => {
                    setPrevSearch({ viewBy: value });
                    void navigate({ search: { viewBy: value } });
                    setIsFiltering(false);
                  }}
                  value={byFilter}>
                  {Object.entries(viewBy).map(([value, label]) => (
                    <DropdownMenuRadioItem key={value} value={value}>
                      {label}
                    </DropdownMenuRadioItem>
                  ))}
                </DropdownMenuRadioGroup>
              </DropdownMenuContent>
            </DropdownMenu>
          </div>
        </div>

        <Separator />

        <div className='flex justify-end gap-4 px-6'>
          {byFilter !== 'byForm' && (
            <>
              <Input className='max-w-md' onChange={handleSearchInput} placeholder='Search' />
              <FunnelIcon
                className='w-[20px] text-purple-900 cursor-pointer'
                fill={isFiltering ? '#5F288D' : 'rgba(0,0,0,0)'}
                onClick={() => {
                  setIsFiltering((prev) => !prev);
                }}
              />
            </>
          )}

          <IssueReportsColumnsVisibilitySelector byFilter={byFilter ?? 'byEntry'} />
        </div>

        <Separator />

        {isFiltering && (
          <div className='grid items-center grid-cols-6 gap-4'>
            {byFilter === 'byEntry' && <IssueReportsFiltersByEntry />}

            {byFilter === 'byObserver' && <IssueReportsFiltersByObserver />}
          </div>
        )}
      </CardHeader>

      {byFilter === 'byEntry' && <IssueReportsByEntryTable searchText={debouncedSearchText} />}

      {byFilter === 'byObserver' && <IssueReportsByObserverTable searchText={debouncedSearchText} />}

      {byFilter === 'byForm' && <IssueReportsAggregatedByFormTable />}
    </Card>
  );
}
