import { useDataSource } from '@/common/data-source-store';
import { useSetPrevSearch } from '@/common/prev-search-store';
import { DataSources, QuickReportFollowUpStatus, type FunctionComponent } from '@/common/types';
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
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Separator } from '@/components/ui/separator';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { CoalitionMemberFilter } from '@/features/filtering/components/CoalitionMemberFilter';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { getValueOrDefault } from '@/lib/utils';
import { Route } from '@/routes/responses';
import { Cog8ToothIcon, FunnelIcon } from '@heroicons/react/24/outline';
import { useNavigate } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { ChangeEvent, useCallback, useMemo, useState } from 'react';
import { useQuickReports } from '../../hooks/quick-reports';
import { ExportedDataType } from '../../models/data-export';
import { IncidentCategory, IncidentCategoryList, QuickReportLocationType } from '../../models/quick-report';
import type { QuickReportsSearchParams } from '../../models/search-params';
import { useQuickReportsColumnsVisibility, useQuickReportsToggleColumn } from '../../store/column-visibility';
import { quickReportsColumnDefs } from '../../utils/column-defs';
import { quickReportsColumnVisibilityOptions } from '../../utils/column-visibility-options';
import { mapIncidentCategory, mapQuickReportFollowUpStatus, mapQuickReportLocationType } from '../../utils/helpers';
import { ExportDataButton } from '../ExportDataButton/ExportDataButton';
import { ResetFiltersButton } from '../ResetFiltersButton/ResetFiltersButton';

