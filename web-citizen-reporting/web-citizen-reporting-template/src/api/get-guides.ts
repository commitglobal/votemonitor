import type { GuideModel } from "@/common/types";
import { API } from "./api";
import { electionRoundId } from "@/lib/utils";

export const getGuides = async (): Promise<GuideModel[]> => {
  return API.get(`/api/election-rounds/${electionRoundId}/citizen-guides`).then(
    (res) => res.data.guides
  );
};
