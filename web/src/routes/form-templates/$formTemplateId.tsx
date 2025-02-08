import type { FunctionComponent } from '@/common/types';
import { formTemplateDetailsQueryOptions } from '@/features/form-templates/queries';
import { formDetailsQueryOptions } from '@/features/forms/queries';
import { redirectIfNotAuth, redirectIfNotPlatformAdmin } from '@/lib/utils';
import { createFileRoute, useLoaderData, useNavigate } from '@tanstack/react-router';

function Details(): FunctionComponent {
  const navigate = useNavigate({ from: '/form-templates/$formTemplateId' });
  const formData = useLoaderData({ from: '/form-templates/$formTemplateId' });

  if (formData.defaultLanguage && formData.id) {
    const formTemplateId = formData.id;
    const languageCode = formData.defaultLanguage;

    void navigate({
      to: '/form-templates/$formTemplateId/$languageCode',
      params: { languageCode, formTemplateId },
      replace: true,
    });
  }

  return null;
}

export const Route = createFileRoute('/form-templates/$formTemplateId')({
  component: Details,
  loader: ({ context: { queryClient }, params: { formTemplateId } }) => {
    return queryClient.ensureQueryData(formTemplateDetailsQueryOptions(formTemplateId));
  },
  beforeLoad: () => {
    redirectIfNotAuth();
    redirectIfNotPlatformAdmin();
  },
});
