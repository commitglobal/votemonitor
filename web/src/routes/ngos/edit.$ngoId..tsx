import { EditNgo } from '@/features/ngos/components/EditNgo';
import { ngoDetailsOptions, useNGODetails } from '@/features/ngos/hooks/ngos-queries';
import { redirectIfNotAuth, redirectIfNotPlatformAdmin } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/ngos/edit/$ngoId/')({
  beforeLoad: ({ params }) => {
    redirectIfNotAuth();
    redirectIfNotPlatformAdmin();
  },
  component: EditNgoPage,

  loader: ({ context: { queryClient }, params: { ngoId } }) => queryClient.ensureQueryData(ngoDetailsOptions(ngoId)),
});

function EditNgoPage() {
  const { ngoId } = Route.useParams();
  const { data: ngo } = useNGODetails(ngoId);

  return <EditNgo existingData={ngo} ngoId={ngoId} />;
}
