import { NgoAdminDetailsView } from '@/features/ngos/components/admins/NgoAdminDetailsView';
import { ngoAdminDetailsOptions, useNgoAdminDetails } from '@/features/ngos/hooks/ngo-admin-queries';
import { ngoDetailsOptions, useNGODetails } from '@/features/ngos/hooks/ngos-queries';
import { redirectIfNotPlatformAdmin } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/ngos/admin/$ngoId/$adminId/view')({
  beforeLoad: ({ params }) => {
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
  const { data: ngo } = useNGODetails(ngoId);

  return <NgoAdminDetailsView ngoId={ngoId} ngoName={ngo.name} ngoAdmin={ngoAdmin} />;
}
