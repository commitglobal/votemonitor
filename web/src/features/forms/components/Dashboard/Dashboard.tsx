import { authApi } from '@/common/auth-api';
import { DateTimeFormat } from '@/common/formats';
import { ElectionRoundStatus, FormStatus, FormType, ReportedError } from '@/common/types';
import AddFormTranslationsDialog, {
  useAddFormTranslationsDialog,
} from '@/components/AddFormTranslationsDialog/AddFormTranslationsDialog';
import FormStatusBadge from '@/components/FormStatusBadge/FormStatusBadge';
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
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useElectionRoundDetails } from '@/features/election-event/hooks/election-event-hooks';
import { useFilteringContainer } from '@/features/filtering/hooks/useFilteringContainer';
import { useLanguages } from '@/hooks/languages';
import i18n from '@/i18n';
import { sendErrorToSentry } from '@/lib/sentry';
import { cn, isNotNilOrWhitespace, mapFormType } from '@/lib/utils';
import { queryClient } from '@/main';
import { FormsSearchParams, Route } from '@/routes/election-event/$tab';
import {
  ChevronDownIcon,
  ChevronUpIcon,
  EllipsisVerticalIcon,
  FunnelIcon,
  PhotoIcon,
  PlusIcon,
} from '@heroicons/react/24/outline';
import { useMutation } from '@tanstack/react-query';
import { Link, useNavigate, useRouter } from '@tanstack/react-router';
import { ColumnDef, createColumnHelper, Row } from '@tanstack/react-table';
import { useDebounce } from '@uidotdev/usehooks';
import { format } from 'date-fns';
import { difference } from 'lodash';
import { useMemo, useState, type ReactElement } from 'react';
import { NgoFormBase } from '../../models';
import { formsKeys, useForms } from '../../queries';
import EditFormAccessDialog, { useEditFormAccessDialog } from './EditFormAccessDialog';
import { FormFilters } from './FormFilters/FormFilters';

