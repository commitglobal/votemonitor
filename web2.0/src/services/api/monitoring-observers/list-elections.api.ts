import { buildURLSearchParams } from "@/lib/utils";
import API from "@/services/api";
import type { PageResponse } from "@/types/common";
import type { ElectionModel, ElectionsSearch } from "@/types/elections";

export const listElections = (
  search: ElectionsSearch
): Promise<PageResponse<ElectionModel>> => {
  return API.get(`/election-rounds`, {
    params: buildURLSearchParams(search),
  }).then((res) => res.data);
};
