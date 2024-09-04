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
import { getRouteApi } from '@tanstack/react-router';
import { useState } from 'react';
import { ExportedDataType } from '../../models/data-export';
import type { CitizenReportsViewBy } from '../../utils/column-visibility-options';
import { ColumnsVisibilitySelector } from '../ColumnsVisibilitySelector/ColumnsVisibilitySelector';
import { ExportDataButton } from '../ExportDataButton/ExportDataButton';
import { FormsFiltersByEntry } from '../FormsFiltersByEntry/FormsFiltersByEntry';
import { FormSubmissionsAggregatedByFormTable } from '../FormSubmissionsAggregatedByFormTable/FormSubmissionsAggregatedByFormTable';
import { FormSubmissionsByEntryTable } from '../FormSubmissionsByEntryTable/FormSubmissionsByEntryTable';

import { FunctionComponent } from "@/common/types";
import { CitizenReportsByEntryTable } from '../CitizenReportsByEntryTable/CitizenReportsByEntryTable';

const routeApi = getRouteApi('/responses/');

const viewBy: Record<string, string> = {
  'byEntry': 'View by entry',
  'byForm': 'View aggregated by form',
};

export function CitizenReportsTab(): FunctionComponent {
  const navigate = routeApi.useNavigate();
  const search = routeApi.useSearch();

  const { viewBy: byFilter } = search;

  const [isFiltering, setIsFiltering] = useState(() =>
    Object.keys(search).some((key) => key !== 'tab' && key !== 'viewBy')
  );

  const setPrevSearch = useSetPrevSearch();

  return (
    <Card>
      <CardHeader>
        <div className='flex justify-between items-center px-6'>
          <CardTitle>Citizen reports</CardTitle>

          <div className='flex gap-4 items-center'>
            <ExportDataButton exportedDataType={ExportedDataType.CitizenReports} />

            <DropdownMenu>
              <DropdownMenuTrigger>
                <Badge className='text-purple-900 hover:bg-purple-50 hover:text-purple-500 h-8' variant='outline'>
                  {viewBy[byFilter ?? 'byEntry']}

                  <ChevronDownIcon className='w-4 ml-2' />
                </Badge>
              </DropdownMenuTrigger>
              <DropdownMenuContent>
                <DropdownMenuRadioGroup
                  onValueChange={(value) => {
                    setPrevSearch({ viewBy: value });
                    void navigate({ search: { viewBy: value } });
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

        <div className='px-6 flex justify-end gap-4'>
          <ColumnsVisibilitySelector byFilter={byFilter ?? 'byEntry'} />
        </div>

        <Separator />

        {isFiltering && (
          <div className='grid grid-cols-6 gap-4 items-center'>
            {byFilter === 'byEntry' && <FormsFiltersByEntry />}

          </div>
        )}
      </CardHeader>

      {byFilter === 'byEntry' && <CitizenReportsByEntryTable />}

      {byFilter === 'byForm' && <FormSubmissionsAggregatedByFormTable />}
    </Card>
  );
}
