import { useQuery } from "@tanstack/react-query";
import { noAuthApi } from "./common/no-auth-api";
import { CitizenReportPageResponse, PageResponse } from "./common/types";

export const useCitizenForms = () => {
  const electionRoundId = import.meta.env["VITE_ELECTION_ROUND_ID"];
  return useQuery({
    queryKey: ["citizen-reporting-forms"],
    queryFn: async () => {
      const response = await noAuthApi.get<CitizenReportPageResponse>(
        `/election-rounds/${electionRoundId}/citizen-reporting-forms`,
        {}
      );

      if (response.status !== 200) {
        throw new Error("Failed to fetch forms");
      }

      return response.data;
    },
  });
};
