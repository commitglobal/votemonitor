import { authApi } from '@/common/auth-api';
import { mapToQuestionRequest } from '@/common/form-requests';
import FormEditor, { EditFormType } from '@/components/FormEditor/FormEditor';
import Layout from '@/components/layout/Layout';
import { NavigateBack } from '@/components/NavigateBack/NavigateBack';
import { useConfirm } from '@/components/ui/alert-dialog-provider';
import { useToast } from '@/components/ui/use-toast';
import { isNilOrWhitespace } from '@/lib/utils';
import { queryClient } from '@/main';
import { Route } from '@/routes/(app)/form-templates/$formTemplateId_.edit';
import { useMutation, useSuspenseQuery } from '@tanstack/react-query';
import { useNavigate, useRouter } from '@tanstack/react-router';
import { useCallback } from 'react';
import { UpdateFormTemplateRequest } from '../../models';
import { formTemlatesKeys, formTemplateDetailsQueryOptions } from '../../queries';
import { FormTemplateDetailsBreadcrumbs } from '@/components/FormTemplateDetailsBreadcrumbs/FormTemplateDetailsBreadcrumbs';

function FormTemplateEdit() {
  const { formTemplateId } = Route.useParams();
  const { data: formTemplate } = useSuspenseQuery(formTemplateDetailsQueryOptions(formTemplateId));

  const navigate = useNavigate();
  const router = useRouter();
  const { toast } = useToast();
  const confirm = useConfirm();

  const updateFormTemplateMutation = useMutation({
    mutationFn: ({
      formTemplate,
    }: {
      formTemplate: UpdateFormTemplateRequest;
      shouldNavigateAwayAfterSubmit: boolean;
    }) => {
      return authApi.put<void>(`/form-templates/${formTemplate.id}`, {
        ...formTemplate,
      });
    },

    onSuccess: async (_, { shouldNavigateAwayAfterSubmit }) => {
      toast({
        title: 'Success',
        description: 'Form template updated successfully',
      });

      await queryClient.invalidateQueries({ queryKey: formTemlatesKeys.all(), type: 'all' });
      router.invalidate();

      if (shouldNavigateAwayAfterSubmit) {
        if (
          await confirm({
            title: 'Changes made to form template in base language',
            body: 'Please note that changes have been made to the form in base language, which can impact the translation(s). All new questions or response options which you have added have been copied to translations but in the base language. Access each translation of the form and manually translate each of the changes.',
          })
        ) {
          await navigate({ to: '/form-templates' });
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

  const saveFormTemplate = useCallback(
    (formData: EditFormType, shouldNavigateAwayAfterSubmit: boolean) => {
      const updatedForm: UpdateFormTemplateRequest = {
        id: formTemplateId,
        code: formData.code,
        name: formData.name,
        defaultLanguage: formData.languageCode,
        description: formData.description,
        formType: formData.formType,
        languages: formData.languages,
        icon: isNilOrWhitespace(formData.icon) ? undefined : formData.icon,
        questions: formData.questions.map(mapToQuestionRequest),
      };

      updateFormTemplateMutation.mutate({ formTemplate: updatedForm, shouldNavigateAwayAfterSubmit });
    },
    [formTemplateId]
  );

  return (
    <Layout
      backButton={<NavigateBack to='/form-templates' />}
      breadcrumbs={
        <FormTemplateDetailsBreadcrumbs
          formCode={formTemplate.code}
          formName={formTemplate.name[formTemplate.defaultLanguage] ?? ''}
        />
      }
      title={`${formTemplate.code} - ${formTemplate.name[formTemplate.defaultLanguage] ?? ''}`}>
      <FormEditor
        formData={formTemplate}
        onSaveForm={(formData: EditFormType, shouldNavigateAwayAfterSubmit: boolean) =>
          saveFormTemplate(formData, shouldNavigateAwayAfterSubmit)
        }
        hasCitizenReportingOption={true}
      />
    </Layout>
  );
}

export default FormTemplateEdit;
