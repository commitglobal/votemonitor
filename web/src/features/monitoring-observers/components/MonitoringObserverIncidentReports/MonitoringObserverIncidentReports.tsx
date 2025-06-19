import type { FunctionComponent } from '@/common/types';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import {
  DropdownMenu,
  DropdownMenuCheckboxItem,
  DropdownMenuContent,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Input } from '@/components/ui/input';
import { Separator } from '@/components/ui/separator';
import {
  observerIncidentReportsColumns,
  observersIncidentReportsColumnVisibilityOptions,
} from '@/features/responses/utils/column-visibility-options';
import { Cog8ToothIcon, FunnelIcon } from '@heroicons/react/24/outline';
import { getRouteApi } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { useState, type ChangeEvent } from 'react';
import { MonitoringObserverIncidentReportsFilters } from '../MonitoringObserverIncidentReportsFilters/MonitoringObserverIncidentReports';
import { MonitoringObserverIncidentReportsTable } from '../MonitoringObserverIncidentReportsTable/MonitoringObserverIncidentReportsTable';

const routeApi = getRouteApi('/(app)/monitoring-observers/view/$monitoringObserverId/$tab');

export function MonitoringObserverIncidentReports(): FunctionComponent {
  const search = routeApi.useSearch();

  const [isFiltering, setIsFiltering] = useState(() => Object.keys(search).length !== 0);
  const [columnsVisibility, setColumnsVisibility] = useState(observerIncidentReportsColumns);

  const [searchText, setSearchText] = useState<string>('');
  const debouncedSearchText = useDebounce(searchText, 300);

  const handleSearchInput = (ev: ChangeEvent<HTMLInputElement>): void => {
    setSearchText(ev.currentTarget.value);
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle>All incident reports submitted by observer</CardTitle>

        <Separator />

        <div className='flex justify-end gap-4 px-6'>
          <Input className='max-w-md' onChange={handleSearchInput} placeholder='Search' />
          <FunnelIcon
            className='w-[20px] text-purple-900 cursor-pointer'
            fill={isFiltering ? '#5F288D' : 'rgba(0,0,0,0)'}
            onClick={() => {
              setIsFiltering((prev) => !prev);
            }}
          />

          <DropdownMenu>
            <DropdownMenuTrigger>
              <Cog8ToothIcon className='w-[20px] text-purple-900 cursor-pointer' />
            </DropdownMenuTrigger>
            <DropdownMenuContent>
              <DropdownMenuLabel>Table columns</DropdownMenuLabel>
              <DropdownMenuSeparator />
              {observersIncidentReportsColumnVisibilityOptions.map((option) => (
                <DropdownMenuCheckboxItem
                  key={option.id}
                  checked={columnsVisibility[option.id]}
                  disabled={!option.enableHiding}
                  onCheckedChange={(checked) => {
                    setColumnsVisibility((prev) => ({ ...prev, [option.id]: checked }));
                  }}>
                  {option.label}
                </DropdownMenuCheckboxItem>
              ))}
            </DropdownMenuContent>
          </DropdownMenu>
        </div>

        <Separator />

        {isFiltering && (
          <div className='grid items-center grid-cols-6 gap-4'>
            <MonitoringObserverIncidentReportsFilters />
          </div>
        )}
      </CardHeader>

      <CardContent>
        <MonitoringObserverIncidentReportsTable
          columnsVisibility={columnsVisibility}
          searchText={debouncedSearchText}
        />
      </CardContent>
    </Card>
  );
}
