import { z } from "zod";
import { SortOrder } from "./common";

export interface GuidesObserverModel {
    id: string;
    title: string;
    fileName: string;
    mimeType: string;
    guideType: string;
    text: string;
    websiteUrl: string;
    createdOn: string;
    createdBy: string;
    presignedUrl: string;
    urlValidityInSeconds: string;
    filePath: string;
    uploadedFileName: string;
    isGuideOwner: boolean;
}

export const guidesObserversSearchSchema = z.object({
    title: z.string().optional(),
    fileName: z.string().optional(),
    mimeType: z.string().optional(),
    guideType: z.string().optional(),
    searchText: z.string().optional(),
    sortColumnName: z.string().optional(),
    sortOrder: z.enum(SortOrder).optional(),
    pageNumber: z.number().default(1),
    pageSize: z.number().default(25),
});

export type GuidesObserversSearch = z.infer<
    typeof guidesObserversSearchSchema
>;
