import { authApi } from '@/common/auth-api';
import { DateOnlyFormat } from '@/common/formats';
import Layout from '@/components/layout/Layout';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { toast } from '@/components/ui/use-toast';
import { queryClient } from '@/main';
import { Route } from '@/routes/election-rounds/$electionRoundId/edit';
import { useMutation, useSuspenseQuery } from '@tanstack/react-query';
import { Link, useRouter } from '@tanstack/react-router';
import { format } from 'date-fns/format';
import { useCallback } from 'react';
import { electionRoundDetailsQueryOptions, electionRoundKeys } from '../../queries';
import ElectionRoundForm, { ElectionRoundRequest } from '../ElectionRoundForm/ElectionRoundForm';
function ElectionRoundEdit() {
  const router = useRouter();
  const { electionRoundId } = Route.useParams();
  const { data: electionRound } = useSuspenseQuery(electionRoundDetailsQueryOptions(electionRoundId));

  const editElectionRoundMutation = useMutation({
    mutationFn: ({
      electionRoundId,
      electionRound,
    }: {
      electionRoundId: string;
      electionRound: ElectionRoundRequest;
    }) => {
      return authApi.put(`/election-rounds/${electionRoundId}`, {
        ...electionRound,
        startDate: format(electionRound.startDate, DateOnlyFormat),
      });
    },

    onSuccess: async (_, { electionRoundId }) => {
      await queryClient.invalidateQueries({ queryKey: electionRoundKeys.lists() });
      router.invalidate();
      router.navigate({
        to: '/election-rounds/$electionRoundId',
        params: { electionRoundId },
      });

      toast({
        title: 'Success',
        description: 'Election round created',
      });
    },

    onError: () => {
      toast({
        title: 'Error creating election round',
        description: 'Please contact Platform admins',
        variant: 'destructive',
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
                <Button title='Cancel' type='button' variant='outline'>Cancel</Button>
              </Link>
              <Button title='Save' type='submit'>
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
