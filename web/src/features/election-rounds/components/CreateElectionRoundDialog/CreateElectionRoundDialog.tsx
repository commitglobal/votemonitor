import { DateOnlyFormat } from '@/common/formats';
import { Button } from '@/components/ui/button';
import { Dialog, DialogClose, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Separator } from '@/components/ui/separator';
import { queryClient } from '@/main';
import API from '@/services/api';
import { useMutation } from '@tanstack/react-query';
import { useRouter } from '@tanstack/react-router';
import { format } from 'date-fns/format';
import { toast } from 'sonner';
import { ElectionRoundModel } from '../../models/types';
import { electionRoundKeys } from '../../queries';
import ElectionRoundForm, { ElectionRoundRequest } from '../ElectionRoundForm/ElectionRoundForm';

export interface ElectionRoundFormProps {
  open: boolean;
  onOpenChange: (open: any) => void;
}

function CreateElectionRoundDialog({ open, onOpenChange }: ElectionRoundFormProps) {
  const router = useRouter();

  const createElectionRoundMutation = useMutation({
    mutationFn: (electionRound: ElectionRoundRequest) => {
      return API.post<ElectionRoundModel>(`/election-rounds`, {
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
      onOpenChange(false);

      toast.success('Election round created');
    },

    onError: () => {
      toast.error('Error creating election round', {
        description: 'Please contact Platform admins',
      });
    },
  });

  return (
    <Dialog open={open} onOpenChange={onOpenChange} modal={true}>
      <DialogContent
        className='max-w-[650px]'
        onInteractOutside={(e) => {
          e.preventDefault();
        }}
        onEscapeKeyDown={(e) => {
          e.preventDefault();
        }}>
        <DialogHeader>
          <DialogTitle className='mb-3.5'>New election round</DialogTitle>
          <Separator />
        </DialogHeader>
        <div className='flex flex-col gap-3 w-full'>
          <ElectionRoundForm onSubmit={(electionRound) => createElectionRoundMutation.mutate(electionRound)}>
            <DialogFooter>
              <DialogClose asChild>
                <Button className='text-purple-900 border border-purple-900 border-input bg-background hover:bg-purple-50 hover:text-purple-600'>
                  Cancel
                </Button>
              </DialogClose>
              <Button title='Create' type='submit' className='px-6'>
                Create
              </Button>
            </DialogFooter>
          </ElectionRoundForm>
        </div>
      </DialogContent>
    </Dialog>
  );
}

export default CreateElectionRoundDialog;
