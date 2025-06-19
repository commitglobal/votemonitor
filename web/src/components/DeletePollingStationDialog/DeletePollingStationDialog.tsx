import { PollingStation } from '@/common/types';
import { Button } from '@/components/ui/button';
import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { pollingStationsKeys } from '@/hooks/polling-stations-levels';
import { queryClient } from '@/main';
import { useRouter } from '@tanstack/react-router';
import type { Row } from '@tanstack/react-table';
import { Loader, Trash } from 'lucide-react';
import * as React from 'react';
import { toast } from 'sonner';
import { useDeletePollingStationMutation } from '../PollingStationsDashboard/hooks';

interface DeletePollingStationDialogProps extends React.ComponentPropsWithoutRef<typeof Dialog> {
  pollingStation: Row<PollingStation>['original'] | undefined;
  showTrigger?: boolean;
  onActionCompleted?: () => void;
}

export function DeletePollingStationDialog({
  pollingStation,
  showTrigger = false,
  ...props
}: DeletePollingStationDialogProps) {
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);

  const router = useRouter();

  const onSuccess = () => {
    queryClient.invalidateQueries({ queryKey: pollingStationsKeys.all(currentElectionRoundId) });
    router.invalidate();
    props.onOpenChange?.(false);

    toast.success('Polling station deleted');
  };
  const onError = () => toast.error('Error occured when deleting polling station');

  const { mutate: deletePollingStationMutation, isPending: isDeletePending } = useDeletePollingStationMutation(
    onSuccess,
    onError
  );

  function onDelete() {
    if (!pollingStation) return;
    deletePollingStationMutation({
      electionRoundId: currentElectionRoundId,
      pollingStationId: pollingStation.id,
    });
  }

  return (
    <Dialog {...props}>
      {showTrigger ? (
        <DialogTrigger asChild>
          <Button variant='outline' size='sm'>
            <Trash className='mr-2 size-4' aria-hidden='true' />
            Delete polling station
          </Button>
        </DialogTrigger>
      ) : null}
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Are you absolutely sure?</DialogTitle>
          <DialogDescription>
            This action cannot be undone. This will permanently delete selected polling station
          </DialogDescription>
        </DialogHeader>
        <DialogFooter className='gap-2 sm:space-x-0'>
          <DialogClose asChild>
            <Button variant='outline'>Cancel</Button>
          </DialogClose>
          <Button aria-label='Delete selected rows' variant='destructive' onClick={onDelete} disabled={isDeletePending}>
            {isDeletePending && <Loader className='mr-2 size-4 animate-spin' aria-hidden='true' />}
            Delete
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
