import { getForms } from "@/api/get-forms";
import { getLocations } from "@/api/get-locations";
import { queryOptions, useQuery } from "@tanstack/react-query";

export const locationsOptions = () => {
  return queryOptions({
    queryKey: ["locations"],
    placeholderData: {},
    queryFn: () => getLocations(),
    staleTime: Infinity,
  });
};

export const useLocations = () => {
  return useQuery(locationsOptions());
};
