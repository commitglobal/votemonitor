import { useCallback, useEffect, useMemo, useState } from 'react';
import { create } from 'zustand';
import { useMutation } from '@tanstack/react-query';
import { useRouter } from '@tanstack/react-router';
import { authApi } from '@/common/auth-api';
import { Button } from '@/components/ui/button';
import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Separator } from '@/components/ui/separator';
import { Switch } from '@/components/ui/switch';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { toast } from '@/components/ui/use-toast';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useCoalitionDetails } from '@/features/election-event/hooks/coalition-hooks';
import { queryClient } from '@/main';
import { sortBy } from 'lodash';
import { observerGuidesKeys } from '../../hooks/observer-guides-hooks';
import { GuideModel } from '../../models/guide';

export interface EditGuideAccessDialogProps {
  isOpen: boolean;
  guideId: string;
  trigger: (guideId: string) => void;
  dismiss: VoidFunction;
}

export const useEditGuideAccessDialog = create<EditGuideAccessDialogProps>((set) => ({
  isOpen: false,
  guideId: '',
  trigger: (guideId: string) => set({ guideId, isOpen: true }),
  dismiss: () => set({ isOpen: false }),
}));

function EditGuideAccessDialog() {
  const { guideId, isOpen, trigger, dismiss } = useEditGuideAccessDialog();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const guide = queryClient.getQueryData<GuideModel>(observerGuidesKeys.details(currentElectionRoundId, guideId));
  const { data: coalitionDetails } = useCoalitionDetails(currentElectionRoundId);
  const router = useRouter();

  const [searchTerm, setSearchTerm] = useState('');
  const [ngosSharedWith, setNgosSharedWith] = useState<string[]>([]);

  useEffect(() => {
    const sharedWithNgos = guide?.guideAccess?.map((fa) => fa.ngoId) ?? [];
    setNgosSharedWith(sharedWithNgos);
  }, [guide?.guideAccess, isOpen, coalitionDetails?.members.length]);

  const onOpenChange = (open: boolean) => {
    if (open) {
      trigger(guideId);
    } else {
      dismiss();
      setSearchTerm('');
      setNgosSharedWith([]);
    }
  };

  const handleToggle = useCallback((ngoId: string) => {
    setNgosSharedWith((prev) => (prev.includes(ngoId) ? prev.filter((id) => id !== ngoId) : [...prev, ngoId]));
  }, []);

  const filteredNGOs = useMemo(
    () =>
      sortBy(coalitionDetails?.members?.filter((ngo) => ngo.name.toLowerCase().includes(searchTerm.toLowerCase())), [
        (ngo) => ngo.name,
      ]),
    [coalitionDetails?.members, searchTerm]
  );

  const guideAccessMutation = useMutation({
    mutationFn: ({
      electionRoundId,
      coalitionId,
      guideId,
      ngoMembersIds,
    }: {
      electionRoundId: string;
      coalitionId: string;
      guideId: string;
      ngoMembersIds: string[];
    }) =>
      authApi.put<void>(`/election-rounds/${electionRoundId}/coalitions/${coalitionId}/guides/${guideId}:access`, {
        ngoMembersIds,
      }),
    onSuccess: async () => {
      toast({ title: 'Success', description: 'Access modified' });
      await queryClient.invalidateQueries({ queryKey: observerGuidesKeys.all(currentElectionRoundId) });
      router.invalidate();
      dismiss();
    },
  });

  function handleSubmit() {
    guideAccessMutation.mutate({
      electionRoundId: currentElectionRoundId,
      coalitionId: coalitionDetails!.id,
      guideId,
      ngoMembersIds: ngosSharedWith,
    });
  }

  const handleToggleAll = (checked: boolean) => {
    if (checked) {
      setNgosSharedWith(filteredNGOs.map((ngo) => ngo.id));
    } else {
      setNgosSharedWith([]);
    }
  };

  const sharedWithAll = useMemo(() => {
    return ngosSharedWith.length === (coalitionDetails?.members.length || 0);
  }, [guide?.guideAccess, isOpen, coalitionDetails?.members.length, ngosSharedWith]);

  return (
    <Dialog open={isOpen} onOpenChange={onOpenChange} modal>
      <DialogContent
        className='min-w-[650px] min-h-[350px]'
        onInteractOutside={(e) => e.preventDefault()}
        onEscapeKeyDown={(e) => e.preventDefault()}>
        <DialogHeader>
          <DialogTitle className='mb-3.5'>Assign this guide to coalition members</DialogTitle>
          <Separator />
        </DialogHeader>
        <DialogDescription className='mt-3.5 text-base text-slate-900'>
          Select coalition members with access to this guide
        </DialogDescription>
        <div className='flex flex-col gap-3'>
          <Input
            type='text'
            placeholder='Search NGOs...'
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className='max-w-sm'
          />
          <div className='flex items-center justify-end mb-2'>
            <span className='mr-2'>Toggle All</span>
            <Switch checked={sharedWithAll} onCheckedChange={(checked) => handleToggleAll(checked)} />
          </div>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>NGO</TableHead>
                <TableHead>Has access</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {filteredNGOs?.map((ngo) => (
                <TableRow key={ngo.id}>
                  <TableCell>{ngo.name}</TableCell>
                  <TableCell>
                    <Switch checked={ngosSharedWith.includes(ngo.id)} onCheckedChange={() => handleToggle(ngo.id)} />
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
          <Separator />
        </div>
        <DialogFooter>
          <DialogClose asChild>
            <Button variant='secondary'>Close</Button>
          </DialogClose>
          <Button className='bg-purple-900 hover:bg-purple-600' onClick={handleSubmit}>
            Update guide access
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

export default EditGuideAccessDialog;
