import { createFileRoute } from '@tanstack/react-router';
import { authApi } from '@/common/auth-api';
import { NGO } from '@/features/ngos/models/NGO';
import { queryOptions } from '@tanstack/react-query';

export const ngoQueryOptions = (ngoId: string) =>
  queryOptions({
    queryKey: ['ngos', { ngoId }],
    queryFn: async () => {
      const response = await authApi.get<NGO>(`/ngos/${ngoId}`);

      if (response.status !== 200) {
        throw new Error('Failed to fetch ngo');
      }

      return response.data;
    },
  });

export const Route = createFileRoute('/ngos/$ngoId')({
  component: NgoDetails,
  loader: ({ context: { queryClient }, params: { ngoId } }) => queryClient.ensureQueryData(ngoQueryOptions(ngoId)),
});

function NgoDetails() {
  const ngo = Route.useLoaderData();

  return <div className='p-2'>Hello from ngos! {JSON.stringify(ngo, null, 2)}</div>;
}
