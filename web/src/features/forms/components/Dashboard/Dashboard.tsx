import { authApi } from '@/common/auth-api';
import { DateTimeFormat } from '@/common/formats';
import { ZFormType, ZTranslationStatus } from '@/common/types';
import CreateDialog from '@/components/dialogs/CreateDialog';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { useConfirm } from '@/components/ui/alert-dialog-provider';
import { Badge } from '@/components/ui/badge';
import { Button, buttonVariants } from '@/components/ui/button';
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
import { toast } from '@/components/ui/use-toast';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useLanguages } from '@/features/languages/queries';
import { cn, mapFormType } from '@/lib/utils';
import { queryClient } from '@/main';
import { ChevronDownIcon, ChevronUpIcon, Cog8ToothIcon, EllipsisVerticalIcon, FunnelIcon } from '@heroicons/react/24/outline';
import { useMutation } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { ColumnDef, Row } from '@tanstack/react-table';
import { format } from 'date-fns';
import { X } from 'lucide-react';
import { useState, type ReactElement } from 'react';
import { FormBase, FormStatus } from '../../models/form';
import { formsKeys, useForms } from '../../queries';
import AddTranslationsDialog, { useAddTranslationsDialog } from './AddTranslationsDialog';
import CreateForm from './CreateForm';
import i18n from '@/i18n';

