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
import { useEffect, useMemo, useState } from 'react';
import { ExportedDataType } from '../../models/data-export';
import { ExportDataButton } from '../ExportDataButton/ExportDataButton';

import { FunctionComponent } from '@/common/types';
import { FILTER_KEY } from '@/features/filtering/filtering-enums';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { Route } from '@/routes/responses';
import { CitizenReportsAggregatedByFormTable } from '../CitizenReportsAggregatedByFormTable/CitizenReportsAggregatedByFormTable';
import { CitizenReportsByEntryTable } from '../CitizenReportsByEntryTable/CitizenReportsByEntryTable';
import { CitizenReportsFiltersByEntry } from '../CitizenReportsFiltersByEntry/CitizenReportsFiltersByEntry';

const viewBy: Record<string, string> = {
  byEntry: 'View by entry',
  byForm: 'View aggregated by form',
};

export function CitizenReportsTab(): FunctionComponent {
  const navigate = useNavigate();
  const search = Route.useSearch();
  const { filteringIsActive } = useFilteringContainer();

  const { viewBy: byFilter } = search;

  const [isFiltering, setIsFiltering] = useState(filteringIsActive);

  const setPrevSearch = useSetPrevSearch();
  useEffect(() => {
    if (byFilter === 'byEntry') {
      setPrevSearch({ [FILTER_KEY.ViewBy]: 'byEntry' });
      navigate({ to: '.', search: { [FILTER_KEY.ViewBy]: 'byEntry' } });
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
        <div className='flex items-center justify-between px-6'>
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
                    navigate({
                      to: '.',
                      replace: true,
                      search: { [FILTER_KEY.ViewBy]: value, [FILTER_KEY.Tab]: 'citizen-reports' },
                    });
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

        {isFiltering && (
          <div className='grid items-center grid-cols-6 gap-4'>
            {byFilter === 'byEntry' && <CitizenReportsFiltersByEntry />}
          </div>
        )}
      </CardHeader>

      {byFilter === 'byEntry' && <CitizenReportsByEntryTable />}

      {byFilter === 'byForm' && <CitizenReportsAggregatedByFormTable />}
    </Card>
  );
}
