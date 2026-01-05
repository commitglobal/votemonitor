import { useMutation } from '@tanstack/react-query'
import { queryClient } from '@/main'
import { formsKeys } from '@/queries/forms'
import { createForm, CreateFormRequest } from '@/services/api/forms/create.api'
import { publishForm } from '@/services/api/forms/publish.api'
import { obsoleteForm } from '@/services/api/forms/obsolete.api'
import { deleteForm } from '@/services/api/forms/delete.api'
import { duplicateForm } from '@/services/api/forms/duplicate.api'
import { updateFormLanguages } from '@/services/api/forms/update-languages.api'
import { Language } from '@/types/language'

export const useCreateFormMutation = (electionRoundId: string) =>
  useMutation({
    mutationFn: async (form: CreateFormRequest) =>
      await createForm(electionRoundId, form),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: formsKeys.all(electionRoundId),
      })
    },
  })

export const usePublishFormMutation = (electionRoundId: string) =>
  useMutation({
    mutationFn: async (formId: string) =>
      await publishForm(electionRoundId, formId),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: formsKeys.all(electionRoundId),
      })
    },
  })

export const useObsoleteFormMutation = (electionRoundId: string) =>
  useMutation({
    mutationFn: async (formId: string) =>
      await obsoleteForm(electionRoundId, formId),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: formsKeys.all(electionRoundId),
      })
    },
  })

export const useDeleteFormMutation = (electionRoundId: string) =>
  useMutation({
    mutationFn: async (formId: string) =>
      await deleteForm(electionRoundId, formId),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: formsKeys.all(electionRoundId),
      })
    },
  })

export const useDuplicateFormMutation = (electionRoundId: string) =>
  useMutation({
    mutationFn: async (formId: string) =>
      await duplicateForm(electionRoundId, formId),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: formsKeys.all(electionRoundId),
      })
    },
  })

export const useUpdateFormLanguagesMutation = (electionRoundId: string) =>
  useMutation({
    mutationFn: async ({
      formId,
      languages,
    }: {
      formId: string
      languages: Language[]
    }) => await updateFormLanguages(electionRoundId, formId, languages),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: formsKeys.all(electionRoundId),
      })
    },
  })
