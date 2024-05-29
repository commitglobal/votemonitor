import { ChevronDownIcon, FunnelIcon } from '@heroicons/react/24/outline';
import { getRouteApi } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { type ChangeEvent, useState, type ReactElement } from 'react';
import Layout from '@/components/layout/Layout';
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
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { ExportedDataType } from '../../models/data-export';
import type { FilterBy } from '../../utils/column-visibility-options';
import { ColumnsVisibilitySelector } from '../ColumnsVisibilitySelector/ColumnsVisibilitySelector';
import { ExportDataButton } from '../ExportDataButton/ExportDataButton';
import { FormsFiltersByEntry } from '../FormsFiltersByEntry/FormsFiltersByEntry';
import { FormsFiltersByObserver } from '../FormsFiltersByObserver/FormsFiltersByObserver';
import { FormsTableByEntry } from '../FormsTableByEntry/FormsTableByEntry';
import { FormsTableByForm } from '../FormsTableByForm/FormsTableByForm';
import { FormsTableByObserver } from '../FormsTableByObserver/FormsTableByObserver';
import { QuickReports } from '../QuickReports/QuickReports';
import { useSetPrevSearch } from '@/common/prev-search-store';

const routeApi = getRouteApi('/responses/');

const viewBy: Record<FilterBy, string> = {
  byEntry: 'View by entry',
  byObserver: 'View by observer',
  byForm: 'View by responses',
};

export default function ResponsesDashboard(): ReactElement {
  const navigate = routeApi.useNavigate();
  const search = routeApi.useSearch();
  const [isFiltering, setIsFiltering] = useState(() =>
    Object.keys(search).some((key) => key !== 'tab' && key !== 'viewBy')
  );

  const [searchText, setSearchText] = useState<string>('');
  const debouncedSearchText = useDebounce(searchText, 300);

  const handleSearchInput = (ev: ChangeEvent<HTMLInputElement>): void => {
    const value = ev.currentTarget.value;
    if (!value || value.length >= 2) setSearchText(ev.currentTarget.value);
  };

  const { viewBy: byFilter, tab } = search;

  const setPrevSearch = useSetPrevSearch();

  return (
    <Layout title='Responses' subtitle='View all form answers and other issues reported by your observers.  '>
      <Tabs
        defaultValue={tab ?? 'form-answers'}
        onValueChange={(tab) => {
          void navigate({
            search(prev) {
              const newSearch = { ...prev, tab };
              setPrevSearch(newSearch);
              return newSearch;
            },
          });
        }}>
        <TabsList className='grid grid-cols-2 bg-gray-200 w-[400px] mb-4'>
          <TabsTrigger value='form-answers'>Form answers</TabsTrigger>
          <TabsTrigger value='quick-reports'>Quick reports</TabsTrigger>
        </TabsList>

        <TabsContent value='form-answers'>
          <Card>
            <CardHeader>
              <div className='flex justify-between items-center px-6'>
                <CardTitle>All forms</CardTitle>

                <div className='flex gap-4 items-center'>
                  <ExportDataButton exportedDataType={ExportedDataType.FormSubmissions} />

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
                {byFilter !== 'byForm' && (
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
                )}

                <ColumnsVisibilitySelector byFilter={byFilter ?? 'byEntry'} />
              </div>

              <Separator />

              {isFiltering && (
                <div className='grid grid-cols-6 gap-4 items-center'>
                  {byFilter === 'byEntry' && <FormsFiltersByEntry />}

                  {byFilter === 'byObserver' && <FormsFiltersByObserver />}
                </div>
              )}
            </CardHeader>

            {byFilter === 'byEntry' && <FormsTableByEntry searchText={debouncedSearchText} />}

            {byFilter === 'byObserver' && <FormsTableByObserver searchText={debouncedSearchText} />}

            {byFilter === 'byForm' && <FormsTableByForm />}
          </Card>
        </TabsContent>

        <TabsContent value='quick-reports'>
          <QuickReports />
        </TabsContent>
      </Tabs>
    </Layout>
  );
}
