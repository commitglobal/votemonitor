import { ChevronDownIcon, Cog8ToothIcon, FunnelIcon } from '@heroicons/react/24/outline';
import { getRouteApi } from '@tanstack/react-router';
import { useDebounce } from '@uidotdev/usehooks';
import { type ChangeEvent, useState, type ReactElement, useCallback } from 'react';
import { CsvFileIcon } from '@/components/icons/CsvFileIcon';
import Layout from '@/components/layout/Layout';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import {
  DropdownMenu,
  DropdownMenuCheckboxItem,
  DropdownMenuContent,
  DropdownMenuLabel,
  DropdownMenuRadioGroup,
  DropdownMenuRadioItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { Input } from '@/components/ui/input';
import { Separator } from '@/components/ui/separator';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { useFormSubmissionsByForm } from '../../hooks/form-submissions-queries';
import { formSubmissionsByFormColumnDefs } from '../../utils/column-defs';
import {
  columnVisibilityOptions,
  formSubmissionsDefaultColumns,
  type FilterBy,
} from '../../utils/column-visibility-options';
import { FormsTableByEntry } from '../FormsTableByEntry/FormsTableByEntry';
import { FormsFiltersByEntry } from '../FormsFiltersByEntry/FormsFiltersByEntry';
import { FormsFiltersByObserver } from '../FormsFiltersByObserver/FormsFiltersByObserver';
import { FormsTableByObserver } from '../FormsTableByObserver/FormsTableByObserver';
import { QuickReports } from '../QuickReports/QuickReports';

const routeApi = getRouteApi('/responses/');

const viewBy: Record<FilterBy, string> = {
  byEntry: 'View by entry',
  byObserver: 'View by observer',
  byForm: 'View by responses',
};

export default function ResponsesDashboard(): ReactElement {
  const [byFilter, setByFilter] = useState<FilterBy>('byEntry');

  const navigate = routeApi.useNavigate();
  const search = routeApi.useSearch();
  const [isFiltering, setIsFiltering] = useState(() => Object.keys(search).some((key) => key !== 'tab'));

  const [columnsVisibility, setColumnsVisibility] = useState(formSubmissionsDefaultColumns.byEntry);

  const [searchText, setSearchText] = useState<string>('');
  const debouncedSearchText = useDebounce(searchText, 300);

  const handleSearchInput = (ev: ChangeEvent<HTMLInputElement>): void => {
    const value = ev.currentTarget.value;
    if (!value || value.length >= 2) setSearchText(ev.currentTarget.value);
  };

  const navigateToAggregatedForm = useCallback(
    (formId: string) => {
      void navigate({ to: '/responses/$formId/aggregated', params: { formId } });
    },
    [navigate]
  );

  return (
    <Layout title='Responses' subtitle='View all form answers and other issues reported by your observers.  '>
      <Tabs
        defaultValue={search.tab ?? 'form-answers'}
        onValueChange={(tab) => {
          void navigate({
            search(prev) {
              return { ...prev, tab };
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

                <div className='flex gap-4'>
                  <Button
                    className='bg-background hover:bg-purple-50 hover:text-purple-500 text-purple-900 flex gap-2'
                    variant='outline'>
                    <CsvFileIcon />
                    Export to csv
                  </Button>

                  <DropdownMenu>
                    <DropdownMenuTrigger>
                      <Badge className='text-purple-900 hover:bg-purple-50 hover:text-purple-500 h-8' variant='outline'>
                        {viewBy[byFilter]}

                        <ChevronDownIcon className='w-4 ml-2' />
                      </Badge>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent>
                      <DropdownMenuRadioGroup
                        onValueChange={(value) => {
                          setByFilter(value as FilterBy);
                          setColumnsVisibility(formSubmissionsDefaultColumns[value as FilterBy]);
                          void navigate({});
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

                <DropdownMenu>
                  <DropdownMenuTrigger>
                    <Cog8ToothIcon className='w-[20px] text-purple-900 cursor-pointer' />
                  </DropdownMenuTrigger>
                  <DropdownMenuContent>
                    <DropdownMenuLabel>Table columns</DropdownMenuLabel>
                    <DropdownMenuSeparator />
                    {columnVisibilityOptions[byFilter].map((option) => (
                      <DropdownMenuCheckboxItem
                        key={option.id}
                        checked={columnsVisibility[option.id]}
                        disabled={!option.enableHiding}
                        onCheckedChange={(checked) => {
                          setColumnsVisibility((prev) => ({ ...prev, [option.id]: checked }));
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
                  {byFilter === 'byEntry' && <FormsFiltersByEntry />}

                  {byFilter === 'byObserver' && <FormsFiltersByObserver />}
                </div>
              )}
            </CardHeader>

            {byFilter === 'byEntry' && (
              <FormsTableByEntry columnsVisibility={columnsVisibility} searchText={debouncedSearchText} />
            )}

            {byFilter === 'byObserver' && (
              <FormsTableByObserver columnsVisibility={columnsVisibility} searchText={debouncedSearchText} />
            )}

            <CardContent>
              {byFilter === 'byForm' && (
                <QueryParamsDataTable
                  columnVisibility={columnsVisibility}
                  columns={formSubmissionsByFormColumnDefs}
                  useQuery={useFormSubmissionsByForm}
                  onRowClick={navigateToAggregatedForm}
                />
              )}
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value='quick-reports'>
          <QuickReports />
        </TabsContent>
      </Tabs>
    </Layout>
  );
}
