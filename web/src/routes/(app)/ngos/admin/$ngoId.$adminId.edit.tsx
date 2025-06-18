import { EditNgoAdmin } from '@/features/ngos/components/admins/EditNgoAdmin';
import { ngoAdminDetailsOptions, useNgoAdminDetails } from '@/features/ngos/hooks/ngo-admin-queries';
import { ngoDetailsOptions } from '@/features/ngos/hooks/ngos-queries';
import { redirectIfNotAuth, redirectIfNotPlatformAdmin } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/(app)/ngos/admin/$ngoId/$adminId/edit')({
  beforeLoad: ({ params }) => {
    redirectIfNotAuth();
    redirectIfNotPlatformAdmin();
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

  return <EditNgoAdmin ngoAdmin={ngoAdmin} />;
}
