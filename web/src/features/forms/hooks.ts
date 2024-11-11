import { authApi } from '@/common/auth-api';
import { toast } from '@/components/ui/use-toast';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { create } from 'zustand';
import { FormFull } from './models/form';
import { formsKeys, useFormTemplateDetails } from './queries';

export interface PreviewTemplateDialogProps {
  isOpen: boolean;
  templateId: string;
  languageCode: string;
  trigger: (templateId: string, languageCode: string) => void;
  dismiss: VoidFunction;
}

export const usePreviewTemplateDialog = create<PreviewTemplateDialogProps>((set) => ({
  isOpen: false,
  templateId: '',
  languageCode: '',
  trigger: (templateId: string, languageCode: string) => set({ templateId, languageCode, isOpen: true }),
  dismiss: () => set({ isOpen: false }),
}));

export const useCreateFormFromTemplate = () => {
  const { isOpen, templateId, languageCode, trigger, dismiss } = usePreviewTemplateDialog();
  const { data: templateDetails } = useFormTemplateDetails(templateId);
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const createFormFromTemplateMutation = useMutation({
    mutationFn: () => {
      return authApi.post<FormFull>(`/election-rounds/${currentElectionRoundId}/forms:fromTemplate`, {
        templateId,
        defaultLanguage: languageCode,
        languages: templateDetails?.languages,
      });
    },
    onSuccess: (response) => {
      toast({
        title: 'Success',
        description: 'Form created from template',
      });
      queryClient.invalidateQueries({ queryKey: formsKeys.all(currentElectionRoundId) });
      navigate({ to: '/forms/$formId/edit', params: { formId: response.data.id } });
    },

    onError: (err) =>
      toast({
        title: 'Error creating form from template',
        description: 'Please contact tech support',
        variant: 'destructive',
      }),

    onSettled: () => dismiss(),
  });

  return { createFormFromTemplateMutation };
};
