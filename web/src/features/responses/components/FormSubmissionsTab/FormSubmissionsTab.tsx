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
import { getRouteApi } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { useState, type ChangeEvent } from 'react';
import { ExportedDataType } from '../../models/data-export';
import type { FormSubmissionsViewBy } from '../../utils/column-visibility-options';
import { ColumnsVisibilitySelector } from '../ColumnsVisibilitySelector/ColumnsVisibilitySelector';
import { ExportDataButton } from '../ExportDataButton/ExportDataButton';
import { FormsFiltersByEntry } from '../FormsFiltersByEntry/FormsFiltersByEntry';
import { FormsFiltersByObserver } from '../FormsFiltersByObserver/FormsFiltersByObserver';
import { FormsTableByObserver } from '../FormsTableByObserver/FormsTableByObserver';
import { FormSubmissionsAggregatedByFormTable } from '../FormSubmissionsAggregatedByFormTable/FormSubmissionsAggregatedByFormTable';
import { FormSubmissionsByEntryTable } from '../FormSubmissionsByEntryTable/FormSubmissionsByEntryTable';

import { FunctionComponent } from '@/common/types';
import { FormsFiltersByForm } from '../FormsFiltersByForm/FormsFiltersByForm';

const routeApi = getRouteApi('/responses/');

const viewBy: Record<FormSubmissionsViewBy, string> = {
  byEntry: 'View by entry',
  byObserver: 'View aggregated by observer',
  byForm: 'View aggregated by form',
};

export default function FormSubmissionsTab(): FunctionComponent {
  const navigate = routeApi.useNavigate();
  const search = routeApi.useSearch();

  const { viewBy: byFilter } = search;

  const [isFiltering, setIsFiltering] = useState(() =>
    Object.keys(search).some((key) => key !== 'tab' && key !== 'viewBy')
  );

  const [searchText, setSearchText] = useState<string>('');
  const debouncedSearchText = useDebounce(searchText, 300);
  const setPrevSearch = useSetPrevSearch();
  const handleSearchInput = (ev: ChangeEvent<HTMLInputElement>): void => {
    const value = ev.currentTarget.value;
    if (!value || value.length >= 2) setSearchText(ev.currentTarget.value);
  };

  return (
    <Card>
      <CardHeader>
        <div className='flex items-center justify-between px-6'>
          <CardTitle>Form submissions</CardTitle>

          <div className='flex items-center gap-4'>
            <ExportDataButton exportedDataType={ExportedDataType.FormSubmissions} />

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

        <div className='flex justify-end gap-4 px-6'>
          <>
            <Input className='max-w-md' onChange={handleSearchInput} placeholder='Search' />
            <FunnelIcon
              className='w-[20px] text-purple-900 cursor-pointer'
              fill={isFiltering ? '#5F288D' : 'rgba(0,0,0,0)'}
              onClick={() => {
                setIsFiltering((prev) => !prev);
              }}
            />
          </>

          <ColumnsVisibilitySelector byFilter={byFilter ?? 'byEntry'} />
        </div>

        <Separator />

        {isFiltering && (
          <>
            {byFilter === 'byEntry' && <FormsFiltersByEntry />}

            {byFilter === 'byObserver' && <FormsFiltersByObserver />}
            {byFilter === 'byForm' && <FormsFiltersByForm />}
          </>
        )}
      </CardHeader>

      {byFilter === 'byEntry' && <FormSubmissionsByEntryTable searchText={debouncedSearchText} />}

      {byFilter === 'byObserver' && <FormsTableByObserver searchText={debouncedSearchText} />}

      {byFilter === 'byForm' && <FormSubmissionsAggregatedByFormTable />}
    </Card>
  );
}