export default function FormsDashboard(): ReactElement {
  const navigate = useNavigate();
  const search = Route.useSearch();
  const debouncedSearch = useDebounce(search, 300);
  const [searchText, setSearchText] = useState('');
  const { filteringIsActive } = useFilteringContainer();
  const router = useRouter();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: electionRound } = useElectionRoundDetails(currentElectionRoundId);
  const { data: appLanguages } = useLanguages();

  const queryParams = useMemo(() => {
    const params = [
      ['searchText', searchText],
      ['typeFilter', debouncedSearch.formTypeFilter],
      ['statusFilter', debouncedSearch.formStatusFilter],
    ].filter(([_, value]) => value);

    return Object.fromEntries(params) as FormsSearchParams;
  }, [searchText, debouncedSearch]);

  const addTranslationsDialog = useAddFormTranslationsDialog();
  const editFormAccessDialog = useEditFormAccessDialog();

  const confirm = useConfirm();

  const columnHelper = createColumnHelper<NgoFormBase>();

  const formColDefs: ColumnDef<NgoFormBase>[] = useMemo(() => {
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
          <DataTableColumnHeader title={i18n.t('electionEvent.observerForms.headers.formCode')} column={column} />
        ),
        cell: ({ row }) => row.original.code,
      }),
      columnHelper.display({
        id: 'name',
        enableSorting: false,
        header: ({ column }) => (
          <DataTableColumnHeader title={i18n.t('electionEvent.observerForms.headers.name')} column={column} />
        ),
        cell: ({ row }) => row.original.name[row.original.defaultLanguage],
      }),
      columnHelper.display({
        id: 'formType',
        enableSorting: true,
        enableResizing: false,
        header: ({ column }) => (
          <DataTableColumnHeader title={i18n.t('electionEvent.observerForms.headers.formType')} column={column} />
        ),
        cell: ({ row }) => (row.depth === 0 ? mapFormType(row.original.formType) : ''),
      }),
      columnHelper.display({
        id: 'defaultLanguage',
        enableSorting: false,
        enableResizing: false,
        header: ({ column }) => (
          <DataTableColumnHeader title={i18n.t('electionEvent.observerForms.headers.language')} column={column} />
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
          <DataTableColumnHeader title={i18n.t('electionEvent.observerForms.headers.questions')} column={column} />
        ),
        cell: ({ row }) => (row.depth === 0 ? row.original.numberOfQuestions : ''),
      }),
      columnHelper.display({
        id: 'status',
        enableSorting: true,
        enableResizing: false,
        header: ({ column }) => (
          <DataTableColumnHeader title={i18n.t('electionEvent.observerForms.headers.status')} column={column} />
        ),
        cell: ({ row }) => {
          const form = row.original;

          return row.depth === 0 ? (
            <FormStatusBadge status={form.status} />
          ) : (
            <FormTranslationStatusBadge
              translationStatus={form.languagesTranslationStatus}
              defaultLanguage={form.defaultLanguage}
            />
          );
        },
      }),
      columnHelper.display({
        id: 'LastModifiedOn',
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

    if (electionRound?.isMonitoringNgoForCitizenReporting) {
      defaultColumns.splice(
        1,
        0,
        columnHelper.display({
          id: 'icon',
          enableSorting: true,
          header: ({ column }) => (
            <DataTableColumnHeader title={i18n.t('electionEvent.observerForms.headers.icon')} column={column} />
          ),
          cell: ({ row }) =>
            row.depth === 0 ? (
              isNotNilOrWhitespace(row.original.icon) ? (
                <TooltipProvider delayDuration={100}>
                  <Tooltip>
                    <TooltipTrigger asChild>
                      <span className='underline cursor-pointer decoration-dashed hover:decoration-solid'>
                        <PhotoIcon className='w-6 h-6' />
                      </span>
                    </TooltipTrigger>
                    <TooltipContent>
                      <div dangerouslySetInnerHTML={{ __html: row.original.icon ?? '' }}></div>
                    </TooltipContent>
                  </Tooltip>
                </TooltipProvider>
              ) : (
                <></>
              )
            ) : (
              ''
            ),
        })
      );
    }

    if (electionRound?.isCoalitionLeader) {
      defaultColumns.push(
        columnHelper.display({
          id: 'sharedWith',
          enableSorting: false,
          header: ({ column }) => (
            <DataTableColumnHeader title={i18n.t('electionEvent.observerForms.headers.sharedWith')} column={column} />
          ),
          cell: ({ row }) =>
            row.depth === 0 ? (
              row.original.formAccess.length ? (
                <TooltipProvider delayDuration={100}>
                  <Tooltip>
                    <TooltipTrigger asChild>
                      <span className='underline cursor-pointer decoration-dashed hover:decoration-solid'>
                        {row.original.formAccess.length}
                      </span>
                    </TooltipTrigger>
                    <TooltipContent>
                      {row.original.formAccess.map((fa) => (
                        <div key={fa.ngoId}>{fa.name}</div>
                      ))}
                    </TooltipContent>
                  </Tooltip>
                </TooltipProvider>
              ) : (
                <>{row.original.formType === FormType.CitizenReporting ? 'Citizens' : 'None'}</>
              )
            ) : null,
        })
      );
      defaultColumns.push({
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
              {row.depth === 0 && row.original.status === FormStatus.Published ? (
                <DropdownMenuItem
                  disabled={electionRound?.status === ElectionRoundStatus.Archived}
                  onClick={() => editFormAccessDialog.trigger(row.original.id)}>
                  Form access
                </DropdownMenuItem>
              ) : null}
              {row.depth === 0 ? (
                <DropdownMenuItem
                  disabled={
                    row.original.status !== FormStatus.Drafted || electionRound?.status === ElectionRoundStatus.Archived
                  }
                  onClick={() => navigateToEdit(row.original.id)}>
                  Edit
                </DropdownMenuItem>
              ) : (
                <DropdownMenuItem
                  disabled={
                    row.original.status !== FormStatus.Drafted || electionRound?.status === ElectionRoundStatus.Archived
                  }
                  onClick={() => navigateToEditTranslation(row.original.id, row.original.defaultLanguage)}>
                  Edit
                </DropdownMenuItem>
              )}

              {row.depth === 0 ? (
                <DropdownMenuItem
                  disabled={
                    row.original.status !== FormStatus.Drafted || electionRound?.status === ElectionRoundStatus.Archived
                  }
                  onClick={() =>
                    addTranslationsDialog.trigger(row.original.id, row.original.languages, (formId, newLanguages) =>
                      addTranslationsMutation.mutate({
                        electionRoundId: currentElectionRoundId,
                        formId,
                        newLanguages,
                        originalLanguages: row.original.languages,
                      })
                    )
                  }>
                  Add translations
                </DropdownMenuItem>
              ) : null}

              {row.depth === 0 && row.original.status === FormStatus.Published ? (
                <DropdownMenuItem
                  disabled={electionRound?.status === ElectionRoundStatus.Archived}
                  onClick={() => handleObsoleteForm(row.original)}>
                  Obsolete
                </DropdownMenuItem>
              ) : null}
              {row.depth === 0 && row.original.status === FormStatus.Drafted ? (
                <DropdownMenuItem
                  disabled={electionRound?.status === ElectionRoundStatus.Archived}
                  onClick={() => handlePublishForm(row.original)}>
                  Publish
                </DropdownMenuItem>
              ) : null}
              {row.depth === 0 ? (
                <DropdownMenuItem
                  disabled={electionRound?.status === ElectionRoundStatus.Archived}
                  onClick={() => handleDuplicateForm(row.original)}>
                  Duplicate
                </DropdownMenuItem>
              ) : null}
              {row.depth === 0 ? (
                <DropdownMenuItem
                  className='text-red-600'
                  disabled={electionRound?.status === ElectionRoundStatus.Archived}
                  onClick={async () => {
                    if (
                      await confirm({
                        title: `Delete form ${row.original.code}?`,
                        body:
                          row.original.status === FormStatus.Published ? (
                            <>
                              Please note that this form is published and may contain associated data. Deleting this
                              form could result in the loss of any submitted answers from your observers. Once deleted,
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
                        electionRoundId: currentElectionRoundId,
                        formId: row.original.id,
                      });
                    }
                  }}>
                  Delete form
                </DropdownMenuItem>
              ) : (
                <DropdownMenuItem
                  className='text-red-600'
                  disabled={electionRound?.status === ElectionRoundStatus.Archived}
                  onClick={async () => {
                    const languageCode = row.original.defaultLanguage;
                    const language = appLanguages?.find((l) => languageCode === l.code);
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
      });
    } else {
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
                    disabled={
                      !row.original.isFormOwner ||
                      electionRound?.status === ElectionRoundStatus.Archived ||
                      row.original.status !== FormStatus.Drafted
                    }
                    onClick={() => navigateToEdit(row.original.id)}>
                    Edit
                  </DropdownMenuItem>
                ) : (
                  <DropdownMenuItem
                    disabled={
                      !row.original.isFormOwner ||
                      electionRound?.status === ElectionRoundStatus.Archived ||
                      row.original.status !== FormStatus.Drafted
                    }
                    onClick={() => navigateToEditTranslation(row.original.id, row.original.defaultLanguage)}>
                    Edit
                  </DropdownMenuItem>
                )}

                {row.depth === 0 ? (
                  <DropdownMenuItem
                    disabled={
                      !row.original.isFormOwner ||
                      electionRound?.status === ElectionRoundStatus.Archived ||
                      row.original.status !== FormStatus.Drafted
                    }
                    onClick={() =>
                      addTranslationsDialog.trigger(row.original.id, row.original.languages, (formId, newLanguages) =>
                        addTranslationsMutation.mutate({
                          electionRoundId: currentElectionRoundId,
                          formId,
                          newLanguages,
                          originalLanguages: row.original.languages,
                        })
                      )
                    }>
                    Add translations
                  </DropdownMenuItem>
                ) : null}
                {row.depth === 0 && row.original.status === FormStatus.Published ? (
                  <DropdownMenuItem
                    disabled={!row.original.isFormOwner || electionRound?.status === ElectionRoundStatus.Archived}
                    onClick={() => handleObsoleteForm(row.original)}>
                    Obsolete
                  </DropdownMenuItem>
                ) : null}
                {row.depth === 0 && row.original.status === FormStatus.Drafted ? (
                  <DropdownMenuItem
                    disabled={!row.original.isFormOwner || electionRound?.status === ElectionRoundStatus.Archived}
                    onClick={() => handlePublishForm(row.original)}>
                    Publish
                  </DropdownMenuItem>
                ) : null}
                {row.depth === 0 ? (
                  <DropdownMenuItem
                    disabled={!row.original.isFormOwner || electionRound?.status === ElectionRoundStatus.Archived}
                    onClick={() => handleDuplicateForm(row.original)}>
                    Duplicate
                  </DropdownMenuItem>
                ) : null}
                {row.depth === 0 ? (
                  <DropdownMenuItem
                    className='text-red-600'
                    disabled={!row.original.isFormOwner || electionRound?.status === ElectionRoundStatus.Archived}
                    onClick={async () => {
                      if (
                        await confirm({
                          title: `Delete form ${row.original.code}?`,
                          body:
                            row.original.status === FormStatus.Published ? (
                              <>
                                Please note that this form is published and may contain associated data. Deleting this
                                form could result in the loss of any submitted answers from your observers. Once
                                deleted, <b>the associated data cannot be retrieved</b>
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
                          electionRoundId: currentElectionRoundId,
                          formId: row.original.id,
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
                      const language = appLanguages?.find((l) => languageCode === l.code);
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
        })
      );
    }

    return defaultColumns;
  }, [currentElectionRoundId, electionRound?.isMonitoringNgoForCitizenReporting, electionRound?.isCoalitionLeader]);

  const [isFiltering, setIsFiltering] = useState(filteringIsActive);

  const handleSearchInput = (ev: React.FormEvent<HTMLInputElement>) => {
    setSearchText(ev.currentTarget.value);
  };

  const handleObsoleteForm = (form: NgoFormBase) => {
    obsoleteFormMutation.mutate({ electionRoundId: currentElectionRoundId, formId: form.id });
  };

  const handlePublishForm = (form: NgoFormBase) => {
    publishFormMutation.mutate({ electionRoundId: currentElectionRoundId, formId: form.id });
  };

  const handleDuplicateForm = (form: NgoFormBase) => {
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
      router.invalidate();
    },
  });

  function getTitle(newLanguages: string[]): string {
    const languagesLabels =
      appLanguages?.filter((l) => newLanguages.includes(l.code)).map((l) => `${l.name} / ${l.nativeName}`) ?? [];

    if (languagesLabels.length === 0) return '';
    if (languagesLabels.length === 1) return languagesLabels[0] + ' added';

    const lastLanguage = languagesLabels.pop(); // Remove the last language from the array
    return languagesLabels.join(', ') + ' and ' + lastLanguage + ' added';
  }

  const addTranslationsMutation = useMutation({
    mutationFn: ({
      electionRoundId,
      formId,
      newLanguages,
      originalLanguages,
    }: {
      electionRoundId: string;
      formId: string;
      newLanguages: string[];
      originalLanguages: string[];
    }) => {
      return authApi.put<void>(`/election-rounds/${electionRoundId}/forms/${formId}:addTranslations`, {
        languageCodes: newLanguages,
      });
    },

    onSuccess: async (_, { electionRoundId, newLanguages, originalLanguages }) => {
      toast({
        title: 'Success',
        description: 'Translations added',
      });

      addTranslationsDialog.dismiss();

      const addedLanguages = difference(newLanguages, originalLanguages);

      await queryClient.invalidateQueries({ queryKey: formsKeys.all(electionRoundId) });
      await router.invalidate();

      confirm({
        title: getTitle(addedLanguages),
        body: (
          <div>
            {addedLanguages.length} translations were created to be translated into the selected languages{' '}
            <b>({addedLanguages.join(', ')})</b>. Please note that this is not an automatic translation as you need to
            manually translate each form in selected languages.
            <br />
            <b>You cannot add or delete questions on the translated forms. </b>Any changes you want to make to the
            questions (deletion or addition of new questions) will be made to the form in the <b>base language</b>, and
            they will be copied to the translated forms.
          </div>
        ),
        actionButton: 'Ok',
      });
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
      router.invalidate();
    },

    onError: (error: ReportedError) => {
      const title = 'Error publishing form';
      // @ts-ignore
      if (error.response.status === 400) {
        toast({
          title,
          description: 'You are missing translations. Please translate all fields and try again',
          variant: 'destructive',
        });

        return;
      }
      toast({
        title,
        description: 'Please contact tech support',
        variant: 'destructive',
      });
      sendErrorToSentry({ error, title });
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
      router.invalidate();
    },

    onError: (error: ReportedError) => {
      const title = 'Error obsoleting form';
      sendErrorToSentry({ error, title });
      toast({
        title,
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
      router.invalidate();
    },

    onError: (error: ReportedError) => {
      const title = 'Error cloning form';
      sendErrorToSentry({ error, title });
      toast({
        title,
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
      router.invalidate();
    },
  });

  const getSubrows = (originalRow: NgoFormBase, index: number): undefined | NgoFormBase[] => {
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

  const getRowClassName = (row: Row<NgoFormBase>): string =>
    cn({ 'bg-secondary-300 bg-opacity-[.15]': row.depth === 1 });

  return (
    <Card className='w-full pt-0'>
      <CardHeader className='flex gap-2 flex-column'>
        <CardTitle className='flex flex-row items-center justify-between pr-6'>
          <div className='text-2xl font-semibold leading-none tracking-tight'>
            {i18n.t('electionEvent.observerForms.cardTitle')}
          </div>
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
        <AddFormTranslationsDialog />
        <EditFormAccessDialog />
      </CardContent>
    </Card>
  );
}
