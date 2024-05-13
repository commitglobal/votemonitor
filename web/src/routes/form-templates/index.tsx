import { SortOrder } from '@/common/types';
import FormTemplatesDashboard from '@/features/form-templates/components/Dashboard/Dashboard';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';
import { z } from 'zod';

const formTemplateRouteSearchSchema = z.object({
  nameFilter: z.string().catch(''),
  pageNumber: z.number().catch(1),
  pageSize: z.number().catch(10),
  sortColumnName: z.string().catch(''),
  sortOrder: z.enum([SortOrder.asc, SortOrder.desc]).catch(SortOrder.asc),
});

export const Route = createFileRoute('/form-templates/')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: FormTemplatesList,
  validateSearch: formTemplateRouteSearchSchema,
});

function FormTemplatesList() {
  return (
    <div className='p-2'>
      <FormTemplatesDashboard />
    </div>
  );
}
