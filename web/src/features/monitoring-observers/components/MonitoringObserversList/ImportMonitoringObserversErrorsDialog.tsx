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
import { ArrowDownTrayIcon } from '@heroicons/react/24/outline';

export interface ImportMonitoringObserversErrorsDialogProps {
    fileId: string;
    open: boolean;
    onOpenChange: (open: any) => void;
}

function ImportMonitoringObserversErrorsDialog({
    fileId,
    open,
    onOpenChange
}: ImportMonitoringObserversErrorsDialogProps) {

    const downloadImportErrorsFile = async () => {
        const res = await authApi.get(`/import-errors/${fileId}`, { responseType: "blob" });
        const csvData = res.data;

        const blob = new Blob([csvData], { type: 'text/csv' });
        const url = window.URL.createObjectURL(blob);

        const a = document.createElement('a');
        a.style.display = 'none';
        a.href = url;
        a.download = 'import-errors.csv';

        document.body.appendChild(a);
        a.click();

        window.URL.revokeObjectURL(url);
    };

    return (
        <Dialog open={open} onOpenChange={onOpenChange} modal={true}>
            <DialogContent className='min-w-[650px] min-h-[350px]' onInteractOutside={(e) => {
                e.preventDefault();
            }} onEscapeKeyDown={(e) => {
                e.preventDefault();
            }}>
                <DialogHeader>
                    <DialogTitle className='mb-3.5'>Data import failed</DialogTitle>
                    <Separator />
                    <DialogDescription>
                        <div className='mt-3.5 text-base text-gray-700'>
                            We encountered issues during the data import process from your CSV file.
                            To assist you in resolving these issues, you can download a detailed error report by clicking the link provided below.
                            We kindly ask you to review this report thoroughly and address any errors before proceeding with the reimport of your data.
                        </div>
                    </DialogDescription>
                </DialogHeader>
                <div className='flex flex-col gap-3'>
                    <p className='text-sm text-gray-700'>
                        Download error template
                    </p>
                    <div
                        onClick={downloadImportErrorsFile}
                        className='px-3 py-1 bg-red-50 rounded-lg cursor-pointer'>
                        <div className='text-sm text-red-500 flex flex-row gap-1'>
                            <ArrowDownTrayIcon className='w-[15px]' />
                            import-errors.csv
                        </div>
                    </div>
                    <div className='text-sm text-slate-700 font-normal'>
                        If you require any further assistance or have questions, please do not hesitate to reach out to our support team
                    </div>
                    <Separator />
                </div>
                <DialogFooter>
                    <DialogClose asChild>
                        <Button className='bg-purple-900 hover:bg-purple-600'>
                            Ok
                        </Button>
                    </DialogClose>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    )
}

export default ImportMonitoringObserversErrorsDialog;