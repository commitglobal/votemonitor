import { createFileRoute } from '@tanstack/react-router';
import type { FunctionComponent } from '@/common/types';
import ResponsesDashboard from '@/features/responses/components/Dashboard/Dashboard';
import { FormSubmissionsSearchParamsSchema } from '@/features/responses/models/search-params';

function Responses(): FunctionComponent {
  return <ResponsesDashboard />;
}

export const Route = createFileRoute('/responses/')({
  component: Responses,
  validateSearch: FormSubmissionsSearchParamsSchema,
});