export interface QuickReportFilterRequest {
  dataSource: DataSources;
  searchText: string | undefined;
  level1Filter: string | undefined;
  level2Filter: string | undefined;
  level3Filter: string | undefined;
  level4Filter: string | undefined;
  level5Filter: string | undefined;
  pollingStationNumberFilter: string | undefined;
  quickReportFollowUpStatus: QuickReportFollowUpStatus | undefined;
  quickReportLocationType: QuickReportLocationType | undefined;
  incidentCategory: IncidentCategory | undefined;
  coalitionMemberId: string | undefined;
  monitoringObserverId: string | undefined;
}
export function QuickReportsTab(): FunctionComponent {
  const navigate = useNavigate();
  const search = Route.useSearch();
  const debouncedSearch = useDebounce(search, 300);

  const columnsVisibility = useQuickReportsColumnsVisibility();
  const toggleColumns = useQuickReportsToggleColumn();
  const { filteringIsActive } = useFilteringContainer();

  const [filtersExpanded, setFiltersExpanded] = useState(filteringIsActive);
  const [searchText, setSearchText] = useState<string>(search.searchText ?? '');

  const setPrevSearch = useSetPrevSearch();
  const handleSearchInput = (ev: ChangeEvent<HTMLInputElement>): void => {
    setSearchText(ev.currentTarget.value);
  };

  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const dataSource = useDataSource();

  const queryParams = useMemo(() => {
    const params: QuickReportFilterRequest = {
      dataSource: getValueOrDefault(debouncedSearch.dataSource, DataSources.Ngo),
      level1Filter: debouncedSearch.level1Filter,
      level2Filter: debouncedSearch.level2Filter,
      level3Filter: debouncedSearch.level3Filter,
      level4Filter: debouncedSearch.level4Filter,
      level5Filter: debouncedSearch.level5Filter,
      pollingStationNumberFilter: debouncedSearch.pollingStationNumberFilter,
      quickReportFollowUpStatus: debouncedSearch.quickReportFollowUpStatus,
      quickReportLocationType: debouncedSearch.quickReportLocationType,
      incidentCategory: debouncedSearch.incidentCategory,
      coalitionMemberId: search.coalitionMemberId,
      monitoringObserverId: undefined,
      searchText: searchText,
    };

    return params;
  }, [debouncedSearch, searchText]);

  const onClearFilter = useCallback(
    (filter: keyof QuickReportsSearchParams | (keyof QuickReportsSearchParams)[]) => () => {
      const filters = Array.isArray(filter)
        ? Object.fromEntries(filter.map((key) => [key, undefined]))
        : { [filter]: undefined };
      navigate({
        to: '.',
        search: (prev: any) => {
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
      navigate({ to: '/responses/quick-reports/$quickReportId', params: { quickReportId } });
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

        {filtersExpanded && (
          <div className='grid items-center grid-cols-6 gap-4'>
            {dataSource === DataSources.Coalition ? <CoalitionMemberFilter /> : null}

            <Select
              onValueChange={(value) => {
                navigate({ to: '.', search: (prev: any) => ({ ...prev, quickReportLocationType: value }) });
              }}
              value={search.quickReportLocationType ?? ''}>
              <SelectTrigger>
                <SelectValue placeholder='Location type' />
              </SelectTrigger>
              <SelectContent>
                <SelectGroup>
                  <SelectItem value={QuickReportLocationType.NotRelatedToAPollingStation}>
                    {mapQuickReportLocationType(QuickReportLocationType.NotRelatedToAPollingStation)}
                  </SelectItem>
                  <SelectItem value={QuickReportLocationType.OtherPollingStation}>
                    {mapQuickReportLocationType(QuickReportLocationType.OtherPollingStation)}
                  </SelectItem>
                  <SelectItem value={QuickReportLocationType.VisitedPollingStation}>
                    {mapQuickReportLocationType(QuickReportLocationType.VisitedPollingStation)}
                  </SelectItem>
                </SelectGroup>
              </SelectContent>
            </Select>

            <Select
              onValueChange={(value) => {
                navigate({
                  to: '.',
                  search: (prev: any) => ({ ...prev, quickReportFollowUpStatus: value }),
                });
              }}
              value={search.quickReportFollowUpStatus ?? ''}>
              <SelectTrigger>
                <SelectValue placeholder='Follow up status' />
              </SelectTrigger>
              <SelectContent>
                <SelectGroup>
                  <SelectItem value={QuickReportFollowUpStatus.NotApplicable}>
                    {mapQuickReportFollowUpStatus(QuickReportFollowUpStatus.NotApplicable)}
                  </SelectItem>
                  <SelectItem value={QuickReportFollowUpStatus.NeedsFollowUp}>
                    {mapQuickReportFollowUpStatus(QuickReportFollowUpStatus.NeedsFollowUp)}
                  </SelectItem>
                  <SelectItem value={QuickReportFollowUpStatus.Resolved}>
                    {mapQuickReportFollowUpStatus(QuickReportFollowUpStatus.Resolved)}
                  </SelectItem>
                </SelectGroup>
              </SelectContent>
            </Select>

            <Select
              onValueChange={(value) => {
                navigate({ to: '.', search: (prev: any) => ({ ...prev, incidentCategory: value }) });
              }}
              value={search.incidentCategory ?? ''}>
              <SelectTrigger>
                <SelectValue placeholder='Incident category' />
              </SelectTrigger>
              <SelectContent>
                <SelectGroup>
                  {IncidentCategoryList.map((incidentCategory) => (
                    <SelectItem key={incidentCategory} value={incidentCategory}>
                      {mapIncidentCategory(incidentCategory)}
                    </SelectItem>
                  ))}
                </SelectGroup>
              </SelectContent>
            </Select>

            <PollingStationsFilters />
            <ResetFiltersButton disabled={!filtersExpanded} params={{ tag: 'quick-reports' }} />

            {filtersExpanded && (
              <div className='flex flex-wrap gap-2 col-span-full'>
                {search.quickReportFollowUpStatus && (
                  <FilterBadge
                    label={`Follow up status: ${mapQuickReportFollowUpStatus(search.quickReportFollowUpStatus)}`}
                    onClear={onClearFilter(['quickReportFollowUpStatus'])}
                  />
                )}
                {search.quickReportLocationType && (
                  <FilterBadge
                    label={`Location Type: ${mapQuickReportLocationType(search.quickReportLocationType)}`}
                    onClear={onClearFilter(['quickReportLocationType'])}
                  />
                )}
                {search.incidentCategory && (
                  <FilterBadge
                    label={`Location Type: ${mapIncidentCategory(search.incidentCategory)}`}
                    onClear={onClearFilter(['incidentCategory'])}
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
                      'pollingStationNumberFilter',
                    ])}
                  />
                )}

                {search.level2Filter && (
                  <FilterBadge
                    label={`Location - L2: ${search.level2Filter}`}
                    onClear={onClearFilter([
                      'level2Filter',
                      'level3Filter',
                      'level4Filter',
                      'level5Filter',
                      'pollingStationNumberFilter',
                    ])}
                  />
                )}

                {search.level3Filter && (
                  <FilterBadge
                    label={`Location - L3: ${search.level3Filter}`}
                    onClear={onClearFilter([
                      'level3Filter',
                      'level4Filter',
                      'level5Filter',
                      'pollingStationNumberFilter',
                    ])}
                  />
                )}

                {search.level4Filter && (
                  <FilterBadge
                    label={`Location - L4: ${search.level4Filter}`}
                    onClear={onClearFilter(['level4Filter', 'level5Filter', 'pollingStationNumberFilter'])}
                  />
                )}

                {search.level5Filter && (
                  <FilterBadge
                    label={`Location - L5: ${search.level5Filter}`}
                    onClear={onClearFilter(['level5Filter', 'pollingStationNumberFilter'])}
                  />
                )}

                {search.pollingStationNumberFilter && (
                  <FilterBadge
                    label={`PS Number: ${search.pollingStationNumberFilter}`}
                    onClear={onClearFilter('pollingStationNumberFilter')}
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
          onRowClick={navigateToQuickReport}
        />
      </CardContent>
    </Card>
  );
}
