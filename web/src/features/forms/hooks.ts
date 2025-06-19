import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import API from '@/services/api';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { toast } from 'sonner';
import { create } from 'zustand';
import { FormFull } from './models';
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
      return API.post<FormFull>(`/election-rounds/${currentElectionRoundId}/forms:fromTemplate`, {
        templateId,
        defaultLanguage: languageCode,
        languages: [languageCode],
      });
    },
    onSuccess: (response) => {
      toast.success('Form created from template');
      queryClient.invalidateQueries({ queryKey: formsKeys.all(currentElectionRoundId) });
      navigate({ to: '/forms/$formId/edit', params: { formId: response.data.id } });
    },

    onError: (err) =>
      toast.error('Error creating form from template', {
        description: 'Please contact tech support',
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

export const useCreateFormFromFormDialog = create<PreviewDialogProps>((set) => ({
  isOpen: false,
  id: '',
  languageCode: '',
  trigger: (formId: string, languageCode: string) => set({ id: formId, languageCode, isOpen: true }),
  dismiss: () => set({ isOpen: false }),
}));

type FormReuseDto = {
  formId: string;
  languageCode: string;
};

export const useCreateFormFromForm = () => {
  const { trigger, dismiss, isOpen } = useCreateFormFromFormDialog();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const createFormFromFormMutation = useMutation({
    mutationFn: ({ formId, languageCode }: FormReuseDto) => {
      return API.post<FormFull>(`/election-rounds/${currentElectionRoundId}/forms:fromForm`, {
        formId,
        defaultLanguage: languageCode,
        languages: [languageCode],
        formElectionRoundId: currentElectionRoundId,
      });
    },
    onSuccess: (response) => {
      toast.success('Form created from template');
      queryClient.invalidateQueries({ queryKey: formsKeys.all(currentElectionRoundId) });
      navigate({ to: '/forms/$formId/edit', params: { formId: response.data.id } });
    },

    onError: (err) =>
      toast.error('Error creating form from template', {
        description: 'Please contact tech support',
      }),

    onSettled: () => {
      if (isOpen) dismiss();
    },
  });

  const createForm = (dto: FormReuseDto) => {
    return createFormFromFormMutation.mutate(dto);
  };

  const openReuseFormPreview = (dto: FormReuseDto) => {
    trigger(dto.formId, dto.languageCode);
  };

  return { openReuseFormPreview, createForm };
};
