import ResponsesDashboard from '@/features/responses/components/Dashboard/Dashboard';
import { FormSubmissionsSearchParamsSchema } from '@/features/responses/models/search-params';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/responses/')({
  component: () => <ResponsesDashboard />,
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  validateSearch: FormSubmissionsSearchParamsSchema,
});
