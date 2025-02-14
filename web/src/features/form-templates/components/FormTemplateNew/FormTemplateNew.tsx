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
import { FormTemplateFull, NewFormTemplateRequest } from '../../models';
import { formTemlatesKeys } from '../../queries';

function FormTemplateNew() {
  const navigate = useNavigate();
  const router = useRouter();
  const { toast } = useToast();

  const newFormTemplateMutation = useMutation({
    mutationFn: async ({
      formTemplate,
    }: {
      shouldNavigateAwayAfterSubmit: boolean;
      formTemplate: NewFormTemplateRequest;
    }) => {
      return await authApi
        .post<FormTemplateFull>(`/form-templates`, {
          ...formTemplate,
        })
        .then((response) => response.data);
    },

    onSuccess: ({ id }, { shouldNavigateAwayAfterSubmit }) => {
      toast({
        title: 'Success',
        description: 'Form template created successfully',
      });

      queryClient.invalidateQueries({ queryKey: formTemlatesKeys.all(), type: 'all' });
      router.invalidate();
      if (shouldNavigateAwayAfterSubmit) {
        navigate({ to: '/form-templates' });
      } else {
        navigate({ to: `/form-templates/$formTemplateId/edit`, params: { formTemplateId: id } });
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

  const saveFormTemplate = useCallback((formData: EditFormType, shouldNavigateAwayAfterSubmit: boolean) => {
    const newFormTemplate: NewFormTemplateRequest = {
      code: formData.code,
      name: formData.name,
      defaultLanguage: formData.languageCode,
      description: formData.description,
      formType: formData.formType,
      languages: formData.languages,
      icon: isNilOrWhitespace(formData.icon) ? undefined : formData.icon,
      questions: formData.questions.map(mapToQuestionRequest),
    };

    newFormTemplateMutation.mutate({ formTemplate: newFormTemplate, shouldNavigateAwayAfterSubmit });
  }, []);

  return (
    <Layout backButton={<NavigateBack to='/form-templates' />} title={`Create new form template`}>
      <FormEditor
        onSaveForm={(formData: EditFormType, shouldNavigateAwayAfterSubmit: boolean) =>
          saveFormTemplate(formData, shouldNavigateAwayAfterSubmit)
        }
        hasCitizenReportingOption={true}
      />
    </Layout>
  );
}

export default FormTemplateNew;
