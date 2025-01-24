import { NgoAdminDetailsView } from '@/features/ngos/components/admins/NgoAdminDetailsView';
import { ngoDetailsOptions, useNgoAdminDetails } from '@/features/ngos/hooks/ngos-queriess';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/ngos/admin/$ngoId/$adminId/view')({
  beforeLoad: ({ params }) => {
    redirectIfNotAuth();
  },
  component: NgoAdminDetails,

  loader: ({ context: { queryClient }, params: { ngoId } }) => queryClient.ensureQueryData(ngoDetailsOptions(ngoId)),
});

function NgoAdminDetails() {
  const { ngoId, adminId } = Route.useParams();
  const { data: ngoAdmin } = useNgoAdminDetails({ ngoId, adminId });

  return (
    <div className='p-2'>
      <NgoAdminDetailsView ngoId={ngoId} ngoAdmin={ngoAdmin} />
    </div>
  );
}
