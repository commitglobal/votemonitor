import { useSetPrevSearch } from '@/common/prev-search-store';
import { type FunctionComponent } from '@/common/types';
import { PollingStationsFilters } from '@/components/PollingStationsFilters/PollingStationsFilters';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import {
  DropdownMenu,
  DropdownMenuCheckboxItem,
  DropdownMenuContent,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Separator } from '@/components/ui/separator';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { FilteringContainer } from '@/features/filtering/components/FilteringContainer';
import { FilteringIcon } from '@/features/filtering/components/FilteringIcon';
import { FormSubmissionsFollowUpFilter } from '@/features/filtering/components/FormSubmissionsFollowUpFilter';
import { QuickReportsLocationTypeFilter } from '@/features/filtering/components/LocationTypeFilters';
import { QuickReportsIncidentCategoryFilter } from '@/features/filtering/components/QuickReportsIncidentCategoryFilter';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { Cog8ToothIcon } from '@heroicons/react/24/outline';
import { getRouteApi } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { useCallback, useMemo } from 'react';
import { useQuickReports } from '../../hooks/quick-reports';
import { ExportedDataType } from '../../models/data-export';
import type { QuickReportsSearchParams } from '../../models/search-params';
import { useQuickReportsColumnsVisibility, useQuickReportsToggleColumn } from '../../store/column-visibility';
import { quickReportsColumnDefs } from '../../utils/column-defs';
import { quickReportsColumnVisibilityOptions } from '../../utils/column-visibility-options';
import { ExportDataButton } from '../ExportDataButton/ExportDataButton';

const routeApi = getRouteApi('/responses/');

export function QuickReportsTab(): FunctionComponent {
  const navigate = routeApi.useNavigate();
  const search = routeApi.useSearch();
  const debouncedSearch = useDebounce(search, 300);

  const columnsVisibility = useQuickReportsColumnsVisibility();
  const toggleColumns = useQuickReportsToggleColumn();
  const { filteringIsExpanded, setFilteringIsExpanded } = useFilteringContainer();

  const queryParams = useMemo(() => {
    const params = [
      ['level1Filter', debouncedSearch.level1Filter],
      ['level1Filter', debouncedSearch.level1Filter],
      ['level2Filter', debouncedSearch.level2Filter],
      ['level3Filter', debouncedSearch.level3Filter],
      ['level4Filter', debouncedSearch.level4Filter],
      ['level5Filter', debouncedSearch.level5Filter],
      ['pollingStationNumberFilter', debouncedSearch.pollingStationNumberFilter],
      ['quickReportFollowUpStatus', debouncedSearch.followUpStatus],
      ['quickReportLocationType', debouncedSearch.quickReportLocationType],
      ['incidentCategory', debouncedSearch.incidentCategory],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params) as QuickReportsSearchParams;
  }, [debouncedSearch]);

  const setPrevSearch = useSetPrevSearch();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);

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

  const navigateToQuickReport = useCallback(
    (quickReportId: string) => {
      void navigate({ to: '/responses/quick-reports/$quickReportId', params: { quickReportId } });
    },
    [navigate]
  );

  return (
    <Card>
      <CardHeader>
        <div className='flex items-center justify-between pr-6'>
          <CardTitle className='text-2xl font-semibold leading-none tracking-tight'>Quick reports</CardTitle>

          <ExportDataButton exportedDataType={ExportedDataType.QuickReports} filterParams={queryParams} />
        </div>

        <Separator />

        <div className='flex justify-end gap-4 px-6 h-9'>
          <FilteringIcon filteringIsExpanded={filteringIsExpanded} setFilteringIsExpanded={setFilteringIsExpanded} />

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

        {filteringIsExpanded && (
          <>
            <FilteringContainer>
              <QuickReportsLocationTypeFilter />
              <FormSubmissionsFollowUpFilter />
              <QuickReportsIncidentCategoryFilter />
              <PollingStationsFilters />
            </FilteringContainer>
          </>
        )}
      </CardHeader>

      <CardContent>
        <QueryParamsDataTable
          columnVisibility={columnsVisibility}
          columns={quickReportsColumnDefs}
          useQuery={(params) => useQuickReports(currentElectionRoundId, params)}
          queryParams={queryParams}
          onRowClick={navigateToQuickReport}
        />
      </CardContent>
    </Card>
  );
}
