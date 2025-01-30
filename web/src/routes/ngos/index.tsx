import { DefaultSearchParamsSchema } from '@/common/zod-schemas';
import NGOsDashboard from '@/features/ngos/components/Dashboard/Dashboard';
import { NGOStatus } from '@/features/ngos/models/NGO';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute, SearchSchemaInput } from '@tanstack/react-router';
import { z } from 'zod';

const NgosAdditionalSearchParams = z.object({
  status: z.nativeEnum(NGOStatus).optional(),
});

export const ngoRouteSearchSchema = NgosAdditionalSearchParams.merge(DefaultSearchParamsSchema);

export const Route = createFileRoute('/ngos/')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: Ngos,
  validateSearch: (search: unknown & SearchSchemaInput) => ngoRouteSearchSchema.parse(search),
});

function Ngos() {
  return <NGOsDashboard />;
}
