import { FormTemplateType } from '@/common/types';
import FormTemplatesDashboard from '@/features/form-templates/components/Dashboard/Dashboard';
import { FormTemplateStatus } from '@/features/form-templates/models';
import { redirectIfNotAuth, redirectIfNotPlatformAdmin } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';
import { z } from 'zod';

const formTemplatesDashboardRouteSearchSchema = z.object({
  searchText: z.string().catch(''),
  formTemplateStatus: z.nativeEnum(FormTemplateStatus).optional(),
  formTemplateType: z.nativeEnum(FormTemplateType).optional(),
});

export type FormTemplatesSearchParams = z.infer<typeof formTemplatesDashboardRouteSearchSchema>;


export const Route = createFileRoute('/form-templates/')({
  component: FormTemplatesDashboard,
  beforeLoad: () => {
    redirectIfNotAuth();
    redirectIfNotPlatformAdmin();
  },
  validateSearch: formTemplatesDashboardRouteSearchSchema,
});