export default function FormsDashboard(): ReactElement {
  const addTranslationsDialog = useAddTranslationsDialog();
  const confirm = useConfirm();
  const { data: languages } = useLanguages();
  const currentElectionRoundId = useCurrentElectionRoundStore(s => s.currentElectionRoundId);
  const isMonitoringNgoForCitizenReporting = useCurrentElectionRoundStore(s => s.isMonitoringNgoForCitizenReporting);

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
      enableResizing: false
    },
    {
      accessorKey: 'code',
      enableSorting: true,
      header: ({ column }) => <DataTableColumnHeader title={i18n.t('electionEvent.observerForms.headers.formCode')} column={column} />,
    },
    {
      id: 'name',
      accessorFn: (row, _) => row.name[row.defaultLanguage],
      enableSorting: false,
      header: ({ column }) => <DataTableColumnHeader title={i18n.t('electionEvent.observerForms.headers.name')} column={column} />,
    },
    {
      accessorKey: 'formType',
      accessorFn: (row, _) => mapFormType(row.formType),
      enableSorting: false,
      enableResizing: false,
      header: ({ column }) => <DataTableColumnHeader title={i18n.t('electionEvent.observerForms.headers.formType')} column={column} />,
      cell: ({ row }) => (row.depth === 0 ? row.original.formType : ''),
    },
    {
      accessorKey: 'defaultLanguage',
      enableSorting: false,
      enableResizing: false,
      header: ({ column }) => <DataTableColumnHeader title={i18n.t('electionEvent.observerForms.headers.language')} column={column} />,
    },
    {
      accessorKey: 'numberOfQuestions',
      enableSorting: false,
      enableResizing: false,
      header: ({ column }) => <DataTableColumnHeader title={i18n.t('electionEvent.observerForms.headers.questions')} column={column} />,
      cell: ({ row }) => (row.depth === 0 ? row.original.numberOfQuestions : ''),
    },
    {
      accessorKey: 'status',
      enableSorting: false,
      enableResizing: false,
      header: ({ column }) => <DataTableColumnHeader title={i18n.t('electionEvent.observerForms.headers.status')} column={column} />,
      cell: ({ row }) => {
        const form = row.original;

        return row.depth === 0 ?
          <Badge
            className={cn({
              'text-slate-700 bg-slate-200': form.status === FormStatus.Drafted,
              'text-green-600 bg-green-200': form.status === FormStatus.Published,
              'text-yellow-600 bg-yellow-200': form.status === FormStatus.Obsolete
            })}>
            {form.status}
          </Badge>
          : <Badge
            className={cn({
              'text-green-600 bg-green-200': form.languagesTranslationStatus[form.defaultLanguage] === ZTranslationStatus.enum.Translated,
              'text-yellow-600 bg-yellow-200': form.languagesTranslationStatus[form.defaultLanguage] === ZTranslationStatus.enum.MissingTranslations,
              'text-slate-700 bg-slate-200': form.languagesTranslationStatus[form.defaultLanguage] === undefined
            })}>
            {form.languagesTranslationStatus[form.defaultLanguage] === ZTranslationStatus.enum.Translated ? 'Translated' : form.languagesTranslationStatus[form.defaultLanguage] === ZTranslationStatus.enum.MissingTranslations ? 'Missing translation' : 'Unknown'}
          </Badge>
      },
    },
    {
      accessorKey: 'lastUpdatedOn',
      enableSorting: false,
      enableResizing: false,
      header: ({ column }) => <DataTableColumnHeader title={i18n.t('electionEvent.observerForms.headers.updatedOn')} column={column} />,
      cell: ({ row }) => (
        row.depth === 0 ?
          <div>
            <p>{row.original.lastModifiedOn ? format(row.original.lastModifiedOn, DateTimeFormat) : format(row.original.createdOn, DateTimeFormat)} </p>
          </div>
          : <></>
      ),
    },
    {
      header: '',
      accessorKey: 'action',
      enableSorting: false,
      enableResizing: false,
      cell: ({ row }) => (
        <DropdownMenu modal={false}>
          <DropdownMenuTrigger asChild>
            <EllipsisVerticalIcon className='w-[24px] h-[24px] tex t-purple-600' />
          </DropdownMenuTrigger>
          <DropdownMenuContent>
            <DropdownMenuItem onClick={() => navigateToForm(row.original.id, row.original.defaultLanguage)}>View</DropdownMenuItem>

            {
              row.depth === 0 ?
                <DropdownMenuItem disabled={row.original.status !== FormStatus.Drafted} onClick={() => navigateToEdit(row.original.id)}>Edit</DropdownMenuItem>
                : <DropdownMenuItem disabled={row.original.status !== FormStatus.Drafted} onClick={() => navigateToEditTranslation(row.original.id, row.original.defaultLanguage)}>Edit</DropdownMenuItem>
            }

            {
              row.depth === 0 ?
                <DropdownMenuItem disabled={row.original.status !== FormStatus.Drafted} onClick={() => addTranslationsDialog.trigger(row.original.id, row.original.languages)}>Add translations</DropdownMenuItem>
                : null
            }
            {
              row.depth === 0 && row.original.status === FormStatus.Published ?
                <DropdownMenuItem onClick={() => handleObsoleteForm(row.original)}>Obsolete</DropdownMenuItem>
                : null
            }
            {
              row.depth === 0 && row.original.status === FormStatus.Drafted ?
                <DropdownMenuItem onClick={() => handlePublishForm(row.original)}>Publish</DropdownMenuItem>
                : null
            }
            {
              row.depth === 0 ?
                <DropdownMenuItem onClick={() => handleDuplicateForm(row.original)}>Duplicate</DropdownMenuItem>
                : null
            }
            {row.depth === 0 ?
              <DropdownMenuItem className='text-red-600' onClick={async () => {
                if (await confirm({
                  title: `Delete form ${row.original.code}?`,
                  body: row.original.status === FormStatus.Published ? <>Please note that this form is published and may contain associated data. Deleting this form could result in the loss of any submitted answers from your observers. Once deleted, <b>the associated data cannot be retrieved</b></> : 'This action is permanent and cannot be undone. Once deleted, this form cannot be retrieved.',
                  actionButton: 'Delete',
                  actionButtonClass: buttonVariants({ variant: "destructive" }),
                  cancelButton: 'Cancel',
                })) {
                  deleteFormMutation.mutate({ electionRoundId: currentElectionRoundId, formId: row.original.id });
                }
              }}>
                Delete form
              </DropdownMenuItem>
              :
              <DropdownMenuItem className='text-red-600' onClick={async () => {
                const languageCode = row.original.defaultLanguage;
                const language = languages?.find(l => languageCode === l.code);
                const fullName = language ? `${language.name} / ${language.nativeName}` : '';

                if (await confirm({
                  title: `Delete translation ${fullName}?`,
                  body: 'This action is permanent and cannot be undone. Once deleted, this translation cannot be retrieved.',
                  actionButton: 'Delete',
                  actionButtonClass: buttonVariants({ variant: "destructive" }),
                  cancelButton: 'Cancel',
                })) {
                  deleteTranslationMutation.mutate({ electionRoundId: currentElectionRoundId, formId: row.original.id, languageCode });
                }
              }}>Delete translation</DropdownMenuItem>}
          </DropdownMenuContent>
        </DropdownMenu>)
    }
  ];

  const [searchText, setSearchText] = useState('');
  const [isFiltering, setFiltering] = useState(false);
  const [formTypeFilter, setFormType] = useState('');
  const navigate = useNavigate();
  const handleSearchInput = (ev: React.FormEvent<HTMLInputElement>) => {
    setSearchText(ev.currentTarget.value);
  };

  const handleObsoleteForm = (form: FormBase) => {
    obsoleteFormMutation.mutate({ electionRoundId: currentElectionRoundId, formId: form.id });
  }

  const handlePublishForm = (form: FormBase) => {
    publishFormMutation.mutate({ electionRoundId: currentElectionRoundId, formId: form.id });
  }

  const handleDuplicateForm = (form: FormBase) => {
    duplicateFormMutation.mutate({ electionRoundId: currentElectionRoundId, formId: form.id });
  }

  const navigateToForm = (formId: string, languageCode: string) => {
    navigate({ to: '/forms/$formId/$languageCode', params: { formId, languageCode } });
  };

  const navigateToEdit = (formId: string) => {
    navigate({ to: '/forms/$formId/edit', params: { formId } });
  };

  const navigateToEditTranslation = (formId: string, languageCode: string) => {
    navigate({ to: '/forms/$formId/edit-translation/$languageCode', params: { formId, languageCode } });
  };

  const deleteTranslationMutation = useMutation({
    mutationKey: formsKeys.all,
    mutationFn: ({ electionRoundId, formId, languageCode }: { electionRoundId: string; formId: string; languageCode: string; }) => {
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
    mutationFn: ({ electionRoundId, formId }: { electionRoundId: string; formId: string }) => {
      return authApi.post<void>(`/election-rounds/${electionRoundId}/forms/${formId}:publish`);
    },

    onSuccess: () => {
      toast({
        title: 'Success',
        description: 'Form published',
      });

      queryClient.invalidateQueries({ queryKey: formsKeys.all });
    },

    onError: (error) => {
      // @ts-ignore
      if (error.response.status === 400) {
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

  const obsoleteFormMutation = useMutation({
    mutationKey: formsKeys.all,
    mutationFn: ({ electionRoundId, formId }: { electionRoundId: string; formId: string }) => {
      return authApi.post<void>(`/election-rounds/${electionRoundId}/forms/${formId}:obsolete`);
    },

    onSuccess: () => {
      toast({
        title: 'Success',
        description: 'Form obsoleted',
      });

      queryClient.invalidateQueries({ queryKey: formsKeys.all });
    },

    onError: () => {
      toast({
        title: 'Error obsoleting form',
        description: 'Please contact tech support',
        variant: 'destructive'
      });
    }
  });

  const duplicateFormMutation = useMutation({
    mutationKey: formsKeys.all,
    mutationFn: ({ electionRoundId, formId }: { electionRoundId: string; formId: string }) => {
      return authApi.post<void>(`/election-rounds/${electionRoundId}/forms/${formId}:duplicate`);
    },

    onSuccess: () => {
      toast({
        title: 'Success',
        description: 'Form duplicated',
      });

      queryClient.invalidateQueries({ queryKey: formsKeys.all });
    },

    onError: (error) => {
      toast({
        title: 'Error cloning form',
        description: 'Please contact tech support',
        variant: 'destructive'
      });
    }
  });

  const deleteFormMutation = useMutation({
    mutationKey: formsKeys.all,
    mutationFn: ({ electionRoundId, formId }: { electionRoundId: string; formId: string }) => {
      return authApi.delete<void>(
        `/election-rounds/${electionRoundId}/forms/${formId}`
      );
    },
    onSuccess: async () => {
      toast({
        title: 'Success',
        description: 'Form deleted',
      });
      await queryClient.invalidateQueries({ queryKey: formsKeys.all });
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
      <CardHeader className='flex gap-2 flex-column'>
        <CardTitle className='flex flex-row items-center justify-between px-6'>
          <div className='text-xl'>
            {i18n.t("electionEvent.observerForms.cardTitle")}
          </div>
          <div>
            <CreateDialog title={i18n.t('electionEvent.observerForms.createDialogTitle')}>
              <CreateForm />
            </CreateDialog>
          </div>
        </CardTitle>
        <Separator />
        <div className='flex flex-row justify-end gap-4 px-6 filters'>
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
          <div className='flex flex-row items-center gap-4 table-filters'>
            <Select value={formTypeFilter} onValueChange={handleFormTypeFilter}>
              <SelectTrigger className='w-[180px]'>
                <SelectValue placeholder='Form type' />
              </SelectTrigger>
              <SelectContent>
                <SelectGroup>
                  <SelectItem value={ZFormType.Values.Opening}>{mapFormType(ZFormType.Values.Opening)}</SelectItem>
                  <SelectItem value={ZFormType.Values.Voting}>{mapFormType(ZFormType.Values.Voting)}</SelectItem>
                  <SelectItem value={ZFormType.Values.ClosingAndCounting}>{mapFormType(ZFormType.Values.ClosingAndCounting)}</SelectItem>
                  {isMonitoringNgoForCitizenReporting && <SelectItem value={ZFormType.Values.CitizenReporting}>{mapFormType(ZFormType.Values.CitizenReporting)}</SelectItem>}
                  <SelectItem value={ZFormType.Values.IncidentReporting}>{mapFormType(ZFormType.Values.IncidentReporting)}</SelectItem>
                  <SelectItem value={ZFormType.Values.Other}>{mapFormType(ZFormType.Values.Other)}</SelectItem>
                </SelectGroup>
              </SelectContent>
            </Select>
            <Button variant='ghost-primary'>
              <span onClick={resetFilters} className='text-base text-purple-900'>
                {i18n.t('electionEvent.observerForms.resetFilters')}
              </span>
            </Button>
            <div className='flex flex-row flex-wrap gap-2'>
              {formTypeFilter && (
                <span
                  onClick={() => handleFormTypeFilter('')}
                  className='flex items-center gap-2 px-4 py-1 text-sm font-medium text-purple-900 bg-purple-100 rounded-full cursor-pointer'>
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
          useQuery={(params) => useForms(currentElectionRoundId, params)}
          getSubrows={getSubrows}
          getRowClassName={getRowClassName}
        />
        <AddTranslationsDialog />
      </CardContent>
    </Card>
  );
}
