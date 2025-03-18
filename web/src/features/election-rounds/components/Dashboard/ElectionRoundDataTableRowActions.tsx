import * as React from 'react';

import { Button } from '@/components/ui/button';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';

import { useConfirm } from '@/components/ui/alert-dialog-provider';
import { Dialog } from '@/components/ui/dialog';
import { ArchiveIcon, MoreHorizontal, Pencil, PlayIcon, Trash2, FileEdit, Eye } from 'lucide-react';
import { ElectionRoundModel } from '../../models/types';
import { Route } from '@/routes/election-rounds';
import { ElectionRoundStatus } from '@/common/types';
import {
  useArchiveElectionRound,
  useDeleteElectionRound,
  useStartElectionRound,
  useUnarchiveElectionRound,
  useUnstartElectionRound,
} from '../../hooks';
import { useToast } from '@/components/ui/use-toast';

interface ElectionRoundDataTableRowActionsProps {
  electionRound: ElectionRoundModel;
}

export function ElectionRoundDataTableRowActions({ electionRound }: ElectionRoundDataTableRowActionsProps) {
  const confirm = useConfirm();
  const navigate = Route.useNavigate();
  const { toast } = useToast();

  const { mutate: deleteElectionRound } = useDeleteElectionRound();
  const { mutate: unstartElectionRound } = useUnstartElectionRound();
  const { mutate: startElectionRound } = useStartElectionRound();
  const { mutate: archiveElectionRound } = useArchiveElectionRound();
  const { mutate: unarchiveElectionRound } = useUnarchiveElectionRound();

  const handleDeleteElectionRound = React.useCallback(async () => {
    if (
      await confirm({
        title: `Delete election round: ${electionRound.englishTitle}?`,
        body: 'Are you sure you want to delete this election round?',
      })
    ) {
      deleteElectionRound({
        electionRoundId: electionRound.id,
        onSuccess: () =>
          toast({
            title: 'Election round deleted successfully',
          }),
        onError: () =>
          toast({
            title: 'Error deleting election round',
            description: 'Please contact tech support',
            variant: 'destructive',
          }),
      });
    }
  }, [electionRound, confirm]);

  const handleArchiveElectionRound = React.useCallback(async () => {
    if (
      await confirm({
        title: `Archive election round: ${electionRound.englishTitle}?`,
        body: 'Are you sure you want to archive this election round?',
        cancelButton: 'Cancel',
      })
    ) {
      archiveElectionRound({
        electionRoundId: electionRound.id,
        onSuccess: () =>
          toast({
            title: 'Election round archived successfully',
          }),
        onError: () =>
          toast({
            title: 'Error archiving election round',
            description: 'Please contact tech support',
            variant: 'destructive',
          }),
      });
    }
  }, [electionRound, confirm]);

  const handleUnstartElectionRound = React.useCallback(async () => {
    if (
      await confirm({
        title: `Draft election round: ${electionRound.englishTitle}?`,
        body: 'Are you sure you want to draft this election round?',
        cancelButton: 'Cancel',
      })
    ) {
      unstartElectionRound({
        electionRoundId: electionRound.id,
        onSuccess: () =>
          toast({
            title: 'Election round drafted successfully',
          }),
        onError: () =>
          toast({
            title: 'Error drafting election round',
            description: 'Please contact tech support',
            variant: 'destructive',
          }),
      });
    }
  }, [electionRound, confirm]);

  const handleUnarchiveElectionRound = React.useCallback(async () => {
    if (
      await confirm({
        title: `Unarchive election round: ${electionRound.englishTitle}?`,
        body: 'Are you sure you want to unarchive this election round?',
        cancelButton: 'Cancel',
      })
    ) {
      unarchiveElectionRound({
        electionRoundId: electionRound.id,
        onSuccess: () =>
          toast({
            title: 'Election round unarchived successfully',
          }),
        onError: () =>
          toast({
            title: 'Error unarchiving election round',
            description: 'Please contact tech support',
            variant: 'destructive',
          }),
      });
    }
  }, [electionRound, confirm]);

  const handleStartElectionRound = React.useCallback(async () => {
    if (
      await confirm({
        title: `Start election round: ${electionRound.englishTitle}?`,
        body: 'Are you sure you want to start this election round?',
        cancelButton: 'Cancel',
      })
    ) {
      startElectionRound({
        electionRoundId: electionRound.id,
        onSuccess: () =>
          toast({
            title: 'Election round started successfully',
          }),
        onError: () =>
          toast({
            title: 'Error starting election round',
            description: 'Please contact tech support',
            variant: 'destructive',
          }),
      });
    }
  }, [electionRound, confirm]);

  return (
    <Dialog>
      <DropdownMenu>
        <DropdownMenuTrigger asChild>
          <Button variant='ghost' className='flex h-8 w-8 p-0 data-[state=open]:bg-muted'>
            <MoreHorizontal className='h-4 w-4' />
            <span className='sr-only'>Open menu</span>
          </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent align='end' className='w-[200px]'>
          <DropdownMenuLabel>Actions</DropdownMenuLabel>
          <DropdownMenuSeparator />
          <DropdownMenuItem
            onClick={() =>
              navigate({ to: '/election-rounds/$electionRoundId', params: { electionRoundId: electionRound.id } })
            }>
            <Eye className='mr-2 h-4 w-4' />
            View
          </DropdownMenuItem>

          <DropdownMenuItem
            onClick={() =>
              navigate({ to: '/election-rounds/$electionRoundId/edit', params: { electionRoundId: electionRound.id } })
            }>
            <Pencil className='mr-2 h-4 w-4' />
            Edit
          </DropdownMenuItem>

          {!(electionRound.status === ElectionRoundStatus.Archived) ? (
            <DropdownMenuItem onSelect={handleArchiveElectionRound}>
              <ArchiveIcon className='mr-2 h-4 w-4' />
              Archive
            </DropdownMenuItem>
          ) : (
            <DropdownMenuItem onSelect={handleUnarchiveElectionRound}>
              <ArchiveIcon className='mr-2 h-4 w-4' />
              Unarchive
            </DropdownMenuItem>
          )}

          {!(electionRound.status === ElectionRoundStatus.Started) ? (
            <DropdownMenuItem onSelect={handleStartElectionRound}>
              <PlayIcon className='mr-2 h-4 w-4' />
              Start
            </DropdownMenuItem>
          ) : (
            <DropdownMenuItem onSelect={handleUnstartElectionRound}>
              <FileEdit className='mr-2 h-4 w-4' />
              Draft
            </DropdownMenuItem>
          )}

          <DropdownMenuItem onSelect={handleDeleteElectionRound} className='text-red-600'>
            <Trash2 className='mr-2 h-4 w-4' />
            Delete
          </DropdownMenuItem>
          <DropdownMenuSeparator />
        </DropdownMenuContent>
      </DropdownMenu>
    </Dialog>
  );
}
