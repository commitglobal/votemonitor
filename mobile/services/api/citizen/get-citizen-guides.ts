import API from "../../api";
import { Guide } from "../get-guides.api";

export const getCitizenGuides = async ({
  electionRoundId,
}: {
  electionRoundId: string;
}): Promise<Guide[]> => {
  return API.get(`election-rounds/${electionRoundId}/citizen-guides`).then(
    (res) => res.data.guides,
  );
};
