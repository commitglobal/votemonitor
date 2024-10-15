import API from "../../api";
import { Guide } from "../get-guides.api";

export const getCitizenGuides = async ({
  electionRoundId,
}: {
  electionRoundId: string;
}): Promise<Guide[]> => {
  console.log("😁😁 citizen guides", electionRoundId);
  return API.get(`election-rounds/${electionRoundId}/citizen-guides`).then(
    (res) => res.data.guides,
  );
};
