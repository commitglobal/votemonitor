import { NGODetails } from '@/features/ngos/components/NGODetails';
import { ngoDetailsOptions, useNGODetails } from '@/features/ngos/hooks/ngos-queries';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute, redirect } from '@tanstack/react-router';
import { z } from 'zod';
import { ngoRouteSearchSchema } from '.';

export const ngoAdminsSearchParamsSchema = ngoRouteSearchSchema.partial();
export type NgoAdminsSearchParams = z.infer<typeof ngoAdminsSearchParamsSchema>;

export const NgosDetailsdPageSearchParamsSchema = ngoAdminsSearchParamsSchema.merge(
  z.object({
    tab: z.enum(['details', 'admins']).catch('details').optional(),
  })
);

export const Route = createFileRoute('/ngos/view/$ngoId/$tab')({
  beforeLoad: ({ params }) => {
    redirectIfNotAuth();

    const coercedTab = coerceTabSlug(params.tab);
    if (params.tab !== coercedTab) {
      throw redirect({
        to: `/ngos/view/$ngoId/$tab`,
        params: { tab: coercedTab, ngoId: params.ngoId },
      });
    }
  },
  component: NgoDetails,
  validateSearch: NgosDetailsdPageSearchParamsSchema,

  loader: ({ context: { queryClient }, params: { ngoId } }) => queryClient.ensureQueryData(ngoDetailsOptions(ngoId)),
});

const coerceTabSlug = (slug: string) => {
  if (slug?.toLowerCase()?.trim() === 'details') return 'details';
  if (slug?.toLowerCase()?.trim() === 'admins') return 'admins';

  return 'details';
};

function NgoDetails() {
  const { ngoId } = Route.useParams();
  const { data: ngo } = useNGODetails(ngoId);

  return (
    <div className='p-2'>
      <NGODetails data={ngo} />
    </div>
  );
}
