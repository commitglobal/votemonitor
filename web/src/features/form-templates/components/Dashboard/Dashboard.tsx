import { authApi } from '@/common/auth-api';
import { DateTimeFormat } from '@/common/formats';
import { FormBase, FormStatus } from '@/common/types';
import AddFormTranslationsDialog, {
  useAddFormTranslationsDialog,
} from '@/components/AddFormTranslationsDialog/AddFormTranslationsDialog';
import FormStatusBadge from '@/components/FormStatusBadge/ElectionRoundStatusBadge';
import FormTranslationStatusBadge from '@/components/FormTranslationStatusBadge/FormTranslationStatusBadge';
import { DataTableColumnHeader } from '@/components/ui/DataTable/DataTableColumnHeader';
import { QueryParamsDataTable } from '@/components/ui/DataTable/QueryParamsDataTable';
import { useConfirm } from '@/components/ui/alert-dialog-provider';
import { Button, buttonVariants } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Input } from '@/components/ui/input';
import { LanguageBadge } from '@/components/ui/language-badge';
import { Separator } from '@/components/ui/separator';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';
import { toast } from '@/components/ui/use-toast';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { useLanguages } from '@/hooks/languages';
import i18n from '@/i18n';
import { cn, mapFormType } from '@/lib/utils';
import { queryClient } from '@/main';
import { FormTemplatesSearchParams, Route } from '@/routes/form-templates/index';
import {
  ChevronDownIcon,
  ChevronUpIcon,
  EllipsisVerticalIcon,
  FunnelIcon,
  PlusIcon,
} from '@heroicons/react/24/outline';
import { useMutation } from '@tanstack/react-query';
import { Link, useNavigate, useRouter } from '@tanstack/react-router';
import { ColumnDef, createColumnHelper, Row } from '@tanstack/react-table';
import { useDebounce } from '@uidotdev/usehooks';
import { format } from 'date-fns';
import { useMemo, useState, type ReactElement } from 'react';
import { formTemlatesKeys, useFormTemplates } from '../../queries';
import { FormTemplateFilters } from './FormTemplateFilters';

