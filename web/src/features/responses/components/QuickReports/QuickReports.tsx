import { Cog8ToothIcon, FunnelIcon } from '@heroicons/react/24/outline';
import { getRouteApi } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { type ChangeEvent, useState, useMemo } from 'react';
import type { FunctionComponent } from '@/common/types';
import { CsvFileIcon } from '@/components/icons/CsvFileIcon';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import {
  DropdownMenu,
  DropdownMenuCheckboxItem,
  DropdownMenuContent,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { Input } from '@/components/ui/input';
import { Separator } from '@/components/ui/separator';
import { useQuickReports } from '../../hooks/quick-reports';
import type { QuickReportsSearchParams } from '../../models/search-params';
import { quickReportsColumnDefs } from '../../utils/column-defs';
import { quickReportsColumnVisibilityOptions, quickReportsDefaultColumns } from '../../utils/column-visibility-options';

const routeApi = getRouteApi('/responses/');

export function QuickReports(): FunctionComponent {
  const search = routeApi.useSearch();

  const [columnsVisibility, setColumnsVisibility] = useState(quickReportsDefaultColumns);
  const [isFiltering, setIsFiltering] = useState(() => Object.keys(search).some((key) => key !== 'tab'));

  const [searchText, setSearchText] = useState<string>('');
  const debouncedSearchText = useDebounce(searchText, 300);

  const handleSearchInput = (ev: ChangeEvent<HTMLInputElement>): void => {
    const value = ev.currentTarget.value;
    if (!value || value.length >= 2) setSearchText(ev.currentTarget.value);
  };

  const queryParams = useMemo(() => {
    const params = [['title', debouncedSearchText]].filter(([_, value]) => value);

    return Object.fromEntries(params) as QuickReportsSearchParams;
  }, [debouncedSearchText]);

  return (
    <Card>
      <CardHeader>
        <div className='flex justify-between items-center px-6'>
          <CardTitle>Quick reports</CardTitle>

          <Button
            className='bg-background hover:bg-purple-50 hover:text-purple-500 text-purple-900 flex gap-2'
            variant='outline'>
            <CsvFileIcon />
            Export to csv
          </Button>
        </div>

        <Separator />

        <div className='px-6 flex justify-end gap-4'>
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
              {quickReportsColumnVisibilityOptions.map((option) => (
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
          <div className='grid grid-cols-6 gap-4 items-center'>{/* @todo add filters and filter badges */}</div>
        )}
      </CardHeader>

      <CardContent>
        <QueryParamsDataTable
          columnVisibility={columnsVisibility}
          columns={quickReportsColumnDefs}
          useQuery={useQuickReports}
          queryParams={queryParams}
        />
      </CardContent>
    </Card>
  );
}
