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
import { Separator } from '@/components/ui/separator';
import { ChevronDownIcon } from '@heroicons/react/24/outline';
import { useNavigate } from '@tanstack/react-router';
import { useEffect, useMemo } from 'react';
import { ExportedDataType } from '../../models/data-export';
import { ExportDataButton } from '../ExportDataButton/ExportDataButton';

import { FunctionComponent } from '@/common/types';
import { FilteringIcon } from '@/features/filtering/components/FilteringIcon';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { Route } from '@/routes/responses';
import { CitizenReportsAggregatedByFormTable } from '../CitizenReportsAggregatedByFormTable/CitizenReportsAggregatedByFormTable';
import { CitizenReportsByEntryTable } from '../CitizenReportsByEntryTable/CitizenReportsByEntryTable';
import { CitizenReportsColumnVisibilitySelector } from '../CitizenReportsColumnVisibilitySelector/CitizenReportsColumnVisibilitySelector';
import { CitizenReportsFiltersByEntry } from '../CitizenReportsFiltersByEntry/CitizenReportsFiltersByEntry';
import { CitizenReportsFiltersByForm } from '../CitizenReportsFiltersByForm/CitizenReportsFiltersByForm';

const viewBy: Record<string, string> = {
  byEntry: 'View by entry',
  byForm: 'View aggregated by form',
};

export function CitizenReportsTab(): FunctionComponent {
  const navigate = useNavigate();
  const search = Route.useSearch();
  const { filteringIsExpanded, setFilteringIsExpanded } = useFilteringContainer();

  const { viewBy: byFilter } = search;

  const setPrevSearch = useSetPrevSearch();
  useEffect(() => {
    if (byFilter === 'byObserver') {
      setPrevSearch({ [FILTER_KEY.ViewBy]: 'byEntry' });
      void navigate({ search: { [FILTER_KEY.ViewBy]: 'byEntry' } });
    }
  }, [byFilter]);

  const queryParams = useMemo(() => {
    const params = [
      ['followUpStatus', search.citizenReportFollowUpStatus],
      ['hasFlaggedAnswers', search.hasFlaggedAnswers],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params);
  }, [search]);

  return (
    <Card>
      <CardHeader>
        <div className='flex items-center justify-between pr-6'>
          <CardTitle>Citizen reports</CardTitle>

          <div className='flex items-center gap-4'>
            <ExportDataButton exportedDataType={ExportedDataType.CitizenReports} filterParams={queryParams} />

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
                    setPrevSearch({ [FILTER_KEY.ViewBy]: value, [FILTER_KEY.Tab]: 'citizen-reports' });
                    void navigate({ search: { [FILTER_KEY.ViewBy]: value, [FILTER_KEY.Tab]: 'citizen-reports' } });
                    setFilteringIsExpanded(false);
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
        <div className='flex justify-end gap-4 px-6 h-9'>
          <FilteringIcon filteringIsExpanded={filteringIsExpanded} setFilteringIsExpanded={setFilteringIsExpanded} />
          <CitizenReportsColumnVisibilitySelector />
        </div>

        <Separator />

        {filteringIsExpanded && (
          <div>
            {byFilter === 'byEntry' && <CitizenReportsFiltersByEntry />}
            {byFilter === 'byForm' && <CitizenReportsFiltersByForm />}
          </div>
        )}
      </CardHeader>

      {byFilter === 'byEntry' && <CitizenReportsByEntryTable />}

      {byFilter === 'byForm' && <CitizenReportsAggregatedByFormTable />}
    </Card>
  );
}
