import { SortOrder } from '@/common/types';
import { createFileRoute } from '@tanstack/react-router';
import { z } from 'zod';
import { redirectIfNotAuth } from '@/lib/utils';
import FormTemplatesDashboard from '@/features/form-templates/components/Dashboard/Dashboard';

const formTemplateRouteSearchSchema = z.object({
  nameFilter: z.string().catch(''),
  pageNumber: z.number().catch(1),
  pageSize: z.number().catch(10),
  sortColumnName: z.string().catch(''),
  sortOrder: z.enum([SortOrder.asc, SortOrder.desc]).catch(SortOrder.asc),
});

export const Route = createFileRoute('/form-templates/')({
  beforeLoad: ({ context }) => {
    redirectIfNotAuth(context.authContext.isAuthenticated);
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
