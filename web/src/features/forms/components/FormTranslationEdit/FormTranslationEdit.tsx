import { authApi } from '@/common/auth-api';
import { mapToQuestionRequest } from '@/common/form-requests';
import { FormDetailsBreadcrumbs } from '@/components/FormDetailsBreadcrumbs/FormDetailsBreadcrumbs';
import { EditFormType } from '@/components/FormEditor/FormEditor';
import FormTranslationEditor from '@/components/FormTranslationEditor/FormTranslationEditor';
import Layout from '@/components/layout/Layout';
import { NavigateBack } from '@/components/NavigateBack/NavigateBack';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useElectionRoundDetails } from '@/features/election-event/hooks/election-event-hooks';
import { isNilOrWhitespace } from '@/lib/utils';
import { queryClient } from '@/main';
import { Route } from '@/routes/forms/$formId_.edit-translation.$languageCode';
import { useMutation, useSuspenseQuery } from '@tanstack/react-query';
import { useNavigate, useRouter } from '@tanstack/react-router';
import { useCallback } from 'react';
import { toast } from 'sonner';
import { UpdateFormRequest } from '../../models';
import { formDetailsQueryOptions, formsKeys } from '../../queries';

function FormTranslationEdit() {
  const { formId, languageCode } = Route.useParams();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: electionEvent } = useElectionRoundDetails(currentElectionRoundId);

  const { data: form } = useSuspenseQuery(formDetailsQueryOptions(currentElectionRoundId, formId));

  const navigate = useNavigate();
  const router = useRouter();

  const updateFormMutation = useMutation({
    mutationFn: ({
      electionRoundId,
      form,
    }: {
      electionRoundId: string;
      form: UpdateFormRequest;
    }) => {
      return authApi.put<void>(`/election-rounds/${electionRoundId}/forms/${form.id}`, {
        ...form,
      });
    },

    onSuccess: async (_, { electionRoundId }) => {
      toast('Form updated successfully');
      await queryClient.invalidateQueries({ queryKey: formsKeys.all(electionRoundId), type: 'all' });
      router.invalidate();
    },

    onError: () => {
      toast.error('Error saving form ',{
        description: 'Please contact tech support',
      });
    },
  });

  const saveForm = useCallback(
    async (formData: EditFormType, shouldNavigateAwayAfterSubmit: boolean) => {
      const updatedForm: UpdateFormRequest = {
        id: formId,
        code: formData.code,
        name: formData.name,
        defaultLanguage: form.defaultLanguage,
        description: formData.description,
        formType: formData.formType,
        languages: formData.languages,
        icon: isNilOrWhitespace(formData.icon) ? undefined : formData.icon,
        questions: formData.questions.map(mapToQuestionRequest),
      };

     await updateFormMutation.mutateAsync({
        electionRoundId: currentElectionRoundId,
        form: updatedForm,
      });
      if (shouldNavigateAwayAfterSubmit) {
        await navigate({ to: '/election-event/$tab', params: { tab: 'observer-forms' } });
      }
    },
    [updateFormMutation, formId, currentElectionRoundId]
  );

  return (
    <Layout
      backButton={<NavigateBack to='/election-event/$tab' params={{ tab: 'observer-forms' }} />}
      breadcrumbs={<FormDetailsBreadcrumbs formCode={form.code} formName={form.name[form.defaultLanguage] ?? ''} />}
      title={`${form.code} - ${form.name[form.defaultLanguage] ?? ''}`}>
      <FormTranslationEditor
        formData={form}
        onSaveForm={(formData: EditFormType, shouldNavigateAwayAfterSubmit: boolean) =>
          saveForm(formData, shouldNavigateAwayAfterSubmit)
        }
        hasCitizenReportingOption={electionEvent?.isMonitoringNgoForCitizenReporting ?? false}
        languageCode={languageCode}
      />
    </Layout>
  );
}

export default FormTranslationEdit;
