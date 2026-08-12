import { useMutation } from '@tanstack/react-query'
import { queryClient } from '@/main'
import { guidesObserversKeys } from '@/queries/guides-observers'
import {
  createGuide,
  type CreateGuideRequest,
} from '@/services/api/guides-observers/create.api'
import { deleteGuide } from '@/services/api/guides-observers/delete.api'
import {
  updateGuide,
  type UpdateGuideRequest,
} from '@/services/api/guides-observers/update.api'

/**
 * Every mutation invalidates the whole guides namespace of the election round.
 * The list is the single source of truth for the table, and the create endpoint
 * answers with a partial model while the update one answers with nothing, so
 * refetching is cheaper than patching the cache by hand.
 */

export const useCreateGuideMutation = (electionRoundId: string) =>
  useMutation({
    mutationFn: async (guide: CreateGuideRequest) =>
      await createGuide(electionRoundId, guide),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: guidesObserversKeys.all(electionRoundId),
      })
    },
  })

export const useUpdateGuideMutation = (electionRoundId: string) =>
  useMutation({
    mutationFn: async ({
      guideId,
      guide,
    }: {
      guideId: string
      guide: UpdateGuideRequest
    }) => await updateGuide(electionRoundId, guideId, guide),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: guidesObserversKeys.all(electionRoundId),
      })
    },
  })

export const useDeleteGuideMutation = (electionRoundId: string) =>
  useMutation({
    mutationFn: async (guideId: string) =>
      await deleteGuide(electionRoundId, guideId),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: guidesObserversKeys.all(electionRoundId),
      })
    },
  })
