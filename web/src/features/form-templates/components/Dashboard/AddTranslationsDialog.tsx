import { authApi } from '@/common/auth-api';
import { Button } from '@/components/ui/button';
import { Dialog, DialogClose, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Separator } from '@/components/ui/separator';
import { toast } from '@/components/ui/use-toast';
import LanguagesMultiselect from '@/containers/LanguagesMultiselect';
import { useMutation } from '@tanstack/react-query';
import { useState } from 'react';
import { formTemplatesKeys } from '../../queries';
import { queryClient } from '@/main';


export interface AddTranslationsDialogProps {
    formTemplateId: string;
    languages: string[];
    open: boolean;
    onOpenChange: (open: any) => void;
}

function AddTranslationsDialog({
    formTemplateId,
    languages,
    open,
    onOpenChange
}: AddTranslationsDialogProps) {
    const [newLanguages, setLanguages] = useState(languages);

    function handleOnChange(values: string[]): void {
        setLanguages(values);
    }

    function handleSubmit(): void {
        addTranslationsMutation.mutate({formTemplateId, languageCodes: newLanguages });
    }

    
  const addTranslationsMutation = useMutation({
    mutationFn: ({ formTemplateId, languageCodes }: { formTemplateId: string; languageCodes: string[]; }) => {
      return authApi.put<void>(`/form-templates/${formTemplateId}:addTranslations`, {
        languageCodes
      });
    },

    onSuccess: () => {
      toast({
        title: 'Success',
        description: 'Translations added',
      });

      onOpenChange(false);
      queryClient.invalidateQueries({ queryKey: formTemplatesKeys.all });
    },
  });

    return (
        <Dialog open={open} onOpenChange={onOpenChange} modal={true} >
            <DialogContent className='min-w-[650px] min-h-[350px]' onInteractOutside={(e) => {
                e.preventDefault();
            }} onEscapeKeyDown={(e) => {
                e.preventDefault();
            }}>
                <DialogHeader>
                    <DialogTitle className='mb-3.5'>Add language(s)</DialogTitle>
                    <Separator />
                    <DialogDescription className='mt-3.5 text-base text-slate-900'>
                        Select one or more languages in which you want to translate the form
                    </DialogDescription>
                </DialogHeader>
                <div className='flex flex-col gap-3'>
                    <p className='text-sm text-gray-700'>
                        Languages <span className='text-red-500'>*</span>
                    </p>
                    <LanguagesMultiselect
                        defaultLanguages={languages}
                        value={languages}
                        placeholder='Select languages'
                        onChange={handleOnChange}
                        />
                    <Separator />
                </div>
                <DialogFooter>
                    <DialogClose asChild>
                        <Button type="button" variant="secondary">
                            Close
                        </Button>
                    </DialogClose>
                    <Button className='bg-purple-900 hover:bg-purple-600' onClick={handleSubmit}>Add translations</Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    )
}

export default AddTranslationsDialog