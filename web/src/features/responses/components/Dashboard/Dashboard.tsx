import { ChevronDownIcon, Cog8ToothIcon, FunnelIcon } from '@heroicons/react/24/outline';
import { useState, type ReactElement } from 'react';
import { CsvFileIcon } from '@/components/icons/CsvFileIcon';
import Layout from '@/components/layout/Layout';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuRadioGroup,
  DropdownMenuRadioItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { Input } from '@/components/ui/input';
import { Separator } from '@/components/ui/separator';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { formSubmissionsByEntryColumnDefs, formSubmissionsByEntryDefaultColumns } from '../../utils/column-defs';
import { useFormSubmissionsByEntry } from '../../hooks/form-submissions-queries';

type FilterBy = 'byEntry' | 'byObserver' | 'byForm';

const viewBy: Record<FilterBy, string> = {
  byEntry: 'View by entry',
  byObserver: 'View by observer',
  byForm: 'View by form',
};

export default function ResponsesDashboard(): ReactElement {
  const [byFilter, setByFilter] = useState<FilterBy>('byEntry');

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
                <Input className='w-[400px]' placeholder='Search' />
                <FunnelIcon className='w-[20px] text-purple-900 cursor-pointer' />
                <Cog8ToothIcon className='w-[20px] text-purple-900 cursor-pointer' />
              </div>
            </CardHeader>

            <CardContent>
              {byFilter === 'byEntry' && (
                <QueryParamsDataTable
                  columnVisibility={formSubmissionsByEntryDefaultColumns}
                  columns={formSubmissionsByEntryColumnDefs}
                  useQuery={useFormSubmissionsByEntry}
                />
              )}
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value='quick-reports'>quick reports</TabsContent>
      </Tabs>
    </Layout>
  );
}
