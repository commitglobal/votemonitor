import { useState, type ReactElement, useRef, CSSProperties } from 'react';
import { type UseQueryResult, useQuery, useMutation } from '@tanstack/react-query';
import { type DataTableParameters, type PageResponse } from '@/common/types';
import { authApi } from '@/common/auth-api';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import Layout from '@/components/layout/Layout';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { ColumnDef, Row } from '@tanstack/react-table';
import { EllipsisVerticalIcon, FunnelIcon, Cog8ToothIcon, ChevronUpIcon, ChevronDownIcon  } from '@heroicons/react/24/outline';
import { X } from 'lucide-react';
import {
  DropdownMenu,
  DropdownMenuCheckboxItem,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Separator } from '@/components/ui/separator';
import { Badge } from '@/components/ui/badge';
import { Input } from '@/components/ui/input';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog';
import { queryClient } from '@/main';
import { useNavigate } from '@tanstack/react-router';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { FormTemplateBase, FormTemplateType, mapFormTemplateType } from '../../models/formTemplate';
import { useFormTemplates } from '../../queries';
import { useTranslation } from 'react-i18next';
import clsx from 'clsx';



export default function FormTemplatesDashboard(): ReactElement {
  const formTemplateColDefs: ColumnDef<FormTemplateBase>[] = [
    {
      header: '',
      id: 'colapse',
      cell: ({ row, getValue }) => (
        <div>
          {row.getCanExpand() ? (
                <button
                  {...{
                    onClick: row.getToggleExpandedHandler(),
                    style: { cursor: 'pointer' },
                  }}
                >
                  {row.getIsExpanded() ? <ChevronUpIcon className='w-4 h-4 ml-auto opacity-50'/> : <ChevronDownIcon className='w-4 h-4 ml-auto opacity-50'/>}
                </button>
              ) : (
                ''
              )}
          {getValue<boolean>()}
        </div>
      ),
    },
    {
      accessorKey: 'code',
      enableSorting: true,
      header: ({ column }) => <DataTableColumnHeader title='Form Code' column={column} />,
    },
    {
      id: 'name',
      accessorFn: (row, _) => row.name[row.defaultLanguage],
      enableSorting: false,
      header: ({ column }) => <DataTableColumnHeader title='Name' column={column} />,
    },
    {
      accessorKey: 'formTemplateType',
      accessorFn: (row, _) => mapFormTemplateType(row.formTemplateType),
      enableSorting: false,
      header: ({ column }) => <DataTableColumnHeader title='Form type' column={column} />,
    },
    {
      accessorKey: 'defaultLanguage',
      enableSorting: false,
      header: ({ column }) => <DataTableColumnHeader title='Language' column={column} />,
    },
    {
      accessorKey: 'status',
      enableSorting: false,
      header: ({ column }) => <DataTableColumnHeader title='Status' column={column} />,
    },
    {
      header: '',
      accessorKey: 'action',
      enableSorting: false,
      cell: ({ row }) => (
        <DropdownMenu>
          <DropdownMenuTrigger>
            <EllipsisVerticalIcon className='w-[24px] h-[24px] tex t-purple-600' />
          </DropdownMenuTrigger>
          <DropdownMenuContent>
            <DropdownMenuItem onClick={() => navigateToFormTemplate(row.original.id)}>View</DropdownMenuItem>
            <DropdownMenuItem onClick={() => navigateToEdit(row.original.id)}>Edit</DropdownMenuItem>
            <DropdownMenuItem className='text-red-600' onClick={() => handleDelete(row.original.id)}>
              Delete
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      ),
    }
  ];

  const [searchText, setSearchText] = useState('');
  const [isFiltering, setFiltering] = useState(false);
  const [formTypeFilter, setFormType] = useState('');

  const navigate = useNavigate();
  const handleSearchInput = (ev: React.FormEvent<HTMLInputElement>) => {
    setSearchText(ev.currentTarget.value);
  };

  const handleDelete = (formTemplateId: string) => {
    deleteMutation.mutate(formTemplateId);
  };

  const navigateToFormTemplate = (formTemplateId: string) => {
    navigate({ to: '/form-templates/$formTemplateId', params: { formTemplateId } });
  };
  const navigateToEdit = (formTemplateId: string) => {
    navigate({ to: '/form-templates/$formTemplateId/edit', params: { formTemplateId } });
  };

  const deleteMutation = useMutation({
    mutationFn: (formTemplateId: string) => {

      return authApi.delete<void>(
        `/form-templates/${formTemplateId}`
      );
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['form-templates'] });
    },
  });

  const changeIsFiltering = () => {
    setFiltering((prev) => {
      console.log(prev);
      return !prev;
    });
  };

  const handleFormTypeFilter = (status: string) => {
    setFormType(status);
  };

  const resetFilters = () => {
    setFormType('');
  };

  const getSubrows = (originalRow: FormTemplateBase, index: number): undefined | FormTemplateBase[] => {
    if(originalRow.languages.length === 0) return undefined;

    // we need to have subrows only for translations
    return originalRow.languages
      .filter(languageCode=> originalRow.defaultLanguage !== languageCode)
      .map(languageCode => ({
        ...originalRow,
        languages: [],
        code: `${originalRow.code} - ${languageCode}`,
        defaultLanguage: languageCode
      }));
  }

  const getRowClassName = (row: Row<FormTemplateBase>): string => (clsx( { 'bg-secondary-300 bg-opacity-[.15]': row.depth === 1 }));

  return (
    <Layout
      title={'Template forms'}
      subtitle='Create templates for observation forms. These forms will be assigned to election events.'>
      <Tabs defaultValue='template-forms'>
        <TabsList className='grid grid-cols-2 bg-gray-200 w-[400px] mb-4 opaci'>
          <TabsTrigger value='template-forms'>Observation template forms</TabsTrigger>
          <TabsTrigger value='psi-forms'>PSI template forms</TabsTrigger>
        </TabsList>
        <TabsContent value='template-forms'>
          <Card className='w-full pt-0'>
            <CardHeader className='flex flex-column gap-2'>
              <div className='flex flex-row justify-between items-center px-6'>
                <CardTitle className='text-xl'>Observation template forms</CardTitle>
              </div>
              <Separator />
              <div className='filters px-6 flex flex-row justify-end gap-4'>
                <Input onChange={handleSearchInput} className='w-[400px]' placeholder='Search' />
                <FunnelIcon
                  onClick={changeIsFiltering}
                  className='w-[20px] text-purple-900 cursor-pointer'
                  fill={isFiltering ? '#5F288D' : 'rgba(0,0,0,0)'}
                />
                <Cog8ToothIcon className='w-[20px] text-purple-900' />
              </div>
              <Separator />
              {isFiltering ? (
                <div className='table-filters flex flex-row gap-4 items-center'>
                  <Select value={formTypeFilter} onValueChange={handleFormTypeFilter}>
                    <SelectTrigger className='w-[180px]'>
                      <SelectValue placeholder='Form type' />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectGroup>
                        <SelectItem value='Opening'>Opening</SelectItem>
                        <SelectItem value='Voting'>Voting</SelectItem>
                        <SelectItem value='ClosingAndCounting'>Closing And Counting</SelectItem>
                      </SelectGroup>
                    </SelectContent>
                  </Select>
                  <Button variant='ghost-primary'>
                    <span onClick={resetFilters} className='text-base text-purple-900'>
                      Reset filters
                    </span>
                  </Button>
                  <div className='flex flex-row gap-2 flex-wrap'>
                    {formTypeFilter && (
                      <span
                        onClick={() => handleFormTypeFilter('')}
                        className='rounded-full cursor-pointer py-1 px-4 bg-purple-100 text-sm text-purple-900 font-medium flex items-center gap-2'>
                        Observer status: {formTypeFilter}
                        <X size={14} />
                      </span>
                    )}
                  </div>
                </div>
              ) : (
                ''
              )}
            </CardHeader>
            <CardContent>
              <QueryParamsDataTable columns={formTemplateColDefs} useQuery={useFormTemplates} getSubrows={getSubrows} getRowClassName={getRowClassName}/>
            </CardContent>
            <CardFooter className='flex justify-between'></CardFooter>
          </Card>
        </TabsContent>
        <TabsContent value='psi-forms'>
          <QueryParamsDataTable columns={formTemplateColDefs} useQuery={useFormTemplates} getSubrows={getSubrows} getRowClassName={getRowClassName} />
        </TabsContent>
      </Tabs>
    </Layout>
  );
}
