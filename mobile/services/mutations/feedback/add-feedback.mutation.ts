import { useMutation } from "@tanstack/react-query";
import { feedbackKeys } from "../../queries.service";
import { addFeedback, AddFeedbackPayload } from "../../definitions.api";

export const useAddFeedbackMutation = (electionRoundId: string | undefined) => {
  return useMutation({
    mutationKey: feedbackKeys.addFeedback(electionRoundId),
    mutationFn: async (payload: AddFeedbackPayload) => {
      return addFeedback(payload);
    },

    onError: (err) => {
      console.log("🛑🛑🛑 ERROR IN ADD FEEDBACK MUTATION 🛑🛑🛑", err);
    },
  });
};