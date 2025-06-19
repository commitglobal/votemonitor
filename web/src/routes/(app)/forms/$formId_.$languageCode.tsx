import PreviewForm from '@/components/PreviewFormPage/PreviewFormPage';
import { formDetailsQueryOptions } from '@/features/forms/queries';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute, useNavigate } from '@tanstack/react-router';
import { useCallback } from 'react';

export const Route = createFileRoute('/(app)/forms/$formId_/$languageCode')({
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
    navigate({ to: '/forms/$formId/edit', params: { formId } });
  }, [navigate]);

  return <PreviewForm form={form} languageCode={form.defaultLanguage} onNavigateToEdit={navigateToEdit} />;
}
