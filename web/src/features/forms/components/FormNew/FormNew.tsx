import { authApi } from '@/common/auth-api';
import { mapToQuestionRequest } from '@/common/form-requests';
import FormEditor, { EditFormType } from '@/components/FormEditor/FormEditor';
import Layout from '@/components/layout/Layout';
import { NavigateBack } from '@/components/NavigateBack/NavigateBack';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useElectionRoundDetails } from '@/features/election-event/hooks/election-event-hooks';
import { isNilOrWhitespace } from '@/lib/utils';
import { queryClient } from '@/main';
import { useMutation } from '@tanstack/react-query';
import { useNavigate, useRouter } from '@tanstack/react-router';
import { useCallback } from 'react';
import { toast } from "sonner";
import { FormFull, NewFormRequest } from '../../models';
import { formsKeys } from '../../queries';

function FormNew() {
  const navigate = useNavigate();
  const router = useRouter();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const { data: electionEvent } = useElectionRoundDetails(currentElectionRoundId);

  const newFormMutation = useMutation({
    mutationFn: ({
      electionRoundId,
      form,
    }: {
      electionRoundId: string;
      form: NewFormRequest;
    }) => {
      return authApi
        .post<FormFull>(`/election-rounds/${electionRoundId}/forms`, {
          ...form,
        })
        .then((response) => response.data);
    },

    onSuccess: ({ id }, { electionRoundId }) => {
      toast('Form created successfully');

      queryClient.invalidateQueries({ queryKey: formsKeys.all(electionRoundId), type: 'all' });
      router.invalidate();

      navigate({ to: `/election-rounds/${electionRoundId}/forms/$formId/edit`, params: { formId: id } });
    },

    onError: () => {
      toast.error('Error creating form',{
        description: 'Please contact tech support',
      });
    },
  });

  const saveForm = useCallback(
    async (formData: EditFormType) => {
      const newForm: NewFormRequest = {
        code: formData.code,
        name: formData.name,
        defaultLanguage: formData.languageCode,
        description: formData.description,
        formType: formData.formType,
        languages: formData.languages,
        icon: isNilOrWhitespace(formData.icon) ? undefined : formData.icon,
        questions: formData.questions.map(mapToQuestionRequest),
      };

      await newFormMutation.mutateAsync({ electionRoundId: currentElectionRoundId, form: newForm });
    },
    [currentElectionRoundId, newFormMutation]
  );

  return (
    <Layout
      backButton={<NavigateBack to='/election-event/$tab' params={{ tab: 'observer-forms' }} />}
      title={`Create new form template`}
      breadcrumbs={<></>}>
      <FormEditor
        onSaveForm={saveForm}
        onNavigateAway={() => {
          void navigate({ to: '/election-event/$tab', params: { tab: 'observer-forms' } });
        }}
        hasCitizenReportingOption={electionEvent?.isMonitoringNgoForCitizenReporting ?? false}
      />
    </Layout>
  );
}

export default FormNew;
