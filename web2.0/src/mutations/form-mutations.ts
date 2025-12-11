import { useMutation } from '@tanstack/react-query'
import { queryClient } from '@/main'
import { formsKeys } from '@/queries/forms'
import { createForm, CreateFormRequest } from '@/services/api/forms/create.api'

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
