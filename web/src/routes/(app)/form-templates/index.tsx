import { FormStatus, FormType } from '@/common/types';
import FormTemplatesDashboard from '@/features/form-templates/components/Dashboard/Dashboard';
import { redirectIfNotAuth, redirectIfNotPlatformAdmin } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';
import { z } from 'zod';

const formTemplatesDashboardRouteSearchSchema = z.object({
  searchText: z.string().catch(''),
  formTemplateStatus: z.nativeEnum(FormStatus).optional(),
  formTemplateType: z.nativeEnum(FormType).optional(),
});

export type FormTemplatesSearchParams = z.infer<typeof formTemplatesDashboardRouteSearchSchema>;


export const Route = createFileRoute('/(app)/form-templates/')({
  component: FormTemplatesDashboard,
  beforeLoad: () => {
    redirectIfNotAuth();
    redirectIfNotPlatformAdmin();
  },
  validateSearch: formTemplatesDashboardRouteSearchSchema,
});
