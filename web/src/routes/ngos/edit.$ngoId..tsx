import { EditNgo } from '@/features/ngos/components/EditNgo';
import { ngoDetailsOptions, useNGODetails } from '@/features/ngos/hooks/ngos-queriess';
import { redirectIfNotAuth } from '@/lib/utils';
import { createFileRoute } from '@tanstack/react-router';

export const Route = createFileRoute('/ngos/edit/$ngoId/')({
  beforeLoad: ({ params }) => {
    redirectIfNotAuth();
  },
  component: EditNgoPage,

  loader: ({ context: { queryClient }, params: { ngoId } }) => queryClient.ensureQueryData(ngoDetailsOptions(ngoId)),
});

function EditNgoPage() {
  const { ngoId } = Route.useParams();
  const { data: ngo } = useNGODetails(ngoId);

  return (
    <div className='p-2'>
      <EditNgo existingData={ngo} ngoId={ngoId} />
    </div>
  );
}
