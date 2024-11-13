import { authApi } from '@/common/auth-api';
import { toast } from '@/components/ui/use-toast';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { create } from 'zustand';
import { FormFull } from './models/form';
import { formsKeys } from './queries';

export interface PreviewDialogProps {
  isOpen: boolean;
  id: string;
  languageCode: string;
  trigger: (templateId: string, languageCode: string) => void;
  dismiss: VoidFunction;
}

export const usePreviewTemplateDialog = create<PreviewDialogProps>((set) => ({
  isOpen: false,
  id: '',
  languageCode: '',
  trigger: (templateId: string, languageCode: string) => set({ id: templateId, languageCode, isOpen: true }),
  dismiss: () => set({ isOpen: false }),
}));

type FormFromTemplateDto = {
  templateId: string;
  languageCode: string;
};

export const useCreateFormFromTemplate = () => {
  const { trigger, dismiss, isOpen } = usePreviewTemplateDialog();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const createFormFromTemplateMutation = useMutation({
    mutationFn: ({ templateId, languageCode }: FormFromTemplateDto) => {
      return authApi.post<FormFull>(`/election-rounds/${currentElectionRoundId}/forms:fromTemplate`, {
        templateId,
        defaultLanguage: languageCode,
        languages: [languageCode],
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

    onSettled: () => {
      if (isOpen) dismiss();
    },
  });

  const createForm = (dto: FormFromTemplateDto) => {
    return createFormFromTemplateMutation.mutate(dto);
  };

  const openFormTemplatePreview = (dto: FormFromTemplateDto) => {
    trigger(dto.templateId, dto.languageCode);
  };

  return { openFormTemplatePreview, createForm };
};
