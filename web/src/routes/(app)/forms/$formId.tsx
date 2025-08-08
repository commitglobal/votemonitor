import type { FunctionComponent } from '@/common/types';
import { formDetailsQueryOptions } from '@/features/forms/queries';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute, useLoaderData, useNavigate } from '@tanstack/react-router';

function Details(): FunctionComponent {
  const navigate = useNavigate({ from: '/forms/$formId' });
  const formData = useLoaderData({ from: '/(app)/forms/$formId' });

  if (formData.defaultLanguage && formData.id) {
    const formId = formData.id;
    const languageCode = formData.defaultLanguage;

    navigate({
      to: '/forms/$formId/$languageCode',
      params: { languageCode, formId },
      replace: true,
    });
  }

  return null;
}

export const Route = createFileRoute('/(app)/forms/$formId')({
  component: Details,
  loader: ({ context: { queryClient, currentElectionRoundContext }, params: { formId } }) => {
    const electionRoundId = currentElectionRoundContext.getState().currentElectionRoundId;

    return queryClient.ensureQueryData(formDetailsQueryOptions(electionRoundId, formId));
  },
  beforeLoad: () => {
    redirectIfNotAuth();
  },
});
