import { DefaultSearchParamsSchema } from '@/common/zod-schemas';
import ObserversDashboard from '@/features/observers/components/Dashboard/Dashboard';
import { ObserverStatus } from '@/features/observers/models/observer';
import { redirectIfNotAuth, redirectIfNotPlatformAdmin } from '@/lib/utils';
import { createFileRoute, SearchSchemaInput } from '@tanstack/react-router';
import { z } from 'zod';

const ObserversAdditionalSearchParams = z.object({
  observerStatus: z.nativeEnum(ObserverStatus).optional(),
});

export const observersRouteSearchSchema = ObserversAdditionalSearchParams.merge(DefaultSearchParamsSchema);

export const Route = createFileRoute('/(app)/observers/')({
  beforeLoad: () => {
    redirectIfNotAuth();
    redirectIfNotPlatformAdmin();
  },
  component: Observers,
  validateSearch: (search: unknown & SearchSchemaInput) => observersRouteSearchSchema.parse(search),
});

function Observers() {
  return <ObserversDashboard />;
}
