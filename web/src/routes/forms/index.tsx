import { SortOrder } from '@/common/types';
import FormsDashboard from '@/features/forms/components/Dashboard/Dashboard';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';
import { z } from 'zod';

const formsRouteSearchSchema = z.object({
  nameFilter: z.string().catch(''),
  pageNumber: z.number().catch(1),
  pageSize: z.number().catch(10),
  sortColumnName: z.string().catch(''),
  sortOrder: z.enum([SortOrder.asc, SortOrder.desc]).catch(SortOrder.asc),
});

export const Route = createFileRoute('/forms/')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: FormsList,
  validateSearch: formsRouteSearchSchema,
});

function FormsList() {
  return (
    <div className='p-2'>
      <FormsDashboard />
    </div>
  );
}
