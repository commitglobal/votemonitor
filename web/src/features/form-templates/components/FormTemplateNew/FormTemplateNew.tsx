import { authApi } from '@/common/auth-api';
import { mapToQuestionRequest } from '@/common/form-requests';
import FormEditor, { EditFormType } from '@/components/FormEditor/FormEditor';
import Layout from '@/components/layout/Layout';
import { NavigateBack } from '@/components/NavigateBack/NavigateBack';
import { useConfirm } from '@/components/ui/alert-dialog-provider';
import { useToast } from '@/components/ui/use-toast';
import { isNilOrWhitespace } from '@/lib/utils';
import { queryClient } from '@/main';
import { Route } from '@/routes/form-templates/$formTemplateId_.edit';
import { useMutation, useSuspenseQuery } from '@tanstack/react-query';
import { useNavigate, useRouter } from '@tanstack/react-router';
import { useCallback } from 'react';
import { FormTemplateFull, NewFormTemplateRequest, UpdateFormTemplateRequest } from '../../models';
import { formTemlatesKeys, formTemplateDetailsQueryOptions } from '../../queries';
import { FormTemplateDetailsBreadcrumbs } from '../FormTemplateDetailsBreadcrumbs/FormDetailsBreadcrumbs';

function FormTemplateNew() {
  const navigate = useNavigate();
  const router = useRouter();
  const { toast } = useToast();
  const confirm = useConfirm();

  const newFormTemplateMutation = useMutation({
    mutationFn: ({ formTemplate }: { formTemplate: NewFormTemplateRequest }) => {
      return authApi.post<FormTemplateFull>(`/form-templates`, {
        ...formTemplate,
      });
    },

    onSuccess: () => {
      toast({
        title: 'Success',
        description: 'Form template created successfully',
      });
      
      // TODO: set in cache
      queryClient.invalidateQueries({ queryKey: formTemlatesKeys.all(), type: 'all' });
      router.invalidate();

      void navigate({ to: '/form-templates' });
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

    newFormTemplateMutation.mutate({ formTemplate: newFormTemplate });
  }, []);

  return (
    <Layout backButton={<NavigateBack to='/form-templates' />} title={`Create new form template`}>
      <FormEditor
        onSaveForm={(formData: EditFormType, shouldNavigateAwayAfterSubmit: boolean) =>
          saveFormTemplate(formData, shouldNavigateAwayAfterSubmit)
        }
        hasCitizenReportingOption={true}
        formEditingMode={'ExistingForm'}
      />
    </Layout>
  );
}

export default FormTemplateNew;
