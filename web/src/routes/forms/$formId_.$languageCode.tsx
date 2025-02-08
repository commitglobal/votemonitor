import PreviewForm from '@/components/PreviewFormPage/PreviewFormPage';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { FormDetailsBreadcrumbs } from '@/components/FormDetailsBreadcrumbs/FormDetailsBreadcrumbs';
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
    <div className='p-2'>
      <PreviewForm
        form={form}
        languageCode={form.defaultLanguage}
        breadcrumbs={<FormDetailsBreadcrumbs formCode={form.code} formName={form.name[languageCode] ?? ''} />}
        onNavigateToEdit={navigateToEdit}
      />
    </div>
  );
}
