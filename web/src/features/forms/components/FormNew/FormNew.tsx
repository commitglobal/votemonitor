import { authApi } from '@/common/auth-api';
import { mapToQuestionRequest } from '@/common/form-requests';
import FormEditor, { EditFormType } from '@/components/FormEditor/FormEditor';
import Layout from '@/components/layout/Layout';
import { NavigateBack } from '@/components/NavigateBack/NavigateBack';
import { useToast } from '@/components/ui/use-toast';
import { isNilOrWhitespace } from '@/lib/utils';
import { queryClient } from '@/main';
import { useMutation } from '@tanstack/react-query';
import { useNavigate, useRouter } from '@tanstack/react-router';
import { useCallback } from 'react';
import { FormFull, NewFormRequest } from '../../models';
import { formsKeys } from '../../queries';
import { useElectionRoundDetails } from '@/features/election-event/hooks/election-event-hooks';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';

function FormNew() {
  const navigate = useNavigate();
  const router = useRouter();
  const { toast } = useToast();
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
      return authApi
        .post<FormFull>(`/election-rounds/${electionRoundId}/forms`, {
          ...form,
        })
        .then((response) => response.data);
    },

    onSuccess: ({ id }, { electionRoundId, shouldNavigateAwayAfterSubmit }) => {
      toast({
        title: 'Success',
        description: 'Form template created successfully',
      });

      queryClient.invalidateQueries({ queryKey: formsKeys.all(electionRoundId), type: 'all' });
      router.invalidate();

      if (shouldNavigateAwayAfterSubmit) {
        void navigate({
          to: '/election-rounds/$electionRoundId/$tab',
          params: { electionRoundId, tab: 'observer-forms' },
        });
      } else {
        void navigate({ to: '/forms/$formId/edit', params: { formId: id } });
      }
    },

    onError: () => {
      toast({
        title: 'Error creating form template',
        description: 'Please contact tech support',
        variant: 'destructive',
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
    <Layout backButton={<NavigateBack to='/election-rounds' />} title={`Create new form template`} breadcrumbs={<></>}>
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
