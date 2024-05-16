import { authApi } from '@/common/auth-api';
import { Button } from '@/components/ui/button';
import { Dialog, DialogClose, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Separator } from '@/components/ui/separator';
import { toast } from '@/components/ui/use-toast';
import { queryClient } from '@/main';
import { useMutation } from '@tanstack/react-query';
import { useState } from 'react';

export interface EditObserversGuideDialogProps {
    guideId: string;
    title: string;
    open: boolean;
    onOpenChange: (open: any) => void;
}

function EditObserversGuideDialog({
    guideId,
    title,
    open,
    onOpenChange
}: EditObserversGuideDialogProps) {
    const [guideTitle, setGuideTitle] = useState<string | undefined>(title);


    const updateObserverGuideMutation = useMutation({
        mutationFn: () => {
            const electionRoundId: string | null = localStorage.getItem('electionRoundId');

            return authApi.put<void>(
                `/election-rounds/${electionRoundId}/observer-guide/${guideId}`,
                {
                    title: guideTitle!
                });
        },

        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['observer-guides'] });
            onOpenChange(false);

            toast({
                title: 'Success',
                description: 'Update was successful',
            });
        },

        onError: () => {
            toast({
                title: 'Error updating observer guide',
                description: 'Please contact Platform admins',
                variant: 'destructive'
            });

            onOpenChange(false);
        }
    });

    const handleUpdate = () => {
        updateObserverGuideMutation.mutate();
    }


    const canUpdateGuide = () => {
        const hasTitle = !!guideTitle?.trim();
        return !hasTitle;
    }

    return (
        <Dialog open={open} onOpenChange={onOpenChange} modal={true}>
            <DialogContent className='min-w-[650px]' onInteractOutside={(e) => {
                e.preventDefault();
            }} onEscapeKeyDown={(e) => {
                e.preventDefault();
            }}>
                <DialogHeader>
                    <DialogTitle className='mb-3.5'>Update observer guide title</DialogTitle>
                    <Separator />
                </DialogHeader>
                <div className='flex flex-col gap-3'>
                    <Label htmlFor={`guideTitle-${guideId}`}>{'Guide title'}</Label>
                    <Input
                        id={`guideTitle-${guideId}`}
                        name="guideTitle"
                        value={guideTitle}
                        onChange={(e) => setGuideTitle(e.target.value)}
                        className={!!guideTitle ? "border-red-300 focus:border-red-300" : ""}
                    />
                    <Separator />
                </div>
                <DialogFooter>
                    <DialogClose asChild>
                        <Button className='border border-input border-purple-900 bg-background hover:bg-purple-50 text-purple-900 hover:text-purple-600'>
                            Cancel
                        </Button>
                    </DialogClose>
                    <Button className='bg-purple-900 hover:bg-purple-600' onClick={handleUpdate} disabled={canUpdateGuide()}>Update guide</Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    )
}

export default EditObserversGuideDialog;