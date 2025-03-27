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
import LanguagesMultiselect from '@/containers/LanguagesMultiselect';
import { useState } from 'react';
import { create } from 'zustand';

export interface AddFormTranslationsDialogPropsProps {
  isOpen: boolean;
  formId: string;
  languages: string[];
  trigger: (
    formId: string,
    languages: string[],
    onAddTranslations: (formId: string, newLanguages: string[]) => void
  ) => void;
  onAddTranslations: (formId: string, newLanguages: string[]) => void;
  dismiss: VoidFunction;
}

export const useAddFormTranslationsDialog = create<AddFormTranslationsDialogPropsProps>((set) => ({
  isOpen: false,
  formId: '',
  languages: [],
  trigger: (formId: string, languages: string[], onAddTranslations: (formId: string, newLanguages: string[]) => void) =>
    set({ formId, languages, isOpen: true, onAddTranslations }),
  dismiss: () => set({ isOpen: false }),
  onAddTranslations: (formId: string, newLanguages: string[]) => {},
}));

function AddFormTranslationsDialog() {
  const { languages, formId, isOpen, trigger, dismiss, onAddTranslations } = useAddFormTranslationsDialog();
  const [newLanguages, setLanguages] = useState<string[]>(languages);

  function handleOnChange(values: string[]): void {
    setLanguages(values);
  }

  const onOpenChange = (open: boolean) => {
    if (open) trigger(formId, languages, onAddTranslations);
    else dismiss();
  };

  return (
    <Dialog open={isOpen} onOpenChange={onOpenChange} modal={true}>
      <DialogContent
        className='min-w-[650px] min-h-[350px]'
        onInteractOutside={(e) => {
          e.preventDefault();
        }}
        onEscapeKeyDown={(e) => {
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
            <Button type='button' variant='secondary'>
              Close
            </Button>
          </DialogClose>
          <Button className='bg-purple-900 hover:bg-purple-600' onClick={() => onAddTranslations(formId, newLanguages)}>
            Add translations
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

export default AddFormTranslationsDialog;
