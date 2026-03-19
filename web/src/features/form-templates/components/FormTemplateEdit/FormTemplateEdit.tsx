import { authApi } from '@/common/auth-api';
import { mapToQuestionRequest } from '@/common/form-requests';
import FormEditor, { EditFormType } from '@/components/FormEditor/FormEditor';
import { FormTemplateDetailsBreadcrumbs } from '@/components/FormTemplateDetailsBreadcrumbs/FormTemplateDetailsBreadcrumbs';
import Layout from '@/components/layout/Layout';
import { NavigateBack } from '@/components/NavigateBack/NavigateBack';
import { isNilOrWhitespace } from '@/lib/utils';
import { queryClient } from '@/main';
import { Route } from '@/routes/form-templates/$formTemplateId_.edit';
import { useMutation, useSuspenseQuery } from '@tanstack/react-query';
import { useNavigate, useRouter } from '@tanstack/react-router';
import { useCallback } from 'react';
import { toast } from 'sonner';
import { UpdateFormTemplateRequest } from '../../models';
import { formTemlatesKeys, formTemplateDetailsQueryOptions } from '../../queries';

function FormTemplateEdit() {
  const { formTemplateId } = Route.useParams();
  const { data: formTemplate } = useSuspenseQuery(formTemplateDetailsQueryOptions(formTemplateId));

  const navigate = useNavigate();
  const router = useRouter();

  const updateFormTemplateMutation = useMutation({
    mutationFn: (formTemplate: UpdateFormTemplateRequest) => {
      return authApi.put<void>(`/form-templates/${formTemplate.id}`, {
        ...formTemplate,
      });
    },

    onSuccess: async () => {
      toast('Form template updated successfully');
      await queryClient.invalidateQueries({ queryKey: formTemlatesKeys.all(), type: 'all' });
      router.invalidate();
    },

    onError: () => {
      toast.error('Error saving form template',{
        description: 'Please contact tech support',
      });
    },
  });

  const saveFormTemplate = useCallback(
    async (formData: EditFormType) => {
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

      await updateFormTemplateMutation.mutateAsync(updatedForm);
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
        onSaveForm={(formData: EditFormType) =>
          saveFormTemplate(formData)
        }
        onNavigateAway={() => {
          navigate({ to: '/form-templates' });
        }}
        hasCitizenReportingOption={true}
      />
    </Layout>
  );
}

export default FormTemplateEdit;
