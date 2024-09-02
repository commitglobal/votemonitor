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
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { Cog8ToothIcon, FunnelIcon } from '@heroicons/react/24/outline';
import { getRouteApi } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { useCallback, useMemo, useState } from 'react';
import { useQuickReports } from '../../hooks/quick-reports';
import { ExportedDataType } from '../../models/data-export';
import type { CitizenReportsSearchParams } from '../../models/search-params';
import { useCitizenReportsColumnsVisibility, useCitizenReportsToggleColumn } from '../../store/column-visibility';
import { quickReportsColumnDefs } from '../../utils/column-defs';
import { citizenReportsColumnVisibilityOptions } from '../../utils/column-visibility-options';
import { ExportDataButton } from '../ExportDataButton/ExportDataButton';
import { ResetFiltersButton } from '../ResetFiltersButton/ResetFiltersButton';

const routeApi = getRouteApi('/responses/');

export function CitizenReports(): FunctionComponent {
  const navigate = routeApi.useNavigate();
  const search = routeApi.useSearch();
  const debouncedSearch = useDebounce(search, 300);

  const columnsVisibility = useCitizenReportsColumnsVisibility();
  const toggleColumns = useCitizenReportsToggleColumn();

  const [isFiltering, setIsFiltering] = useState(() =>
    Object.keys(search).some((key) => key !== 'tab' && key !== 'viewBy')
  );

  const queryParams = useMemo(() => {
    const params = [
      ['followUpStatus', debouncedSearch.followUpStatus],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params) as CitizenReportsSearchParams;
  }, [debouncedSearch]);

  const setPrevSearch = useSetPrevSearch();
    const currentElectionRoundId = useCurrentElectionRoundStore(s => s.currentElectionRoundId);

  const onClearFilter = useCallback(
    (filter: keyof CitizenReportsSearchParams | (keyof CitizenReportsSearchParams)[]) => () => {
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

  const navigateToCitizenReport = useCallback(
    (citizenReportId: string) => {
      void navigate({ to: '/responses/citizen-reports/$citizenReportId', params: { citizenReportId } });
    },
    [navigate]
  );

  return (
    <Card>
      <CardHeader>
        <div className='flex justify-between items-center px-6'>
          <CardTitle>Citizen reports</CardTitle>

          <ExportDataButton exportedDataType={ExportedDataType.CitizenReports} />
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
              {citizenReportsColumnVisibilityOptions.map((option) => (
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
              </div>
            )}
          </div>
        )}
      </CardHeader>

      <CardContent>
        <QueryParamsDataTable
          columnVisibility={columnsVisibility}
          columns={quickReportsColumnDefs}
          useQuery={(params) => useQuickReports(currentElectionRoundId, params)}
          queryParams={queryParams}
          onRowClick={navigateToCitizenReport}
        />
      </CardContent>
    </Card>
  );
}
