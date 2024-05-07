import { SortOrder } from '@/common/types';
import NGOsDashboard from '@/features/ngos/components/Dashboard/Dashboard';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';
import { z } from 'zod';

const ngoRouteSearchSchema = z.object({
  nameFilter: z.string().catch(''),
  pageNumber: z.number().catch(1),
  pageSize: z.number().catch(10),
  sortColumnName: z.string().catch(''),
  sortOrder: z.enum([SortOrder.asc, SortOrder.desc]).catch(SortOrder.asc),
});

export const Route = createFileRoute('/ngos/')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: Ngos,
  validateSearch: ngoRouteSearchSchema,
});

function Ngos() {
  return <NGOsDashboard />;
}
