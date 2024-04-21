import { ChevronDownIcon, Cog8ToothIcon, FunnelIcon } from '@heroicons/react/24/outline';
import { useDebounce } from '@uidotdev/usehooks';
import { type ChangeEvent, useState, type ReactElement } from 'react';
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
import {
  useFormSubmissionsByEntry,
  useFormSubmissionsByForm,
  useFormSubmissionsByObserver,
} from '../../hooks/form-submissions-queries';
import {
  formSubmissionsByEntryColumnDefs,
  formSubmissionsByFormColumnDefs,
  formSubmissionsByObserverColumnDefs,
} from '../../utils/column-defs';
import {
  columnVisibilityOptions,
  formSubmissionsDefaultColumns,
  type FilterBy,
} from '../../utils/column-visibility-options';

const viewBy: Record<FilterBy, string> = {
  byEntry: 'View by entry',
  byObserver: 'View by observer',
  byForm: 'View by form',
};

export default function ResponsesDashboard(): ReactElement {
  const [byFilter, setByFilter] = useState<FilterBy>('byEntry');

  const [submissionsByEntryColumnVisibility, setSubmissionsByEntryColumnVisibility] = useState(
    formSubmissionsDefaultColumns.byEntry
  );

  const [searchText, setSearchText] = useState('');
  const debouncedSearchText = useDebounce(searchText, 300);

  const handleSearchInput = (ev: ChangeEvent<HTMLInputElement>): void => {
    const value = ev.currentTarget.value;
    if (!value || value.length >= 2) setSearchText(ev.currentTarget.value);
  };

  return (
    <Layout title='Responses' subtitle='View all form answers and other issues reported by your observers.  '>
      <Tabs defaultValue='form-answers'>
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
                          setSubmissionsByEntryColumnVisibility(formSubmissionsDefaultColumns[value as FilterBy]);
                        }}
                        value={byFilter}>
                        {Object.entries(viewBy).map(([value, label]) => (
                          <DropdownMenuRadioItem value={value}>{label}</DropdownMenuRadioItem>
                        ))}
                      </DropdownMenuRadioGroup>
                    </DropdownMenuContent>
                  </DropdownMenu>
                </div>
              </div>

              <Separator />

              <div className='px-6 flex justify-end gap-4'>
                <Input className='w-[400px]' onChange={handleSearchInput} placeholder='Search' />
                <FunnelIcon className='w-[20px] text-purple-900 cursor-pointer' />

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
                        checked={submissionsByEntryColumnVisibility[option.id]}
                        disabled={!option.enableHiding}
                        onCheckedChange={(checked) => {
                          setSubmissionsByEntryColumnVisibility((prev) => ({ ...prev, [option.id]: checked }));
                        }}>
                        {option.label}
                      </DropdownMenuCheckboxItem>
                    ))}
                  </DropdownMenuContent>
                </DropdownMenu>
              </div>
            </CardHeader>

            <CardContent>
              {byFilter === 'byEntry' && (
                <QueryParamsDataTable
                  columnVisibility={submissionsByEntryColumnVisibility}
                  columns={formSubmissionsByEntryColumnDefs}
                  useQuery={useFormSubmissionsByEntry}
                  queryParams={{ formCodeFilter: debouncedSearchText }}
                />
              )}

              {byFilter === 'byObserver' && (
                <QueryParamsDataTable
                  columnVisibility={submissionsByEntryColumnVisibility}
                  columns={formSubmissionsByObserverColumnDefs}
                  useQuery={useFormSubmissionsByObserver}
                  queryParams={{ observerNameFilter: debouncedSearchText }}
                />
              )}

              {byFilter === 'byForm' && (
                <QueryParamsDataTable
                  columnVisibility={submissionsByEntryColumnVisibility}
                  columns={formSubmissionsByFormColumnDefs}
                  useQuery={useFormSubmissionsByForm}
                  queryParams={{ formCodeFilter: debouncedSearchText }}
                />
              )}
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value='quick-reports'>TBD</TabsContent>
      </Tabs>
    </Layout>
  );
}
