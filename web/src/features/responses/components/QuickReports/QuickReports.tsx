import { Cog8ToothIcon, FunnelIcon } from '@heroicons/react/24/outline';
import { getRouteApi } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { type ChangeEvent, useState, useMemo, useCallback } from 'react';
import type { FunctionComponent } from '@/common/types';
import { FilterBadge } from '@/components/ui/badge';
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
import { PollingStationsFilters } from '@/components/PollingStationsFilters/PollingStationsFilters';
import { Separator } from '@/components/ui/separator';
import { useQuickReports } from '../../hooks/quick-reports';
import type { QuickReportsSearchParams } from '../../models/search-params';
import { quickReportsColumnDefs } from '../../utils/column-defs';
import { quickReportsColumnVisibilityOptions } from '../../utils/column-visibility-options';
import { ExportedDataType } from '../../models/data-export';
import { useQuickReportsColumnsVisibility, useQuickReportsToggleColumn } from '../../store/column-visibility';
import { ExportDataButton } from '../ExportDataButton/ExportDataButton';

const routeApi = getRouteApi('/responses/');

export function QuickReports(): FunctionComponent {
  const navigate = routeApi.useNavigate();
  const search = routeApi.useSearch();
  const debouncedSearch = useDebounce(search, 300);

  const columnsVisibility = useQuickReportsColumnsVisibility();
  const toggleColumns = useQuickReportsToggleColumn();

  const [isFiltering, setIsFiltering] = useState(() => Object.keys(search).some((key) => key !== 'tab'));

  const [searchText, setSearchText] = useState<string>('');
  const debouncedSearchText = useDebounce(searchText, 300);

  const handleSearchInput = (ev: ChangeEvent<HTMLInputElement>): void => {
    const value = ev.currentTarget.value;
    if (!value || value.length >= 2) setSearchText(ev.currentTarget.value);
  };

  const queryParams = useMemo(() => {
    const params = [
      ['title', debouncedSearchText],
      ['level1Filter', debouncedSearch.level1Filter],
      ['level2Filter', debouncedSearch.level2Filter],
      ['level3Filter', debouncedSearch.level3Filter],
      ['level4Filter', debouncedSearch.level4Filter],
      ['level5Filter', debouncedSearch.level5Filter],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params) as QuickReportsSearchParams;
  }, [debouncedSearch, debouncedSearchText]);

  const onClearFilter = useCallback(
    (filter: keyof QuickReportsSearchParams | (keyof QuickReportsSearchParams)[]) => () => {
      const filters = Array.isArray(filter)
        ? Object.fromEntries(filter.map((key) => [key, undefined]))
        : { [filter]: undefined };
      void navigate({ search: (prev) => ({ ...prev, ...filters }) });
    },
    [navigate]
  );

  const isFiltered = Object.keys(search).some((key) => key !== 'tab');

  const navigateToQuickReport = useCallback(
    (quickReportId: string) => {
      void navigate({ to: '/responses/quick-reports/$quickReportId', params: { quickReportId } });
    },
    [navigate]
  );

  return (
    <Card>
      <CardHeader>
        <div className='flex justify-between items-center px-6'>
          <CardTitle>Quick reports</CardTitle>

          <ExportDataButton exportedDataType={ExportedDataType.QuickReports} />
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
                    toggleColumns(option.id, checked);
                  }}>
                  {option.label}
                </DropdownMenuCheckboxItem>
              ))}
            </DropdownMenuContent>
          </DropdownMenu>
        </div>

        <Separator />

        {isFiltering && (
          <div className='grid grid-cols-6 gap-4 items-center'>
            <PollingStationsFilters />
            <Button
              disabled={!isFiltered}
              onClick={() => {
                void navigate({});
              }}
              variant='ghost-primary'>
              Reset filters
            </Button>

            {isFiltered && (
              <div className='col-span-full flex gap-2 flex-wrap'>
                {search.level1Filter && (
                  <FilterBadge
                    label={`Location - L1: ${search.level1Filter}`}
                    onClear={onClearFilter([
                      'level1Filter',
                      'level2Filter',
                      'level3Filter',
                      'level4Filter',
                      'level5Filter',
                    ])}
                  />
                )}

                {search.level2Filter && (
                  <FilterBadge
                    label={`Location - L2: ${search.level2Filter}`}
                    onClear={onClearFilter(['level2Filter', 'level3Filter', 'level4Filter', 'level5Filter'])}
                  />
                )}

                {search.level3Filter && (
                  <FilterBadge
                    label={`Location - L3: ${search.level3Filter}`}
                    onClear={onClearFilter(['level3Filter', 'level4Filter', 'level5Filter'])}
                  />
                )}

                {search.level4Filter && (
                  <FilterBadge
                    label={`Location - L4: ${search.level4Filter}`}
                    onClear={onClearFilter(['level4Filter', 'level5Filter'])}
                  />
                )}

                {search.level5Filter && (
                  <FilterBadge
                    label={`Location - L5: ${search.level5Filter}`}
                    onClear={onClearFilter('level5Filter')}
                  />
                )}
              </div>
            )}
          </div>
        )}
      </CardHeader>

      <CardContent>
        <QueryParamsDataTable
          columnVisibility={columnsVisibility}
          columns={quickReportsColumnDefs}
          useQuery={useQuickReports}
          queryParams={queryParams}
          onRowClick={navigateToQuickReport}
        />
      </CardContent>
    </Card>
  );
}
