import { authApi } from '@/common/auth-api';
import { Button } from '@/components/ui/button';
import { Dialog, DialogClose, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Separator } from '@/components/ui/separator';
import { toast } from '@/components/ui/use-toast';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { queryClient } from '@/main';
import { useMutation } from '@tanstack/react-query';
import { useRef, useState } from 'react';

export interface UploadObserversGuideDialogProps {
    open: boolean;
    onOpenChange: (open: any) => void;
}

function UploadObserversGuideDialog({
    open,
    onOpenChange
}: UploadObserversGuideDialogProps) {
    const [fileName, setFileName] = useState('');
    const [guideTitle, setGuideTitle] = useState<string | undefined>('');
    const [websiteUrl, setWebsiteUrl] = useState<string | undefined>('');
    const hiddenFileInput: React.Ref<any> = useRef(null);
      const currentElectionRoundId = useCurrentElectionRoundStore(s => s.currentElectionRoundId);

    const handleClick = () => {
        hiddenFileInput?.current?.click();
    };

    const uploadObserverGuideMutation = useMutation({
        mutationFn: ({ electionRoundId }: { electionRoundId: string }) => {

            // get the selected file from the input
            const file = hiddenFileInput.current.files[0];
            // create a new FormData object and append the file to it
            const formData = new FormData();
            formData.append("Title", guideTitle!);
            formData.append("Attachment", file);
            formData.append("WebsiteUrl", websiteUrl ?? '');


            return authApi.post(
                `/election-rounds/${electionRoundId}/observer-guide`,
                formData, {
                headers: {
                    "Content-Type": "multipart/form-data",
                }
            });
        },

        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['observer-guides'] });
            onOpenChange(false);

            toast({
                title: 'Success',
                description: 'Upload was successful',
            });
        },

        onError: () => {
            toast({
                title: 'Error uploading observer guide',
                description: 'Please contact Platform admins',
                variant: 'destructive'
            });

            onOpenChange(false);
        }
    });


    const handleImport = () => {
        uploadObserverGuideMutation.mutate({ electionRoundId: currentElectionRoundId });
    }

    const handleChange = (event: any) => {
        const fileUploaded = event.target.files[0];
        setFileName(fileUploaded.name);
    };

    const canUploadGuide = () => {
        const hasSelectedFile = !!hiddenFileInput?.current?.files?.length;
        const hasWebsiteUrl = !!websiteUrl?.trim();
        const hasTitle = !!guideTitle?.trim();
        return !(hasTitle && (hasSelectedFile || hasWebsiteUrl));
    }

    return (
        <Dialog open={open} onOpenChange={onOpenChange} modal={true}>
            <DialogContent className='max-w-[650px] min-h-[350px]' onInteractOutside={(e) => {
                e.preventDefault();
            }} onEscapeKeyDown={(e) => {
                e.preventDefault();
            }}>
                <DialogHeader>
                    <DialogTitle className='mb-3.5'>Upload observer guide</DialogTitle>
                    <Separator />
                </DialogHeader>
                <div className='flex flex-col gap-3 max-w-[600px]'>
                    <Label htmlFor="guideTitle">{'Guide title'}</Label>
                    <Input
                        id="guideTitle"
                        name="guideTitle"
                        value={guideTitle}
                        onChange={(e) => setGuideTitle(e.target.value)}
                        className={!!guideTitle ? "border-red-300 focus:border-red-300" : ""}
                    />
                    <Label htmlFor="guideWebsiteUrl">{'Guide URL'}</Label>
                    <Input
                        id="guideWebsiteUrl"
                        name="guideWebsiteUrl"
                        value={websiteUrl}
                        placeholder={'https://'}
                        onChange={(e) => setWebsiteUrl(e.target.value)}
                        className={!!websiteUrl ? "border-red-300 focus:border-red-300" : ""}
                    />
                    <Label htmlFor="guideFile">{'Guide file'}</Label>

                    <input type='file' id="guideFile" ref={hiddenFileInput} onChange={handleChange} style={{ display: 'none' }} accept='.csv' />
                    <Button onClick={handleClick} variant='outline' className=''>
                        <span className='text-gray-500 font-normal truncate'>
                            {fileName || (
                                <div>
                                    Drag & drop your files or <span className='underline'>Browse</span>
                                </div>
                            )}
                        </span>
                    </Button>
                    <Separator />
                </div>
                <DialogFooter>
                    <DialogClose asChild>
                        <Button className='border border-input border-purple-900 bg-background hover:bg-purple-50 text-purple-900 hover:text-purple-600'>
                            Cancel
                        </Button>
                    </DialogClose>
                    <Button className='bg-purple-900 hover:bg-purple-600' onClick={handleImport} disabled={canUploadGuide()}>Upload guide</Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    )
}

export default UploadObserversGuideDialog;