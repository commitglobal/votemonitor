import { mapToQuestionRequest } from '@/common/form-requests';
import FormEditor, { EditFormType } from '@/components/FormEditor/FormEditor';
import Layout from '@/components/layout/Layout';
import { NavigateBack } from '@/components/NavigateBack/NavigateBack';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useElectionRoundDetails } from '@/features/election-event/hooks/election-event-hooks';
import { isNilOrWhitespace } from '@/lib/utils';
import { queryClient } from '@/main';
import API from '@/services/api';
import { useMutation } from '@tanstack/react-query';
import { useNavigate, useRouter } from '@tanstack/react-router';
import { useCallback } from 'react';
import { toast } from 'sonner';
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
      shouldNavigateAwayAfterSubmit: boolean;
    }) => {
      return API.post<FormFull>(`/election-rounds/${electionRoundId}/forms`, {
        ...form,
      }).then((response) => response.data);
    },

    onSuccess: ({ id }, { electionRoundId, shouldNavigateAwayAfterSubmit }) => {
      toast.success('Form created successfully');

      queryClient.invalidateQueries({ queryKey: formsKeys.all(electionRoundId), type: 'all' });
      router.invalidate();

      if (shouldNavigateAwayAfterSubmit) {
        navigate({
          to: '/election-event/$tab',
          params: { tab: 'observer-forms' },
        });
      } else {
        navigate({ to: '/forms/$formId/edit', params: { formId: id } });
      }
    },

    onError: () => {
      toast('Error creating form', {
        description: 'Please contact tech support',
      });
    },
  });

  const saveForm = useCallback(
    (electionRoundId: string, formData: EditFormType, shouldNavigateAwayAfterSubmit: boolean) => {
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

      newFormMutation.mutate({ electionRoundId, form: newForm, shouldNavigateAwayAfterSubmit });
    },
    []
  );

  return (
    <Layout
      backButton={<NavigateBack to='/election-event/$tab' params={{ tab: 'observer-forms' }} />}
      title={`Create new form template`}
      breadcrumbs={<></>}>
      <FormEditor
        onSaveForm={(formData: EditFormType, shouldNavigateAwayAfterSubmit: boolean) =>
          saveForm(currentElectionRoundId, formData, shouldNavigateAwayAfterSubmit)
        }
        hasCitizenReportingOption={electionEvent?.isMonitoringNgoForCitizenReporting ?? false}
      />
    </Layout>
  );
}

export default FormNew;
