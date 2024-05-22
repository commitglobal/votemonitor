import { authApi } from '@/common/auth-api';
import CreateDialog from '@/components/dialogs/CreateDialog';
import Layout from '@/components/layout/Layout';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Separator } from '@/components/ui/separator';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { useDialog } from '@/components/ui/use-dialog';
import { toast } from '@/components/ui/use-toast';
import { cn } from '@/lib/utils';
import { queryClient } from '@/main';
import { ChevronDownIcon, ChevronUpIcon, Cog8ToothIcon, EllipsisVerticalIcon, FunnelIcon } from '@heroicons/react/24/outline';
import { useMutation } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { ColumnDef, Row } from '@tanstack/react-table';
import { X } from 'lucide-react';
import { useState, type ReactElement, useCallback } from 'react';
import { FormTemplateBase, mapFormTemplateType } from '../../models/formTemplate';
import { formTemplatesKeys, useFormTemplates } from '../../queries';
import AddTranslationsDialog from './AddTranslationsDialog';
import CreateTemplateForm from './CreateTemplateForm';

export default function FormTemplatesDashboard(): ReactElement {
  const addTranslationsDialog = useDialog();

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
              {row.getIsExpanded() ? <ChevronUpIcon className='w-4 h-4 ml-auto opacity-50' /> : <ChevronDownIcon className='w-4 h-4 ml-auto opacity-50' />}
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

            <DropdownMenuItem onClick={() => navigateToFormTemplate(row.original.id, row.original.defaultLanguage)}>View</DropdownMenuItem>

            {
              row.depth === 0 ?
                <DropdownMenuItem onClick={() => navigateToEdit(row.original.id, row.original.defaultLanguage)}>Edit</DropdownMenuItem>
                : <DropdownMenuItem onClick={() => navigateToEditTranslation(row.original.id, row.original.defaultLanguage)}>Edit</DropdownMenuItem>
            }

            {
              row.depth === 0 ?
                <DropdownMenuItem onClick={() => handleEditTranslations(row.original)}>Add translations</DropdownMenuItem>
                : null
            }
            {row.depth === 0 ?
              <DropdownMenuItem className='text-red-600' onClick={() => handleDeleteFormTemplate(row.original.id)}>
                Delete template form
              </DropdownMenuItem>
              :
              <DropdownMenuItem className='text-red-600' onClick={() => handleDeleteTranslation(row.original.id, row.original.defaultLanguage)}>
                Delete translation
              </DropdownMenuItem>}
          </DropdownMenuContent>
        </DropdownMenu>)
    }
  ];

  const [searchText, setSearchText] = useState('');
  const [isFiltering, setFiltering] = useState(false);
  const [formTypeFilter, setFormType] = useState('');
  const [currentFormTemplate, setCurrentFormTemplate] = useState<FormTemplateBase | null>(null)
  const navigate = useNavigate();
  const handleSearchInput = (ev: React.FormEvent<HTMLInputElement>) => {
    setSearchText(ev.currentTarget.value);
  };

  const handleDeleteFormTemplate = (formTemplateId: string) => {
    deleteFormTemplateMutation.mutate(formTemplateId);
  };

  const handleEditTranslations = (formTemplate: FormTemplateBase) => {
    setCurrentFormTemplate(formTemplate);
    addTranslationsDialog.trigger();
  }

  const handleDeleteTranslation = (formTemplateId: string, translationToDelete: string) => {
    deleteTranslationMutation.mutate({ formTemplateId, languageCode: translationToDelete });
  };

  const navigateToFormTemplate = (formTemplateId: string, languageCode: string) => {
    navigate({ to: '/form-templates/$formTemplateId/$languageCode', params: { formTemplateId, languageCode } });
  };

  const navigateToEdit = (formTemplateId: string, languageCode: string) => {
    navigate({ to: '/form-templates/$formTemplateId/edit', params: { formTemplateId } });
  };

  const navigateToEditTranslation = (formTemplateId: string, languageCode: string) => {
    navigate({ to: '/form-templates/$formTemplateId/edit-translation/$languageCode', params: { formTemplateId, languageCode } });
  };

  const deleteTranslationMutation = useMutation({
    mutationFn: ({ formTemplateId, languageCode }: { formTemplateId: string; languageCode: string; }) => {
      return authApi.delete<void>(`/form-templates/${formTemplateId}/${languageCode}`);
    },

    onSuccess: () => {
      toast({
        title: 'Success',
        description: 'Translation deleted',
      });

      queryClient.invalidateQueries({ queryKey: formTemplatesKeys.all });
    },
  });

  const deleteFormTemplateMutation = useMutation({
    mutationFn: (formTemplateId: string) => {

      return authApi.delete<void>(
        `/form-templates/${formTemplateId}`
      );
    },
    onSuccess: () => {
      toast({
        title: 'Success',
        description: 'Template form deleted',
      });
      queryClient.invalidateQueries({ queryKey: formTemplatesKeys.all });
    },
  });

  const changeIsFiltering = () => {
    setFiltering((prev) => {
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
    if (originalRow.languages.length === 0) return undefined;

    // we need to have subrows only for translations
    return originalRow.languages
      .filter(languageCode => originalRow.defaultLanguage !== languageCode)
      .map(languageCode => ({
        ...originalRow,
        languages: [],
        code: `${originalRow.code} - ${languageCode}`,
        defaultLanguage: languageCode
      }));
  };

  const getRowClassName = (row: Row<FormTemplateBase>): string =>
    cn({ 'bg-secondary-300 bg-opacity-[.15]': row.depth === 1 });

  const rowClickHandler = useCallback((formTemplateId: string, defaultLanguage?: string) => {
    navigateToFormTemplate(formTemplateId, defaultLanguage ?? '');
  }, [navigateToFormTemplate]);

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
              <CardTitle className='flex flex-row justify-between items-center px-6'>
                <div className='text-xl'>
                  Observation template forms
                </div>
                <div>
                  <CreateDialog title='Create template form'>
                    <CreateTemplateForm />
                  </CreateDialog>
                </div>
              </CardTitle>
              <Separator />
              <div className='filters px-6 flex flex-row justify-end gap-4'>
                <div className='w-[400px]'><Input onChange={handleSearchInput} placeholder='Search' /></div>
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
                        Form type: {formTypeFilter}
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
              <QueryParamsDataTable
                columns={formTemplateColDefs}
                useQuery={useFormTemplates}
                getSubrows={getSubrows}
                getRowClassName={getRowClassName}
                onRowClick={rowClickHandler}
              />
              {!!currentFormTemplate && (
                <AddTranslationsDialog
                  {...addTranslationsDialog.dialogProps}
                  formTemplateId={currentFormTemplate.id}
                  languages={currentFormTemplate.languages}
                />
              )}
            </CardContent>
            <CardFooter className='flex justify-between'></CardFooter>
          </Card>
        </TabsContent>
        <TabsContent value='psi-forms'>
          <QueryParamsDataTable
            columns={formTemplateColDefs}
            useQuery={useFormTemplates}
            getSubrows={getSubrows}
            getRowClassName={getRowClassName}
            onRowClick={rowClickHandler}
          />
        </TabsContent>
      </Tabs>

    </Layout>
  );
}
