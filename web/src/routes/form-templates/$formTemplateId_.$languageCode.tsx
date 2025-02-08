import { NavigateBack } from '@/components/NavigateBack/NavigateBack';
import PreviewForm from '@/components/PreviewFormPage/PreviewFormPage';
import { FormTemplateDetailsBreadcrumbs } from '@/features/form-templates/components/FormTemplateDetailsBreadcrumbs/FormDetailsBreadcrumbs';
import { formTemplateDetailsQueryOptions } from '@/features/form-templates/queries';
import { redirectIfNotAuth, redirectIfNotPlatformAdmin } from '@/lib/utils';
import { createFileRoute, useNavigate } from '@tanstack/react-router';
import { useCallback } from 'react';

export const Route = createFileRoute('/form-templates/$formTemplateId_/$languageCode')({
  component: Details,
  loader: ({ context: { queryClient }, params: { formTemplateId } }) => {
    return queryClient.ensureQueryData(formTemplateDetailsQueryOptions(formTemplateId));
  },
  beforeLoad: () => {
    redirectIfNotAuth();
    redirectIfNotPlatformAdmin();
  },
});

function Details() {
  const { formTemplateId, languageCode } = Route.useParams();
  const form = Route.useLoaderData();
  const navigate = useNavigate();
  const navigateToEdit = useCallback(() => {
    void navigate({ to: '/form-templates/$formTemplateId/edit', params: { formTemplateId } });
  }, [navigate]);

  return (
    <div className='p-2'>
      <PreviewForm
        form={form}
        languageCode={form.defaultLanguage}
        breadcrumbs={<FormTemplateDetailsBreadcrumbs formCode={form.code} formName={form.name[languageCode] ?? ''} />}
        backButton={<NavigateBack to='/form-templates' />}
        onNavigateToEdit={navigateToEdit}
      />
    </div>
  );
}

