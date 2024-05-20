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
import { Separator } from '@/components/ui/separator';
import { toast } from '@/components/ui/use-toast';
import { queryClient } from '@/main';
import { ArrowDownTrayIcon } from '@heroicons/react/24/outline';
import { useMutation } from '@tanstack/react-query';
import { AxiosError } from 'axios';
import { useRef, useState } from 'react';

import { downloadImportExample } from '../../helpers';

export interface ImportMonitoringObserversDialogProps {
    onImportError: (fileId: string) => void;
    open: boolean;
    onOpenChange: (open: any) => void;
}

function ImportMonitoringObserversDialog({
    onImportError,
    open,
    onOpenChange
}: ImportMonitoringObserversDialogProps) {
    const [fileName, setFileName] = useState('');
    const hiddenFileInput: React.Ref<any> = useRef(null);

    const handleClick = () => {
        hiddenFileInput?.current?.click();
    };

    const importObserversMutation = useMutation({
        mutationFn: () => {
            const electionRoundId: string | null = localStorage.getItem('electionRoundId');

            // get the selected file from the input
            const file = hiddenFileInput.current.files[0];
            // create a new FormData object and append the file to it
            const formData = new FormData();
            formData.append("file", file);

            return authApi.post(
                `/election-rounds/${electionRoundId}/monitoring-observers:import`,
                formData, {
                headers: {
                    "Content-Type": "multipart/form-data",
                }
            });
        },

        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['monitoring-observers'] });
            queryClient.invalidateQueries({ queryKey: ['tags'] });
            onOpenChange(false);

            toast({
                title: 'Success',
                description: 'Import was successful',
            });
        },

        onError: (error: AxiosError, variables, ctx) => {
            if (error.response?.status === 400) {
                // @ts-ignore
                const importErrorsFileId = error.response.data.id;
                if (importErrorsFileId) {
                    onImportError(importErrorsFileId);
                } else {
                    toast({
                        title: 'Error importing monitoring observers',
                        description: 'Please contact Platform admins',
                        variant: 'destructive'
                    });
                }
            }

            onOpenChange(false);
        }
    });


    const handleImport = () => {
        importObserversMutation.mutate();
    }

    const handleChange = (event: any) => {
        const fileUploaded = event.target.files[0];
        setFileName(fileUploaded.name);
    };

    return (
        <Dialog open={open} onOpenChange={onOpenChange} modal={true}>
            <DialogContent className='min-w-[650px] min-h-[350px]' onInteractOutside={(e) => {
                e.preventDefault();
            }} onEscapeKeyDown={(e) => {
                e.preventDefault();
            }}>
                <DialogHeader>
                    <DialogTitle className='mb-3.5'>Import monitoring observer list</DialogTitle>
                    <Separator />
                    <DialogDescription>
                        <div className='mt-3.5 text-base'>
                            In order to successfully import a list of monitoring observers, please use the template
                            provided below. Download the template, fill it in with the observer information and then
                            upload it. No other format is accepted for import.
                        </div>
                    </DialogDescription>
                </DialogHeader>
                <div className='flex flex-col gap-3'>
                    <p className='text-sm text-gray-700'>
                        Download template <span className='text-red-500'>*</span>
                    </p>
                    <div
                        onClick={downloadImportExample}
                        className='px-3 py-1 bg-purple-50 rounded-lg cursor-pointer'>
                        <div className='text-sm text-purple-900 flex flex-row gap-1'>
                            <ArrowDownTrayIcon className='w-[15px]' />
                            monitoring_observers_template.csv
                        </div>
                        <div className='text-xs text-purple-900'>28kb</div>
                    </div>
                    <input type='file' ref={hiddenFileInput} onChange={handleChange} style={{ display: 'none' }} />
                    <Button onClick={handleClick} variant='outline'>
                        <span className='text-gray-500 font-normal'>
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
                    <Button className='bg-purple-900 hover:bg-purple-600' onClick={handleImport} disabled={!(!!hiddenFileInput?.current?.files?.length)}>Import list</Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    )
}

export default ImportMonitoringObserversDialog;