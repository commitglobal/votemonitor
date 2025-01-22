import Layout from '@/components/layout/Layout';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { Route } from '@/routes/election-rounds/$electionRoundId_.edit';
import { useMutation, useSuspenseQuery } from '@tanstack/react-query';
import { useRouter } from '@tanstack/react-router';
import { electionRoundDetailsQueryOptions, electionRoundKeys } from '../../queries';
import ElectionRoundForm, { ElectionRoundRequest } from '../ElectionRoundForm/ElectionRoundForm';
import { authApi } from '@/common/auth-api';
import { format } from 'date-fns/format';
import { ElectionRoundModel } from '../../models/types';
import { DateOnlyFormat } from '@/common/formats';
import { queryClient } from '@/main';
import { toast } from '@/components/ui/use-toast';
function ElectionRoundEdit() {
  const router = useRouter();
  const { electionRoundId } = Route.useParams();
  const { data: electionRound } = useSuspenseQuery(electionRoundDetailsQueryOptions(electionRoundId));

  const editElectionRoundMutation = useMutation({
    mutationFn: (electionRound: ElectionRoundRequest) => {
      return authApi.put<ElectionRoundModel>(`/election-rounds/${electionRoundId}`, {
        ...electionRound,
        startDate: format(electionRound.startDate, DateOnlyFormat),
      });
    },

    onSuccess: async ({ data }) => {
      await queryClient.invalidateQueries({ queryKey: electionRoundKeys.lists() });
      router.invalidate();
      router.navigate({
        to: '/election-rounds/$electionRoundId',
        params: { electionRoundId: data.id },
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
  return (
    <Layout title={electionRound.title} subtitle={electionRound.englishTitle} enableBreadcrumbs={false}>
      <Card>
        <CardHeader>
          <CardTitle>Edit event details</CardTitle>
          <Separator />
        </CardHeader>
        <CardContent>
          <ElectionRoundForm
            onSubmit={(electionRound) => editElectionRoundMutation.mutate(electionRound)}
            electionRound={electionRound}>
            <Button title='Save' type='submit' className='px-6'>
              Save
            </Button>
          </ElectionRoundForm>
        </CardContent>
      </Card>
    </Layout>
  );
}

export default ElectionRoundEdit;
