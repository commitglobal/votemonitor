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
import { useNavigate } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { useMemo, useState, type ChangeEvent } from 'react';
import { ExportedDataType } from '../../models/data-export';
import type { IncidentReportsViewBy } from '../../utils/column-visibility-options';
import { ExportDataButton } from '../ExportDataButton/ExportDataButton';

import { FunctionComponent } from '@/common/types';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { Route } from '@/routes/responses';
import { IncidentReportsAggregatedByFormTable } from '../IncidentReportsAggregatedByFormTable/IncidentReportsAggregatedByFormTable';
import { IncidentReportsByEntryTable } from '../IncidentReportsByEntryTable/IncidentReportsByEntryTable';
import { IncidentReportsByObserverTable } from '../IncidentReportsByObserverTable/IncidentReportsByObserverTable';
import { IncidentReportsColumnsVisibilitySelector } from '../IncidentReportsColumnsVisibilitySelector/IncidentReportsColumnsVisibilitySelector';
import { IncidentReportsFiltersByEntry } from '../IncidentReportsFiltersByEntry/IncidentReportsFiltersByEntry';
import { IncidentReportsFiltersByForm } from '../IncidentReportsFiltersByForm/IncidentReportsFiltersByForm';
import { IncidentReportsFiltersByObserver } from '../IncidentReportsFiltersByObserver/IncidentReportsFiltersByObserver';

const viewBy: Record<IncidentReportsViewBy, string> = {
  byEntry: 'View by entry',
  byObserver: 'View aggregated by observer',
  byForm: 'View aggregated by form',
};

export default function IncidentReportsTab(): FunctionComponent {
  const navigate = useNavigate();
  const search = Route.useSearch();
  const { filteringIsActive } = useFilteringContainer();

  const { viewBy: byFilter } = search;

  const [filtersExpanded, setFiltersExpanded] = useState(filteringIsActive);

  const [searchText, setSearchText] = useState<string>('');
  const debouncedSearchText = useDebounce(searchText, 300);
  const setPrevSearch = useSetPrevSearch();
  const handleSearchInput = (ev: ChangeEvent<HTMLInputElement>): void => {
    const value = ev.currentTarget.value;
    if (!value || value.length >= 2) setSearchText(ev.currentTarget.value);
  };

  const queryParams = useMemo(() => {
    const params = [
      ['searchText', searchText],
      ['hasFlaggedAnswers', search.hasFlaggedAnswers],
      ['level1Filter', search.level1Filter],
      ['level2Filter', search.level2Filter],
      ['level3Filter', search.level3Filter],
      ['level4Filter', search.level4Filter],
      ['level5Filter', search.level5Filter],
      ['pollingStationNumberFilter', search.pollingStationNumberFilter],
      ['followUpStatus', search.incidentReportFollowUpStatus],
      ['locationType', search.incidentReportLocationType],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params);
  }, [searchText, search]);

  return (
    <Card>
      <CardHeader>
        <div className='flex items-center justify-between pr-6'>
          <CardTitle className='text-2xl font-semibold leading-none tracking-tight'>
            Incident reports submissions
          </CardTitle>

          <div className='flex items-center gap-4'>
            <ExportDataButton exportedDataType={ExportedDataType.IncidentReports} filterParams={queryParams} />

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
                    setPrevSearch({ [FILTER_KEY.ViewBy]: value, [FILTER_KEY.Tab]: 'incident-reports' });
                    navigate({
                      to: '.',
                      search: { [FILTER_KEY.ViewBy]: value, [FILTER_KEY.Tab]: 'incident-reports' },
                    });
                    setFiltersExpanded(false);
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

          <IncidentReportsColumnsVisibilitySelector byFilter={byFilter ?? 'byEntry'} />
        </div>

        <Separator />

        {filtersExpanded && (
          <div className='grid items-center grid-cols-6 gap-4'>
            {byFilter === 'byEntry' && <IncidentReportsFiltersByEntry />}
            {byFilter === 'byObserver' && <IncidentReportsFiltersByObserver />}
            {byFilter === 'byForm' && <IncidentReportsFiltersByForm />}
          </div>
        )}
      </CardHeader>

      {byFilter === 'byEntry' && <IncidentReportsByEntryTable searchText={debouncedSearchText} />}

      {byFilter === 'byObserver' && <IncidentReportsByObserverTable searchText={debouncedSearchText} />}

      {byFilter === 'byForm' && <IncidentReportsAggregatedByFormTable />}
    </Card>
  );
}
