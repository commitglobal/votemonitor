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

import { FileUploader } from '@/components/ui/file-uploader';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { zodResolver } from '@hookform/resolvers/zod';
import { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { downloadImportExample } from '../../helpers';
import { monitoringObserversKeys } from '../../hooks/monitoring-observers-queries';
export interface ImportMonitoringObserversDialogProps {
  onImportError: (fileId: string) => void;
  open: boolean;
  onOpenChange: (open: any) => void;
}

function ImportMonitoringObserversDialog({ onImportError, open, onOpenChange }: ImportMonitoringObserversDialogProps) {
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);

  const importObserversSchema = z.object({
    file: z.array(z.instanceof(File)).length(1),
  });

  type ImportObserverType = z.infer<typeof importObserversSchema>;

  const form = useForm<ImportObserverType>({
    resolver: zodResolver(importObserversSchema),
    defaultValues: {
      file: [],
    },
  });

  useEffect(() => {
    if (form.formState.isSubmitSuccessful) {
      form.reset({}, { keepValues: false });
      onOpenChange(false);
    }
  }, [form.formState.isSubmitSuccessful, form.reset]);

  const importObserversMutation = useMutation({
    mutationFn: ({ electionRoundId, file }: { electionRoundId: string; file: File }) => {
      // create a new FormData object and append the file to it
      const formData = new FormData();
      formData.append('file', file);

      return authApi.post(`/election-rounds/${electionRoundId}/monitoring-observers:import`, formData, {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      });
    },

    onSuccess: (_, {electionRoundId}) => {
      queryClient.invalidateQueries({ queryKey: monitoringObserversKeys.lists(electionRoundId) });
      queryClient.invalidateQueries({ queryKey: monitoringObserversKeys.tags(electionRoundId) });

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
            variant: 'destructive',
          });
        }
      } else {
        toast({
          title: 'Error importing monitoring observers',
          description: 'Please contact Platform admins',
          variant: 'destructive',
        });
      }

      onOpenChange(false);
    },
  });

  function onSubmit({ file }: ImportObserverType): void {
    importObserversMutation.mutate({ electionRoundId: currentElectionRoundId, file: file[0]! });
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange} modal={true}>
      <DialogContent
        className='min-w-[650px] min-h-[350px]'
        onInteractOutside={(e) => {
          e.preventDefault();
        }}
        onEscapeKeyDown={(e) => {
          e.preventDefault();
        }}>
        <DialogHeader>
          <DialogTitle className='mb-3.5'>Import monitoring observer list</DialogTitle>
          <Separator />
          <DialogDescription>
            <div className='mt-3.5 text-base'>
              In order to successfully import a list of monitoring observers, please use the template provided below.
              Download the template, fill it in with the observer information and then upload it. No other format is
              accepted for import.
            </div>
          </DialogDescription>
        </DialogHeader>
        <div className='flex flex-col gap-3'>
          <p className='text-sm text-gray-700'>
            Download template <span className='text-red-500'>*</span>
          </p>
          <div onClick={downloadImportExample} className='px-3 py-1 rounded-lg cursor-pointer bg-purple-50'>
            <div className='flex flex-row gap-1 text-sm text-purple-900'>
              <ArrowDownTrayIcon className='w-[15px]' />
              monitoring_observers_template.csv
            </div>
            <div className='text-xs text-purple-900'>28kb</div>
          </div>

          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className='w-full space-y-4'>
              <FormField
                control={form.control}
                name='file'
                render={({ field, fieldState }) => (
                  <div className='space-y-6'>
                    <FormItem className='w-full'>

                      <FormControl>
                        <FileUploader
                          value={field.value}
                          onValueChange={field.onChange}
                          maxFileCount={1}
                          maxSize={50 * 1024 * 1024}
                          accept={{ 'text/csv': [] }}
                          disabled={importObserversMutation.isPending}
                          {...fieldState}
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  </div>
                )}
              />
              <Separator />
              <DialogFooter>
                <DialogClose asChild>
                  <Button className='text-purple-900 border border-purple-900 border-input bg-background hover:bg-purple-50 hover:text-purple-600'>
                    Cancel
                  </Button>
                </DialogClose>
                <Button
                  className='bg-purple-900 hover:bg-purple-600'
                  type="submit">
                  Import list
                </Button>
              </DialogFooter>
            </form>
          </Form>
        </div>
      </DialogContent>
    </Dialog>
  );
}

export default ImportMonitoringObserversDialog;
