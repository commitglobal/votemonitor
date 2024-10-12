import { authApi } from '@/common/auth-api';
import { DateTimeFormat } from '@/common/formats';
import { ZTranslationStatus } from '@/common/types';
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
import { Separator } from '@/components/ui/separator';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';
import { toast } from '@/components/ui/use-toast';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { useLanguages } from '@/hooks/languages';
import i18n from '@/i18n';
import { cn, mapFormType } from '@/lib/utils';
import { queryClient } from '@/main';
import { FormsSearchParams, Route } from '@/routes/election-event/$tab';
import {
  ChevronDownIcon,
  ChevronUpIcon,
  EllipsisVerticalIcon,
  FunnelIcon,
  PlusIcon,
} from '@heroicons/react/24/outline';
import { useMutation } from '@tanstack/react-query';
import { Link, useNavigate } from '@tanstack/react-router';
import { ColumnDef, Row } from '@tanstack/react-table';
import { useDebounce } from '@uidotdev/usehooks';
import { format } from 'date-fns';
import { useMemo, useState, type ReactElement } from 'react';
import { FormBase, FormStatus } from '../../models/form';
import { formsKeys, useForms } from '../../queries';
import AddTranslationsDialog, { useAddTranslationsDialog } from './AddTranslationsDialog';
import { FormFilters } from './FormFilters/FormFilters';

