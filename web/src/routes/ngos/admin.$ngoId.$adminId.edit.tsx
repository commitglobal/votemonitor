import { EditNgoAdmin } from '@/features/ngos/components/admins/EditNgoAdmin';
import { ngoAdminDetailsOptions, useNgoAdminDetails } from '@/features/ngos/hooks/ngo-admin-queries';
import { ngoDetailsOptions, useNGODetails } from '@/features/ngos/hooks/ngos-queries';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/ngos/admin/$ngoId/$adminId/edit')({
  beforeLoad: ({ params }) => {
    redirectIfNotAuth();
  },
  component: NgoAdminDetails,

  loader: async ({ context: { queryClient }, params: { ngoId, adminId } }) => {
    const ngoDataPromise = queryClient.ensureQueryData(ngoDetailsOptions(ngoId));
    const ngoAdminDataPromise = queryClient.ensureQueryData(ngoAdminDetailsOptions({ ngoId, adminId }));

    const [ngoData, ngoAdminData] = await Promise.all([ngoDataPromise, ngoAdminDataPromise]);

    return { ngoAdminData, ngoData };
  },
});

function NgoAdminDetails() {
  const { ngoId, adminId } = Route.useParams();
  const { data: ngoAdmin } = useNgoAdminDetails({ ngoId, adminId });
  const { data: ngo } = useNGODetails(ngoId);

  return (
    <div className='p-2'>
      <EditNgoAdmin existingData={ngoAdmin} id={ngoId} />
    </div>
  );
}
