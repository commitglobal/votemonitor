import { authApi } from '@/common/auth-api';
import { mapToQuestionRequest } from '@/common/form-requests';
import { FormDetailsBreadcrumbs } from '@/components/FormDetailsBreadcrumbs/FormDetailsBreadcrumbs';
import FormEditor, { EditFormType } from '@/components/FormEditor/FormEditor';
import Layout from '@/components/layout/Layout';
import { NavigateBack } from '@/components/NavigateBack/NavigateBack';
import { useConfirm } from '@/components/ui/alert-dialog-provider';
import { useToast } from '@/components/ui/use-toast';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useElectionRoundDetails } from '@/features/election-event/hooks/election-event-hooks';
import { isNilOrWhitespace } from '@/lib/utils';
import { queryClient } from '@/main';
import { Route } from '@/routes/forms/$formId_.edit';
import { useMutation, useSuspenseQuery } from '@tanstack/react-query';
import { useNavigate, useRouter } from '@tanstack/react-router';
import { useCallback } from 'react';
import { UpdateFormRequest } from '../../models';
import { formDetailsQueryOptions, formsKeys } from '../../queries';

function FormEdit() {
  const { formId } = Route.useParams();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: electionEvent } = useElectionRoundDetails(currentElectionRoundId);

  const { data: form } = useSuspenseQuery(formDetailsQueryOptions(currentElectionRoundId, formId));

  const navigate = useNavigate();
  const router = useRouter();
  const { toast } = useToast();
  const confirm = useConfirm();

  const updateFormMutation = useMutation({
    mutationFn: ({
      electionRoundId,
      form,
    }: {
      electionRoundId: string;
      form: UpdateFormRequest;
      shouldNavigateAwayAfterSubmit: boolean;
    }) => {
      return authApi.put<void>(`/election-rounds/${electionRoundId}/forms/${form.id}`, {
        ...form,
      });
    },

    onSuccess: async (_, { electionRoundId, shouldNavigateAwayAfterSubmit }) => {
      toast({
        title: 'Success',
        description: 'Form updated successfully',
      });

      await queryClient.invalidateQueries({ queryKey: formsKeys.all(electionRoundId), type: 'all' });
      router.invalidate();

      if (shouldNavigateAwayAfterSubmit) {
        if (
          await confirm({
            title: 'Changes made to form template in base language',
            body: 'Please note that changes have been made to the form in base language, which can impact the translation(s). All new questions or response options which you have added have been copied to translations but in the base language. Access each translation of the form and manually translate each of the changes.',
          })
        ) {
          await navigate({ to: '/election-event/$tab', params: { tab: 'observer-forms' } });
        }
      }
    },

    onError: () => {
      toast({
        title: 'Error saving form template',
        description: 'Please contact tech support',
        variant: 'destructive',
      });
    },
  });

  const saveForm = useCallback(
    (formData: EditFormType, shouldNavigateAwayAfterSubmit: boolean) => {
      const updatedForm: UpdateFormRequest = {
        id: formId,
        code: formData.code,
        name: formData.name,
        defaultLanguage: formData.languageCode,
        description: formData.description,
        formType: formData.formType,
        languages: formData.languages,
        icon: isNilOrWhitespace(formData.icon) ? undefined : formData.icon,
        questions: formData.questions.map(mapToQuestionRequest),
      };

      updateFormMutation.mutate({
        electionRoundId: currentElectionRoundId,
        form: updatedForm,
        shouldNavigateAwayAfterSubmit,
      });
    },
    [formId, currentElectionRoundId]
  );

  return (
    <Layout
      backButton={<NavigateBack to='/form-templates' />}
      breadcrumbs={<FormDetailsBreadcrumbs formCode={form.code} formName={form.name[form.defaultLanguage] ?? ''} />}
      title={`${form.code} - ${form.name[form.defaultLanguage] ?? ''}`}>
      <FormEditor
        formData={form}
        onSaveForm={(formData: EditFormType, shouldNavigateAwayAfterSubmit: boolean) =>
          saveForm(formData, shouldNavigateAwayAfterSubmit)
        }
        hasCitizenReportingOption={electionEvent?.isMonitoringNgoForCitizenReporting ?? false}
      />
    </Layout>
  );
}

export default FormEdit;