export default function FormsDashboard(): ReactElement {
  const navigate = useNavigate();
  const search = Route.useSearch();
  const debouncedSearch = useDebounce(search, 300);
  const [searchText, setSearchText] = useState('');
  const { filteringIsActive } = useFilteringContainer();

  const queryParams = useMemo(() => {
    const params = [
      ['searchText', searchText],
      ['typeFilter', debouncedSearch.formTypeFilter],
      ['statusFilter', debouncedSearch.formStatusFilter],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params) as FormsSearchParams;
  }, [searchText, debouncedSearch]);

  const addTranslationsDialog = useAddTranslationsDialog();
  const confirm = useConfirm();

  const { data: languages } = useLanguages();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);

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
              }}>
              {row.getIsExpanded() ? (
                <ChevronUpIcon className='w-4 h-4 ml-auto opacity-50' />
              ) : (
                <ChevronDownIcon className='w-4 h-4 ml-auto opacity-50' />
              )}
            </button>
          ) : (
            ''
          )}
          {getValue<boolean>()}
        </div>
      ),
      enableResizing: false,
    },
    {
      accessorKey: 'code',
      enableSorting: true,
      header: ({ column }) => (
        <DataTableColumnHeader title={i18n.t('electionEvent.observerForms.headers.formCode')} column={column} />
      ),
    },
    {
      id: 'name',
      accessorFn: (row, _) => row.name[row.defaultLanguage],
      enableSorting: false,
      header: ({ column }) => (
        <DataTableColumnHeader title={i18n.t('electionEvent.observerForms.headers.name')} column={column} />
      ),
    },
    {
      accessorKey: 'formType',
      accessorFn: (row, _) => mapFormType(row.formType),
      enableSorting: true,
      enableResizing: false,
      header: ({ column }) => (
        <DataTableColumnHeader title={i18n.t('electionEvent.observerForms.headers.formType')} column={column} />
      ),
      cell: ({ row }) => (row.depth === 0 ? row.original.formType : ''),
    },
    {
      accessorKey: 'defaultLanguage',
      enableSorting: false,
      enableResizing: false,
      header: ({ column }) => (
        <DataTableColumnHeader title={i18n.t('electionEvent.observerForms.headers.language')} column={column} />
      ),
    },
    {
      accessorKey: 'numberOfQuestions',
      enableSorting: true,
      enableResizing: false,
      header: ({ column }) => (
        <DataTableColumnHeader title={i18n.t('electionEvent.observerForms.headers.questions')} column={column} />
      ),
      cell: ({ row }) => (row.depth === 0 ? row.original.numberOfQuestions : ''),
    },
    {
      accessorKey: 'status',
      enableSorting: true,
      enableResizing: false,
      header: ({ column }) => (
        <DataTableColumnHeader title={i18n.t('electionEvent.observerForms.headers.status')} column={column} />
      ),
      cell: ({ row }) => {
        const form = row.original;

        return row.depth === 0 ? (
          <Badge
            className={cn({
              'text-slate-700 bg-slate-200': form.status === FormStatus.Drafted,
              'text-green-600 bg-green-200': form.status === FormStatus.Published,
              'text-yellow-600 bg-yellow-200': form.status === FormStatus.Obsolete,
            })}>
            {form.status}
          </Badge>
        ) : (
          <Badge
            className={cn({
              'text-green-600 bg-green-200':
                form.languagesTranslationStatus[form.defaultLanguage] === ZTranslationStatus.enum.Translated,
              'text-yellow-600 bg-yellow-200':
                form.languagesTranslationStatus[form.defaultLanguage] === ZTranslationStatus.enum.MissingTranslations,
              'text-slate-700 bg-slate-200': form.languagesTranslationStatus[form.defaultLanguage] === undefined,
            })}>
            {form.languagesTranslationStatus[form.defaultLanguage] === ZTranslationStatus.enum.Translated
              ? 'Translated'
              : form.languagesTranslationStatus[form.defaultLanguage] === ZTranslationStatus.enum.MissingTranslations
              ? 'Missing translation'
              : 'Unknown'}
          </Badge>
        );
      },
    },
    {
      accessorKey: 'LastModifiedOn',
      enableSorting: true,
      enableResizing: false,
      header: ({ column }) => (
        <DataTableColumnHeader title={i18n.t('electionEvent.observerForms.headers.updatedOn')} column={column} />
      ),
      cell: ({ row }) =>
        row.depth === 0 ? (
          <TooltipProvider delayDuration={100}>
            <Tooltip>
              <TooltipTrigger asChild>
                <span className='underline cursor-pointer decoration-dashed hover:decoration-solid'>
                  {' '}
                  {format(row.original.lastModifiedOn, DateTimeFormat)}
                </span>
              </TooltipTrigger>
              <TooltipContent>
                <p>{row.original.lastModifiedBy}</p>
              </TooltipContent>
            </Tooltip>
          </TooltipProvider>
        ) : (
          <></>
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
            <DropdownMenuItem onClick={() => navigateToForm(row.original.id, row.original.defaultLanguage)}>
              View
            </DropdownMenuItem>

            {row.depth === 0 ? (
              <DropdownMenuItem
                disabled={row.original.status !== FormStatus.Drafted}
                onClick={() => navigateToEdit(row.original.id)}>
                Edit
              </DropdownMenuItem>
            ) : (
              <DropdownMenuItem
                disabled={row.original.status !== FormStatus.Drafted}
                onClick={() => navigateToEditTranslation(row.original.id, row.original.defaultLanguage)}>
                Edit
              </DropdownMenuItem>
            )}

            {row.depth === 0 ? (
              <DropdownMenuItem
                disabled={row.original.status !== FormStatus.Drafted}
                onClick={() => addTranslationsDialog.trigger(row.original.id, row.original.languages)}>
                Add translations
              </DropdownMenuItem>
            ) : null}
            {row.depth === 0 && row.original.status === FormStatus.Published ? (
              <DropdownMenuItem onClick={() => handleObsoleteForm(row.original)}>Obsolete</DropdownMenuItem>
            ) : null}
            {row.depth === 0 && row.original.status === FormStatus.Drafted ? (
              <DropdownMenuItem onClick={() => handlePublishForm(row.original)}>Publish</DropdownMenuItem>
            ) : null}
            {row.depth === 0 ? (
              <DropdownMenuItem onClick={() => handleDuplicateForm(row.original)}>Duplicate</DropdownMenuItem>
            ) : null}
            {row.depth === 0 ? (
              <DropdownMenuItem
                className='text-red-600'
                onClick={async () => {
                  if (
                    await confirm({
                      title: `Delete form ${row.original.code}?`,
                      body:
                        row.original.status === FormStatus.Published ? (
                          <>
                            Please note that this form is published and may contain associated data. Deleting this form
                            could result in the loss of any submitted answers from your observers. Once deleted,{' '}
                            <b>the associated data cannot be retrieved</b>
                          </>
                        ) : (
                          'This action is permanent and cannot be undone. Once deleted, this form cannot be retrieved.'
                        ),
                      actionButton: 'Delete',
                      actionButtonClass: buttonVariants({ variant: 'destructive' }),
                      cancelButton: 'Cancel',
                    })
                  ) {
                    deleteFormMutation.mutate({ electionRoundId: currentElectionRoundId, formId: row.original.id });
                  }
                }}>
                Delete form
              </DropdownMenuItem>
            ) : (
              <DropdownMenuItem
                className='text-red-600'
                onClick={async () => {
                  const languageCode = row.original.defaultLanguage;
                  const language = languages?.find((l) => languageCode === l.code);
                  const fullName = language ? `${language.name} / ${language.nativeName}` : '';

                  if (
                    await confirm({
                      title: `Delete translation ${fullName}?`,
                      body: 'This action is permanent and cannot be undone. Once deleted, this translation cannot be retrieved.',
                      actionButton: 'Delete',
                      actionButtonClass: buttonVariants({ variant: 'destructive' }),
                      cancelButton: 'Cancel',
                    })
                  ) {
                    deleteTranslationMutation.mutate({
                      electionRoundId: currentElectionRoundId,
                      formId: row.original.id,
                      languageCode,
                    });
                  }
                }}>
                Delete translation
              </DropdownMenuItem>
            )}
          </DropdownMenuContent>
        </DropdownMenu>
      ),
    },
  ];

  const [isFiltering, setIsFiltering] = useState(filteringIsActive);

  const handleSearchInput = (ev: React.FormEvent<HTMLInputElement>) => {
    setSearchText(ev.currentTarget.value);
  };

  const handleObsoleteForm = (form: FormBase) => {
    obsoleteFormMutation.mutate({ electionRoundId: currentElectionRoundId, formId: form.id });
  };

  const handlePublishForm = (form: FormBase) => {
    publishFormMutation.mutate({ electionRoundId: currentElectionRoundId, formId: form.id });
  };

  const handleDuplicateForm = (form: FormBase) => {
    duplicateFormMutation.mutate({ electionRoundId: currentElectionRoundId, formId: form.id });
  };

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
    mutationFn: ({
      electionRoundId,
      formId,
      languageCode,
    }: {
      electionRoundId: string;
      formId: string;
      languageCode: string;
    }) => {
      return authApi.delete<void>(`/election-rounds/${electionRoundId}/forms/${formId}/${languageCode}`);
    },

    onSuccess: (_data, { electionRoundId }) => {
      toast({
        title: 'Success',
        description: 'Translation deleted',
      });

      queryClient.invalidateQueries({ queryKey: formsKeys.all(electionRoundId) });
    },
  });

  const publishFormMutation = useMutation({
    mutationFn: ({ electionRoundId, formId }: { electionRoundId: string; formId: string }) => {
      return authApi.post<void>(`/election-rounds/${electionRoundId}/forms/${formId}:publish`);
    },

    onSuccess: (_data, { electionRoundId }) => {
      toast({
        title: 'Success',
        description: 'Form published',
      });

      queryClient.invalidateQueries({ queryKey: formsKeys.all(electionRoundId) });
    },

    onError: (error) => {
      // @ts-ignore
      if (error.response.status === 400) {
        toast({
          title: 'Error publishing form',
          description: 'You are missing translations. Please translate all fields and try again',
          variant: 'destructive',
        });

        return;
      }
      toast({
        title: 'Error publishing form',
        description: 'Please contact tech support',
        variant: 'destructive',
      });
    },
  });

  const obsoleteFormMutation = useMutation({
    mutationFn: ({ electionRoundId, formId }: { electionRoundId: string; formId: string }) => {
      return authApi.post<void>(`/election-rounds/${electionRoundId}/forms/${formId}:obsolete`);
    },

    onSuccess: (_data, { electionRoundId }) => {
      toast({
        title: 'Success',
        description: 'Form obsoleted',
      });

      queryClient.invalidateQueries({ queryKey: formsKeys.all(electionRoundId) });
    },

    onError: () => {
      toast({
        title: 'Error obsoleting form',
        description: 'Please contact tech support',
        variant: 'destructive',
      });
    },
  });

  const duplicateFormMutation = useMutation({
    mutationFn: ({ electionRoundId, formId }: { electionRoundId: string; formId: string }) => {
      return authApi.post<void>(`/election-rounds/${electionRoundId}/forms/${formId}:duplicate`);
    },

    onSuccess: (_data, { electionRoundId }) => {
      toast({
        title: 'Success',
        description: 'Form duplicated',
      });

      queryClient.invalidateQueries({ queryKey: formsKeys.all(electionRoundId) });
    },

    onError: (error) => {
      toast({
        title: 'Error cloning form',
        description: 'Please contact tech support',
        variant: 'destructive',
      });
    },
  });

  const deleteFormMutation = useMutation({
    mutationFn: ({ electionRoundId, formId }: { electionRoundId: string; formId: string }) => {
      return authApi.delete<void>(`/election-rounds/${electionRoundId}/forms/${formId}`);
    },
    onSuccess: async (_data, { electionRoundId }) => {
      toast({
        title: 'Success',
        description: 'Form deleted',
      });
      await queryClient.invalidateQueries({ queryKey: formsKeys.all(electionRoundId) });
    },
  });

  const getSubrows = (originalRow: FormBase, index: number): undefined | FormBase[] => {
    if (originalRow.languages.length === 0) return undefined;

    // we need to have subrows only for translations
    return originalRow.languages
      .filter((languageCode) => originalRow.defaultLanguage !== languageCode)
      .map((languageCode) => ({
        ...originalRow,
        languages: [],
        code: `${originalRow.code} - ${languageCode}`,
        defaultLanguage: languageCode,
      }));
  };

  const getRowClassName = (row: Row<FormBase>): string => cn({ 'bg-secondary-300 bg-opacity-[.15]': row.depth === 1 });

  return (
    <Card className='w-full pt-0'>
      <CardHeader className='flex gap-2 flex-column'>
        <CardTitle className='flex flex-row items-center justify-between px-6'>
          <div className='text-xl'>{i18n.t('electionEvent.observerForms.cardTitle')}</div>
          <div>
            <Link to='/forms/new'>
              <Button title='Create form' variant='default'>
                <PlusIcon className='w-5 h-5 mr-2 -ml-1.5' />
                <span>Create form</span>
              </Button>
            </Link>
          </div>
        </CardTitle>
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
        </div>

        <Separator />
        {isFiltering && <FormFilters />}
      </CardHeader>
      <CardContent>
        <QueryParamsDataTable
          columns={formColDefs}
          useQuery={(params) => useForms(currentElectionRoundId, params)}
          getSubrows={getSubrows}
          queryParams={queryParams}
          getRowClassName={getRowClassName}
        />
        <AddTranslationsDialog />
      </CardContent>
    </Card>
  );
}
