import PreviewForm from '@/components/PreviewFormPage/PreviewFormPage';
import { formTemplateDetailsQueryOptions } from '@/features/form-templates/queries';
import { redirectIfNotAuth, redirectIfNotPlatformAdmin } from '@/lib/utils';
import { createFileRoute, useNavigate } from '@tanstack/react-router';
import { useCallback } from 'react';

export const Route = createFileRoute('/(app)/form-templates/$formTemplateId_/$languageCode')({
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
    navigate({ to: '/form-templates/$formTemplateId/edit', params: { formTemplateId } });
  }, [navigate]);

  return <PreviewForm form={form} languageCode={form.defaultLanguage} onNavigateToEdit={navigateToEdit} />;
}
