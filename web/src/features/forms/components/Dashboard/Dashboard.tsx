import { authApi } from '@/common/auth-api';
import CreateDialog from '@/components/dialogs/CreateDialog';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Separator } from '@/components/ui/separator';
import { useDialog } from '@/components/ui/use-dialog';
import { toast } from '@/components/ui/use-toast';
import { cn } from '@/lib/utils';
import { queryClient } from '@/main';
import { ChevronDownIcon, ChevronUpIcon, Cog8ToothIcon, EllipsisVerticalIcon, FunnelIcon } from '@heroicons/react/24/outline';
import { useMutation } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { ColumnDef, Row } from '@tanstack/react-table';
import { X } from 'lucide-react';
import { useState, type ReactElement } from 'react';
import { FormBase, FormStatus, mapFormType } from '../../models/form';
import { formsKeys, useForms } from '../../queries';
import AddTranslationsDialog from './AddTranslationsDialog';
import CreateForm from './CreateForm';

export default function FormsDashboard(): ReactElement {
  const addTranslationsDialog = useDialog();

  const formColDefs: ColumnDef<FormBase>[] = [
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
      accessorKey: 'formType',
      accessorFn: (row, _) => mapFormType(row.formType),
      enableSorting: false,
      header: ({ column }) => <DataTableColumnHeader title='Form type' column={column} />,
    },
    {
      accessorKey: 'defaultLanguage',
      enableSorting: false,
      header: ({ column }) => <DataTableColumnHeader title='Language' column={column} />,
    },
    {
      accessorKey: 'numberOfQuestions',
      enableSorting: false,
      header: ({ column }) => <DataTableColumnHeader title='Questions' column={column} />,
    },
    {
      accessorKey: 'status',
      enableSorting: false,
      header: ({ column }) => <DataTableColumnHeader title='Status' column={column} />,
      cell: ({ row }) => (
        <Badge
          className={cn({
            'text-slate-700 bg-slate-200': row.original.status === FormStatus.Drafted,
            'text-green-600 bg-green-200': row.original.status === FormStatus.Published,
            'text-yellow-600 bg-yellow-200': row.original.status === FormStatus.Archived,
          })}>
          {row.original.status}
        </Badge>
      ),
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

            <DropdownMenuItem onClick={() => navigateToForm(row.original.id, row.original.defaultLanguage)}>View</DropdownMenuItem>

            {
              row.depth === 0 ?
                <DropdownMenuItem disabled={row.original.status === FormStatus.Published} onClick={() => navigateToEdit(row.original.id, row.original.defaultLanguage)}>Edit</DropdownMenuItem>
                : <DropdownMenuItem disabled={row.original.status === FormStatus.Published} onClick={() => navigateToEditTranslation(row.original.id, row.original.defaultLanguage)}>Edit</DropdownMenuItem>
            }

            {
              row.depth === 0 ?
                <DropdownMenuItem disabled={row.original.status === FormStatus.Published} onClick={() => handleEditTranslations(row.original)}>Add translations</DropdownMenuItem>
                : null
            }
            {
              row.depth === 0 && row.original.status === FormStatus.Published ?
                <DropdownMenuItem onClick={() => hangleDraftForm(row.original)}>Draft</DropdownMenuItem>
                : null
            }
            {
              row.depth === 0 && row.original.status === FormStatus.Drafted ?
                <DropdownMenuItem onClick={() => handlePublishForm(row.original)}>Publish</DropdownMenuItem>
                : null
            }
            {row.depth === 0 ?
              <DropdownMenuItem disabled={row.original.status === FormStatus.Published} className='text-red-600' onClick={() => handleDeleteForm(row.original.id)}>
                Delete form
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
  const [currentForm, setCurrentForm] = useState<FormBase | null>(null)
  const navigate = useNavigate();
  const handleSearchInput = (ev: React.FormEvent<HTMLInputElement>) => {
    setSearchText(ev.currentTarget.value);
  };

  const handleDeleteForm = (formId: string) => {
    deleteFormMutation.mutate(formId);
  };

  const handleEditTranslations = (form: FormBase) => {
    setCurrentForm(form);
    addTranslationsDialog.trigger();
  }
  const hangleDraftForm = (form: FormBase) => {
    draftFormMutation.mutate(form.id);
  }

  const handlePublishForm = (form: FormBase) => {
    publishFormMutation.mutate(form.id);
  }

  const handleDeleteTranslation = (formId: string, translationToDelete: string) => {
    deleteTranslationMutation.mutate({ formId, languageCode: translationToDelete });
  };

  const navigateToForm = (formId: string, languageCode: string) => {
    navigate({ to: '/forms/$formId/$languageCode', params: { formId, languageCode } });
  };

  const navigateToEdit = (formId: string, languageCode: string) => {
    navigate({ to: '/forms/$formId/edit', params: { formId } });
  };

  const navigateToEditTranslation = (formId: string, languageCode: string) => {
    navigate({ to: '/forms/$formId/edit-translation/$languageCode', params: { formId, languageCode } });
  };

  const deleteTranslationMutation = useMutation({
    mutationKey: formsKeys.all,
    mutationFn: ({ formId, languageCode }: { formId: string; languageCode: string; }) => {
      const electionRoundId: string | null = localStorage.getItem('electionRoundId');

      return authApi.delete<void>(`/election-rounds/${electionRoundId}/forms/${formId}/${languageCode}`);
    },

    onSuccess: () => {
      toast({
        title: 'Success',
        description: 'Translation deleted',
      });

      queryClient.invalidateQueries({ queryKey: formsKeys.all });
    },
  });

  const publishFormMutation = useMutation({
    mutationKey: formsKeys.all,
    mutationFn: (formId: string) => {
      const electionRoundId: string | null = localStorage.getItem('electionRoundId');

      return authApi.post<void>(`/election-rounds/${electionRoundId}/forms/${formId}:publish`);
    },

    onSuccess: () => {
      toast({
        title: 'Success',
        description: 'Form published',
      });

      queryClient.invalidateQueries({ queryKey: formsKeys.all });
    },

    onError: (error)=>{
      // @ts-ignore
      if(error.response.status === 400){
        toast({
          title: 'Error publishing form',
          description: 'You are missing translations. Please translate all fields and try again',
          variant: 'destructive'
        });

       return  
      }
      toast({
        title: 'Error publishing form',
        description: 'Please contact tech support',
        variant: 'destructive'
      });
    }
  });

  const draftFormMutation = useMutation({
    mutationKey: formsKeys.all,
    mutationFn: (formId: string) => {
      const electionRoundId: string | null = localStorage.getItem('electionRoundId');

      return authApi.post<void>(`/election-rounds/${electionRoundId}/forms/${formId}:draft`);
    },

    onSuccess: () => {
      toast({
        title: 'Success',
        description: 'Form drafted',
      });

      queryClient.invalidateQueries({ queryKey: formsKeys.all });
    },
    onError: (error)=>{
      // @ts-ignore
      if(error.response.status === 409){
        toast({
          title: 'Error drafting form',
          description: 'Cannot draft a form with answers submitted',
          variant: 'destructive'
        });

       return  
      }
      toast({
        title: 'Error publishing form',
        description: 'Please contact tech support',
        variant: 'destructive'
      });
    }
  });

  const deleteFormMutation = useMutation({
    mutationKey: formsKeys.all,
    mutationFn: (formId: string) => {
      const electionRoundId: string | null = localStorage.getItem('electionRoundId');

      return authApi.delete<void>(
        `/election-rounds/${electionRoundId}/forms/${formId}`
      );
    },
    onSuccess: () => {
      toast({
        title: 'Success',
        description: 'Form deleted',
      });
      queryClient.invalidateQueries({ queryKey: formsKeys.all });
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

  const getSubrows = (originalRow: FormBase, index: number): undefined | FormBase[] => {
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

  const getRowClassName = (row: Row<FormBase>): string => cn({ 'bg-secondary-300 bg-opacity-[.15]': row.depth === 1 });

  return (
      <Card className='w-full pt-0'>
        <CardHeader className='flex flex-column gap-2'>
          <CardTitle className='flex flex-row justify-between items-center px-6'>
            <div className='text-xl'>
              Observation forms
            </div>
            <div>
              <CreateDialog title='Create form'>
                <CreateForm />
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
                    <SelectItem value='Other'>Other</SelectItem>
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
          <QueryParamsDataTable
            columns={formColDefs}
            useQuery={useForms}
            getSubrows={getSubrows}
            getRowClassName={getRowClassName}
          />
          {!!currentForm && (
            <AddTranslationsDialog
              {...addTranslationsDialog.dialogProps}
              formId={currentForm.id}
              languages={currentForm.languages}
            />
          )}
        </CardContent>
      </Card>
  );
}
