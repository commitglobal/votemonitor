import { authApi } from '@/common/auth-api';
import { useConfirm } from '@/components/ui/alert-dialog-provider';
import { Button } from '@/components/ui/button';
import { Dialog, DialogClose, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Separator } from '@/components/ui/separator';
import { toast } from '@/components/ui/use-toast';
import LanguagesMultiselect from '@/containers/LanguagesMultiselect';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useLanguages } from '@/features/languages/queries';
import { queryClient } from '@/main';
import { useMutation } from '@tanstack/react-query';
import { difference } from 'lodash';
import { useState } from 'react';
import { create } from 'zustand';
import { formsKeys } from '../../queries';
import { useRouter } from '@tanstack/react-router';

export interface AddTranslationsDialogPropsProps {
    isOpen: boolean;
    formId: string;
    languages: string[];
    trigger: (formId: string, languages: string[]) => void;
    dismiss: VoidFunction;
}

export const useAddTranslationsDialog = create<AddTranslationsDialogPropsProps>((set) => ({
    isOpen: false,
    formId: '',
    languages: [],
    trigger: (formId: string, languages: string[]) => set({ formId, languages, isOpen: true }),
    dismiss: () => set({ isOpen: false }),
}));

function AddTranslationsDialog() {
    const { languages, formId, isOpen, trigger, dismiss } = useAddTranslationsDialog();
    const [newLanguages, setLanguages] = useState<string[]>(languages);
    const { data: appLanguages } = useLanguages();
    const currentElectionRoundId = useCurrentElectionRoundStore(s => s.currentElectionRoundId);
    const confirm = useConfirm();
    const router = useRouter();

    function handleOnChange(values: string[]): void {
        setLanguages(values);
    }

    function handleSubmit(): void {
        addTranslationsMutation.mutate({ electionRoundId: currentElectionRoundId, formId, languageCodes: newLanguages });
    }

    const onOpenChange = (open: boolean) => {
        if (open) trigger(formId, languages);
        else dismiss();
    };

    function getTitle(newLanguages: string[]): string {
        const languagesLabels = appLanguages?.filter(l => newLanguages.includes(l.code)).map(l => `${l.name} / ${l.nativeName}`) ?? [];

        if (languagesLabels.length === 0) return '';
        if (languagesLabels.length === 1) return languagesLabels[0] + ' added';

        const lastLanguage = languagesLabels.pop(); // Remove the last language from the array
        return languagesLabels.join(', ') + ' and ' + lastLanguage + ' added';
    }

    const addTranslationsMutation = useMutation({
        mutationFn: ({ electionRoundId, formId, languageCodes }: { electionRoundId: string; formId: string; languageCodes: string[]; }) => {
            return authApi.put<void>(`/election-rounds/${electionRoundId}/forms/${formId}:addTranslations`, {
                languageCodes
            });
        },

        onSuccess: async (_, { languageCodes }) => {
            toast({
                title: 'Success',
                description: 'Translations added',
            });

            dismiss();
            const addedLanguages = difference(languageCodes, languages);


            await confirm({
                title: getTitle(addedLanguages),
                body: <div>
                    {addedLanguages.length} translations were created to be translated into the selected languages <b>({addedLanguages.join(', ')})</b>.
                    Please note that this is not an automatic translation as you need to manually translate each form in selected languages.
                    <br />
                    <b>You cannot add or delete questions on the translated forms. </b>Any changes you want to make to the questions (deletion or addition of new questions) will be made to the form in the <b>base language</b>, and they will be copied to the translated forms.
                </div>,
                actionButton: 'Ok'

            })
            await queryClient.invalidateQueries({ queryKey: formsKeys.all });
            router.invalidate();
        },
    });

    return (
        <Dialog open={isOpen} onOpenChange={onOpenChange} modal={true} >
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
                    <p className='text-sm text-gray-700'>Languages <span className='text-red-500'>*</span>
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