export default function FormTemplatesDashboard(): ReactElement {
  const navigate = useNavigate();
  const search = Route.useSearch();
  const debouncedSearch = useDebounce(search, 300);
  const [searchText, setSearchText] = useState('');
  const { filteringIsActive } = useFilteringContainer();
  const router = useRouter();

  const queryParams = useMemo(() => {
    const params: FormTemplatesSearchParams = {
      searchText: searchText,
      formTemplateType: debouncedSearch.formTemplateType,
      formTemplateStatus: debouncedSearch.formTemplateStatus,
    };

    return params;
  }, [searchText, debouncedSearch]);

  const addTranslationsDialog = useAddFormTranslationsDialog();

  const confirm = useConfirm();

  const { data: languages } = useLanguages();

  const columnHelper = createColumnHelper<FormBase>();

  const formColDefs: ColumnDef<FormBase>[] = useMemo(() => {
    const defaultColumns = [
      columnHelper.display({
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
      }),
      columnHelper.display({
        id: 'code',
        enableSorting: true,
        header: ({ column }) => (
          <DataTableColumnHeader title={i18n.t('electionEvent.formTemplates.headers.formCode')} column={column} />
        ),
        cell: ({ row }) => row.original.code,
      }),
      columnHelper.display({
        id: 'name',
        enableSorting: false,
        header: ({ column }) => (
          <DataTableColumnHeader title={i18n.t('electionEvent.formTemplates.headers.name')} column={column} />
        ),
        cell: ({ row }) => row.original.name[row.original.defaultLanguage],
      }),
      columnHelper.display({
        id: 'formType',
        enableSorting: true,
        enableResizing: false,
        header: ({ column }) => (
          <DataTableColumnHeader title={i18n.t('electionEvent.formTemplates.headers.formType')} column={column} />
        ),
        cell: ({ row }) => (row.depth === 0 ? mapFormType(row.original.formType) : ''),
      }),
      columnHelper.display({
        id: 'defaultLanguage',
        enableSorting: false,
        enableResizing: false,
        header: ({ column }) => (
          <DataTableColumnHeader title={i18n.t('electionEvent.formTemplates.headers.language')} column={column} />
        ),
        cell: ({ row }) => (
          <LanguageBadge languageCode={row.original.defaultLanguage} variant={'unstyled'} displayMode='native' />
        ),
      }),
      columnHelper.display({
        id: 'numberOfQuestions',
        enableSorting: true,
        enableResizing: false,
        header: ({ column }) => (
          <DataTableColumnHeader title={i18n.t('electionEvent.formTemplates.headers.questions')} column={column} />
        ),
        cell: ({ row }) => (row.depth === 0 ? row.original.numberOfQuestions : ''),
      }),
      columnHelper.display({
        id: 'status',
        enableSorting: true,
        enableResizing: false,
        header: ({ column }) => (
          <DataTableColumnHeader title={i18n.t('electionEvent.formTemplates.headers.status')} column={column} />
        ),
        cell: ({ row }) => {
          const form = row.original;

          return row.depth === 0 ? (
            <FormStatusBadge status={form.status} />
          ) : (
            <FormTranslationStatusBadge
              defaultLanguage={form.defaultLanguage}
              translationStatus={form.languagesTranslationStatus}
            />
          );
        },
      }),
      columnHelper.display({
        id: 'LastModifiedOn',
        enableSorting: true,
        enableResizing: false,
        header: ({ column }) => (
          <DataTableColumnHeader title={i18n.t('electionEvent.formTemplates.headers.updatedOn')} column={column} />
        ),
        cell: ({ row }) =>
          row.depth === 0 ? (
            <TooltipProvider delayDuration={100}>
              <Tooltip>
                <TooltipTrigger asChild>
                  <span className='underline cursor-pointer decoration-dashed hover:decoration-solid'>
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
      }),
    ];

    defaultColumns.push(
      columnHelper.display({
        header: '',
        id: 'action',
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
                  disabled={!row.original.isFormOwner || row.original.status !== FormStatus.Drafted}
                  onClick={() => navigateToEdit(row.original.id)}>
                  Edit
                </DropdownMenuItem>
              ) : (
                <DropdownMenuItem
                  disabled={!row.original.isFormOwner || row.original.status !== FormStatus.Drafted}
                  onClick={() => navigateToEditTranslation(row.original.id, row.original.defaultLanguage)}>
                  Edit
                </DropdownMenuItem>
              )}

              {row.depth === 0 ? (
                <DropdownMenuItem
                  disabled={!row.original.isFormOwner || row.original.status !== FormStatus.Drafted}
                  onClick={() => addTranslationsDialog.trigger(row.original.id, row.original.languages)}>
                  Add translations
                </DropdownMenuItem>
              ) : null}
              {row.depth === 0 && row.original.status === FormStatus.Published ? (
                <DropdownMenuItem disabled={!row.original.isFormOwner} onClick={() => handleObsoleteForm(row.original)}>
                  Obsolete
                </DropdownMenuItem>
              ) : null}
              {row.depth === 0 && row.original.status === FormStatus.Drafted ? (
                <DropdownMenuItem disabled={!row.original.isFormOwner} onClick={() => handlePublishForm(row.original)}>
                  Publish
                </DropdownMenuItem>
              ) : null}
              {row.depth === 0 ? (
                <DropdownMenuItem
                  disabled={!row.original.isFormOwner}
                  onClick={() => handleDuplicateForm(row.original)}>
                  Duplicate
                </DropdownMenuItem>
              ) : null}
              {row.depth === 0 ? (
                <DropdownMenuItem
                  className='text-red-600'
                  disabled={!row.original.isFormOwner}
                  onClick={async () => {
                    if (
                      await confirm({
                        title: `Delete form ${row.original.code}?`,
                        body:
                          row.original.status === FormStatus.Published ? (
                            <>
                              Please note that this form is published and may contain associated data. Deleting this
                              form could result in the loss of any submitted answers from your observers. Once deleted,{' '}
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
                      deleteFormMutation.mutate({
                        formTemplateId: row.original.id,
                      });
                    }
                  }}>
                  Delete form
                </DropdownMenuItem>
              ) : (
                <DropdownMenuItem
                  className='text-red-600'
                  disabled={!row.original.isFormOwner}
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
                        formTemplateId: row.original.id,
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
      })
    );

    return defaultColumns;
  }, []);

  const [isFiltering, setIsFiltering] = useState(filteringIsActive);

  const handleSearchInput = (ev: React.FormEvent<HTMLInputElement>) => {
    setSearchText(ev.currentTarget.value);
  };

  const handleObsoleteForm = (formTemplate: FormBase) => {
    obsoleteFormTemplateMutation.mutate({ formTemplateId: formTemplate.id });
  };

  const handlePublishForm = (formTemplate: FormBase) => {
    publishFormMutation.mutate({ formTemplateId: formTemplate.id });
  };

  const handleDuplicateForm = (formTemplate: FormBase) => {
    duplicateFormMutation.mutate({ formTemplateId: formTemplate.id });
  };

  const navigateToForm = (formTemplateId: string, languageCode: string) => {
    navigate({ to: '/form-templates/$formTemplateId/$languageCode', params: { formTemplateId, languageCode } });
  };

  const navigateToEdit = (formTemplateId: string) => {
    navigate({ to: '/form-templates/$formTemplateId/edit', params: { formTemplateId } });
  };

  const navigateToEditTranslation = (formTemplateId: string, languageCode: string) => {
    navigate({
      to: '/form-templates/$formTemplateId/edit-translation/$languageCode',
      params: { formTemplateId, languageCode },
    });
  };

  const deleteTranslationMutation = useMutation({
    mutationFn: ({ formTemplateId, languageCode }: { formTemplateId: string; languageCode: string }) => {
      return authApi.delete<void>(`/form-templates/${formTemplateId}/${languageCode}`);
    },

    onSuccess: (_data) => {
      toast({
        title: 'Success',
        description: 'Translation deleted',
      });

      queryClient.invalidateQueries({ queryKey: formTemlatesKeys.all() });
      router.invalidate();
    },
  });

  const publishFormMutation = useMutation({
    mutationFn: ({ formTemplateId }: { formTemplateId: string }) => {
      return authApi.post<void>(`/form-templates/${formTemplateId}:publish`);
    },

    onSuccess: () => {
      toast({
        title: 'Success',
        description: 'Form template published',
      });

      queryClient.invalidateQueries({ queryKey: formTemlatesKeys.all() });
      router.invalidate();
    },

    onError: (error) => {
      // @ts-ignore
      if (error.response.status === 400) {
        toast({
          title: 'Error publishing form template',
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

  const obsoleteFormTemplateMutation = useMutation({
    mutationFn: ({ formTemplateId }: { formTemplateId: string }) => {
      return authApi.post<void>(`/form-templates/${formTemplateId}:obsolete`);
    },

    onSuccess: () => {
      toast({
        title: 'Success',
        description: 'Form template obsoleted',
      });

      queryClient.invalidateQueries({ queryKey: formTemlatesKeys.all() });
      router.invalidate();
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
    mutationFn: ({ formTemplateId }: { formTemplateId: string }) => {
      return authApi.post<void>(`/form-templates/${formTemplateId}:duplicate`);
    },

    onSuccess: () => {
      toast({
        title: 'Success',
        description: 'Form template duplicated',
      });

      queryClient.invalidateQueries({ queryKey: formTemlatesKeys.all() });
      router.invalidate();
    },

    onError: () => {
      toast({
        title: 'Error cloning form template',
        description: 'Please contact tech support',
        variant: 'destructive',
      });
    },
  });

  const deleteFormMutation = useMutation({
    mutationFn: ({ formTemplateId }: { formTemplateId: string }) => {
      return authApi.delete<void>(`/form-templates/${formTemplateId}`);
    },
    onSuccess: async () => {
      toast({
        title: 'Success',
        description: 'Form template deleted',
      });
      await queryClient.invalidateQueries({ queryKey: formTemlatesKeys.all() });
      router.invalidate();
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
        <CardTitle className='flex flex-row items-center justify-between pr-6'>
          <div className='text-2xl font-semibold leading-none tracking-tight'>
            {i18n.t('electionEvent.formTemplates.cardTitle')}
          </div>
          <div>
            <Link to='/form-templates/new'>
              <Button title='Create form' variant='default'>
                <PlusIcon className='w-5 h-5 mr-2 -ml-1.5' />
                <span>Create form template</span>
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
        {isFiltering && <FormTemplateFilters />}
      </CardHeader>
      <CardContent>
        <QueryParamsDataTable
          columns={formColDefs}
          useQuery={useFormTemplates}
          getSubrows={getSubrows}
          queryParams={queryParams}
          getRowClassName={getRowClassName}
        />
        <AddFormTranslationsDialog />
      </CardContent>
    </Card>
  );
}
