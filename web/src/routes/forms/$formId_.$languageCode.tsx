import { FormDetailsBreadcrumbs } from '@/components/FormDetailsBreadcrumbs/FormDetailsBreadcrumbs';
import Layout from '@/components/layout/Layout';
import { NavigateBack } from '@/components/NavigateBack/NavigateBack';
import PreviewForm from '@/components/PreviewFormPage/PreviewFormPage';
import { formDetailsQueryOptions } from '@/features/forms/queries';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute, useNavigate } from '@tanstack/react-router';
import { useCallback } from 'react';

export const Route = createFileRoute('/forms/$formId_/$languageCode')({
  component: Details,
  loader: ({ context: { queryClient, currentElectionRoundContext }, params: { formId } }) => {
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    return queryClient.ensureQueryData(formDetailsQueryOptions(electionRoundId, formId));
  },
  beforeLoad: () => {
    redirectIfNotAuth();
  },
});

function Details() {
  const { formId, languageCode } = Route.useParams();
  const form = Route.useLoaderData();
  const navigate = useNavigate();
  const navigateToEdit = useCallback(() => {
    void navigate({ to: '/forms/$formId/edit', params: { formId } });
  }, [navigate]);

  return (
    <Layout
      backButton={<NavigateBack to='/election-event/$tab' params={{ tab: 'observer-forms' }} />}
      breadcrumbs={<FormDetailsBreadcrumbs formCode={form.code} formName={form.name[languageCode] ?? ''} />}
      title={`${form.code} - ${form.name[languageCode]}`}>
      <PreviewForm form={form} languageCode={form.defaultLanguage} onNavigateToEdit={navigateToEdit} />
    </Layout>
  );
}
