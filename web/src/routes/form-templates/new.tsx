import FormTemplateNew from '@/features/form-templates/components/FormTemplateNew/FormTemplateNew';
import { redirectIfNotAuth, redirectIfNotPlatformAdmin } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/form-templates/new')({
  beforeLoad: () => {
    redirectIfNotAuth();
    redirectIfNotPlatformAdmin();
  },
  component: CreateNewFormFromScratch,
});

function CreateNewFormFromScratch() {
  return <FormTemplateNew />;
}
