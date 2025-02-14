import { FormTemplateDetailsBreadcrumbs } from '@/components/FormTemplateDetailsBreadcrumbs/FormTemplateDetailsBreadcrumbs';
import Layout from '@/components/layout/Layout';
import { NavigateBack } from '@/components/NavigateBack/NavigateBack';
import PreviewForm from '@/components/PreviewFormPage/PreviewFormPage';
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
    <Layout
      backButton={<NavigateBack to='/form-templates' />}
      breadcrumbs={<FormTemplateDetailsBreadcrumbs formCode={form.code} formName={form.name[languageCode] ?? ''} />}
      title={`${form.code} - ${form.name[languageCode]}`}>
      <PreviewForm form={form} languageCode={form.defaultLanguage} onNavigateToEdit={navigateToEdit} />
    </Layout>
  );
}
