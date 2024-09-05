import { createFileRoute } from '@tanstack/react-router';
import { authApi } from '@/common/auth-api';
import { NGO } from '@/features/ngos/models/NGO';
import { queryOptions, useSuspenseQuery } from '@tanstack/react-query';
import { redirectIfNotAuth } from '@/lib/utils';

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

export const Route = createFileRoute('/ngos/$ngoId')({
  beforeLoad: () => {
    redirectIfNotAuth();
  },
  component: NgoDetails,
  loader: ({ context: { queryClient }, params: { ngoId } }) => queryClient.ensureQueryData(ngoQueryOptions(ngoId)),
});

function NgoDetails() {
  const { ngoId } = Route.useParams();
  const { data: ngo } = useSuspenseQuery(ngoQueryOptions(ngoId));

  return <div className='p-2'>Hello from ngos! {JSON.stringify(ngo, null, 2)}</div>;
}
