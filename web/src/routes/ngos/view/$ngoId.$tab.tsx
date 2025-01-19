import { authApi } from '@/common/auth-api';
import { NGODetails } from '@/features/ngos/components/NGODetails';
import { NGO } from '@/features/ngos/models/NGO';
import { redirectIfNotAuth } from '@/lib/utils';
import { queryOptions, useSuspenseQuery } from '@tanstack/react-query';
import { createFileRoute, redirect } from '@tanstack/react-router';
import { z } from 'zod';
import { ngoRouteSearchSchema } from '..';

export const ngoQueryOptions = (ngoId: string) =>
  queryOptions({
    queryKey: ['ngos', { ngoId }],
    queryFn: async () => {
      const response = await authApi.get<NGO>(`/ngos/${ngoId}`);

      if (response.status !== 200) {
        throw new Error('Failed to fetch ngo details');
      }

      return response.data;
    },
  });

export const NgoAdminsSearchParamsSchema = ngoRouteSearchSchema.partial();
export type NgoAdminsSearchParams = z.infer<typeof NgoAdminsSearchParamsSchema>;

export const NgosDetailsdPageSearchParamsSchema = NgoAdminsSearchParamsSchema.merge(
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

  loader: ({ context: { queryClient }, params: { ngoId } }) => queryClient.ensureQueryData(ngoQueryOptions(ngoId)),
});

const coerceTabSlug = (slug: string) => {
  if (slug?.toLowerCase()?.trim() === 'details') return 'details';
  if (slug?.toLowerCase()?.trim() === 'admins') return 'admins';

  return 'details';
};

function NgoDetails() {
  const { ngoId } = Route.useParams();
  const { data: ngo } = useSuspenseQuery(ngoQueryOptions(ngoId));

  return (
    <div className='p-2'>
      <NGODetails data={ngo} />
    </div>
  );
}
