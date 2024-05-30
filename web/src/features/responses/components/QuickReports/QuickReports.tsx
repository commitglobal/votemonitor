import { useSetPrevSearch } from '@/common/prev-search-store';
import { FollowUpStatus, type FunctionComponent } from '@/common/types';
import { PollingStationsFilters } from '@/components/PollingStationsFilters/PollingStationsFilters';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { FilterBadge } from '@/components/ui/badge';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import {
  DropdownMenu,
  DropdownMenuCheckboxItem,
  DropdownMenuContent,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Separator } from '@/components/ui/separator';
import { Cog8ToothIcon, FunnelIcon } from '@heroicons/react/24/outline';
import { getRouteApi } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { useCallback, useMemo, useState } from 'react';
import { useQuickReports } from '../../hooks/quick-reports';
import { ExportedDataType } from '../../models/data-export';
import { QuickReportLocationType } from '../../models/quick-report';
import type { QuickReportsSearchParams } from '../../models/search-params';
import { useQuickReportsColumnsVisibility, useQuickReportsToggleColumn } from '../../store/column-visibility';
import { quickReportsColumnDefs } from '../../utils/column-defs';
import { quickReportsColumnVisibilityOptions } from '../../utils/column-visibility-options';
import { mapQuickReportLocationType } from '../../utils/helpers';
import { ExportDataButton } from '../ExportDataButton/ExportDataButton';
import { ResetFiltersButton } from '../ResetFiltersButton/ResetFiltersButton';

const routeApi = getRouteApi('/responses/');

export function QuickReports(): FunctionComponent {
  const navigate = routeApi.useNavigate();
  const search = routeApi.useSearch();
  const debouncedSearch = useDebounce(search, 300);

  const columnsVisibility = useQuickReportsColumnsVisibility();
  const toggleColumns = useQuickReportsToggleColumn();

  const [isFiltering, setIsFiltering] = useState(() =>
    Object.keys(search).some((key) => key !== 'tab' && key !== 'viewBy')
  );

  const queryParams = useMemo(() => {
    const params = [
      ['level1Filter', debouncedSearch.level1Filter],
      ['level1Filter', debouncedSearch.level1Filter],
      ['level2Filter', debouncedSearch.level2Filter],
      ['level3Filter', debouncedSearch.level3Filter],
      ['level4Filter', debouncedSearch.level4Filter],
      ['level5Filter', debouncedSearch.level5Filter],
      ['followUpStatus', debouncedSearch.followUpStatus],
      ['quickReportLocationType', debouncedSearch.quickReportLocationType],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params) as QuickReportsSearchParams;
  }, [debouncedSearch]);

  const setPrevSearch = useSetPrevSearch();

  const onClearFilter = useCallback(
    (filter: keyof QuickReportsSearchParams | (keyof QuickReportsSearchParams)[]) => () => {
      const filters = Array.isArray(filter)
        ? Object.fromEntries(filter.map((key) => [key, undefined]))
        : { [filter]: undefined };
      void navigate({
        search: (prev) => {
          const newSearch = { ...prev, ...filters };
          setPrevSearch(newSearch);
          return newSearch;
        },
      });
    },
    [navigate, setPrevSearch]
  );

  const isFiltered = Object.keys(search).some((key) => key !== 'tab' && key !== 'viewBy');

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
            <Select
              onValueChange={(value) => {
                void navigate({ search: (prev) => ({ ...prev, quickReportLocationType: value }) });
              }}
              value={search.quickReportLocationType ?? ''}>
              <SelectTrigger>
                <SelectValue placeholder='Location type' />
              </SelectTrigger>
              <SelectContent>
                <SelectGroup>
                  <SelectItem value='NotRelatedToAPollingStation'>Not related to a pollingStation</SelectItem>
                  <SelectItem value='OtherPollingStation'>Other polling station</SelectItem>
                  <SelectItem value='VisitedPollingStation'>Visited pollingStation</SelectItem>
                </SelectGroup>
              </SelectContent>
            </Select>

            <Select
              onValueChange={(value) => {
                void navigate({ search: (prev) => ({ ...prev, followUpStatus: value }) });
              }}
              value={search.followUpStatus ?? ''}>
              <SelectTrigger>
                <SelectValue placeholder='Follow up status' />
              </SelectTrigger>
              <SelectContent>
                <SelectGroup>
                  {Object.values(FollowUpStatus).map((value) => (
                    <SelectItem value={value} key={value}>{value}</SelectItem>
                  ))}
                </SelectGroup>
              </SelectContent>
            </Select>

            <PollingStationsFilters />
            <ResetFiltersButton disabled={!isFiltered} />

            {isFiltered && (
              <div className='col-span-full flex gap-2 flex-wrap'>
                {search.followUpStatus && (
                  <FilterBadge
                    label={`Follow up status: ${search.followUpStatus}`}
                    onClear={onClearFilter(['followUpStatus'])}
                  />
                )}
                {search.quickReportLocationType && (
                  <FilterBadge
                    label={`Location Type: ${mapQuickReportLocationType(search.quickReportLocationType as QuickReportLocationType)}`}
                    onClear={onClearFilter(['quickReportLocationType'])}
                  />
                )}
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
