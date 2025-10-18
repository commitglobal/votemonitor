import { buildURLSearchParams } from "@/lib/utils";
import API from "@/services/api";
import type { PageResponse } from "@/types/common";
import type {GuidesObserverModel, GuidesObserversSearch} from "@/types/guides-observer.ts";

export const listGuidesObservers = (
    electionRoundId: string,
    search: GuidesObserversSearch
): Promise<PageResponse<GuidesObserverModel>> => {
    return API.get(`election-rounds/${electionRoundId}/observer-guide`, {
        params: buildURLSearchParams(search),
    }).then((res) => res.data);
};
