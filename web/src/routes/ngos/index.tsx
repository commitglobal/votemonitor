import { SortOrder } from '@/common/types';
import NGOsDashboard from '@/features/ngos/components/Dashboard/Dashboard';
import { NGOStatus } from '@/features/ngos/models/NGO';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';
import { z } from 'zod';

export const ngoRouteSearchSchema = z.object({
  searchText: z.coerce.string().optional(),
  status: z.nativeEnum(NGOStatus).optional(),
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
