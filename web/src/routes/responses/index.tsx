import ResponsesDashboard from '@/features/responses/components/Dashboard/Dashboard';
import { FormSubmissionsSearchParamsSchema } from '@/features/responses/models/search-params';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/responses/')({
  component: () => <ResponsesDashboard />,
  validateSearch: FormSubmissionsSearchParamsSchema,
});
