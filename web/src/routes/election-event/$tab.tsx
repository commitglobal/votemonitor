import { ZFormType } from '@/common/types';
import ElectionEventDashboard from '@/features/election-event/components/Dashboard/Dashboard';
import { FormStatus } from '@/features/forms/models/form';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute, redirect } from '@tanstack/react-router';
import { z } from 'zod';

const coerceTabSlug = (slug: string) => {
  if (slug?.toLowerCase()?.trim() === 'event-details') return 'event-details';
  if (slug?.toLowerCase()?.trim() === 'polling-stations') return 'polling-stations';
  if (slug?.toLowerCase()?.trim() === 'observer-guides') return 'observer-guides';
  if (slug?.toLowerCase()?.trim() === 'observer-forms') return 'observer-forms';
  if (slug?.toLowerCase()?.trim() === 'locations') return 'locations';
  if (slug?.toLowerCase()?.trim() === 'citizen-guides') return 'citizen-guides';

  return 'event-details';
};

export const FormsSearchParamsSchema = z.object({
  formTypeFilter: ZFormType.optional().catch('Opening'),
  formStatusFilter: z.nativeEnum(FormStatus).optional().catch(FormStatus.Published),
});
export type FormsSearchParams = z.infer<typeof FormsSearchParamsSchema>;

export const ElectionEventPageSearchParamsSchema = FormsSearchParamsSchema.merge(
  z.object({
    tab: z
      .enum(['event-details', 'polling-stations', 'observer-guides', 'observer-forms', 'locations', 'citizen-guides'])
      .catch('event-details')
      .optional(),
  })
);

export const Route = createFileRoute('/election-event/$tab')({
  component: ElectionEventDashboard,
  validateSearch: ElectionEventPageSearchParamsSchema,
  beforeLoad: ({ params: { tab } }) => {
    redirectIfNotAuth();

    const coercedTab = coerceTabSlug(tab);
    if (tab !== coercedTab) {
      throw redirect({ to: `/election-event/$tab`, params: { tab: coercedTab } });
    }
  },
});
