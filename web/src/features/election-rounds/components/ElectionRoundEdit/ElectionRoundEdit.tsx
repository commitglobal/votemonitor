import { updateElectionRound } from '@/api/election-rounds/update-election-round';
import { ElectionRoundStatus } from '@/common/types';
import Layout from '@/components/layout/Layout';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { queryClient } from '@/main';
import { Route } from '@/routes/election-rounds/$electionRoundId/edit';
import { useMutation, useSuspenseQuery } from '@tanstack/react-query';
import { Link, useRouter } from '@tanstack/react-router';
import { useCallback } from 'react';
import { toast } from 'sonner';
import { electionRoundDetailsQueryOptions, electionRoundKeys } from '../../queries';
import ElectionRoundForm, { ElectionRoundRequest } from '../ElectionRoundForm/ElectionRoundForm';
function ElectionRoundEdit() {
  const router = useRouter();
  const { electionRoundId } = Route.useParams();
  const { data: electionRound } = useSuspenseQuery(electionRoundDetailsQueryOptions(electionRoundId));
  const disableEdit = (electionRound?.status || ElectionRoundStatus.NotStarted) !== ElectionRoundStatus.NotStarted;

  const editElectionRoundMutation = useMutation({
    mutationFn: ({
      electionRoundId,
      electionRound,
    }: {
      electionRoundId: string;
      electionRound: ElectionRoundRequest;
    }) => {
      return updateElectionRound(electionRoundId, electionRound);
    },

    onSuccess: async (_, { electionRoundId }) => {
      await queryClient.invalidateQueries({ queryKey: electionRoundKeys.lists() });
      router.invalidate();
      router.navigate({
        to: '/election-rounds/$electionRoundId',
        params: { electionRoundId },
      });

      toast('Election round created');
    },
    onError: () => {
      toast.error('Error creating election round',{
        description: 'Please contact tech support',
      });
    },
  });

  const submitData = useCallback(
    (electionRound: ElectionRoundRequest) => editElectionRoundMutation.mutate({ electionRoundId, electionRound }),
    [editElectionRoundMutation, electionRoundId]
  );

  return (
    <Layout title={electionRound.title} subtitle={electionRound.englishTitle} enableBreadcrumbs={false}>
      <Card>
        <CardHeader>
          <CardTitle>Edit event details</CardTitle>
          <Separator />
        </CardHeader>
        <CardContent>
          <ElectionRoundForm onSubmit={submitData} electionRound={electionRound}>
            <div className='px-6 w-full flex justify-end gap-4'>
              <Link to='/election-rounds/$electionRoundId' params={{ electionRoundId }}>
                <Button title='Cancel' type='button' variant='outline'>
                  Cancel
                </Button>
              </Link>
              <Button title='Save' type='submit' disabled={disableEdit}>
                Save
              </Button>
            </div>
          </ElectionRoundForm>
        </CardContent>
      </Card>
    </Layout>
  );
}

export default ElectionRoundEdit;
