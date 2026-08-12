import z from 'zod'
import { SortOrder } from './common'

/**
 * Guide flavours supported by the API. A guide holds exactly one kind of
 * content, decided when it is created and immutable afterwards: an uploaded
 * file, an external link or a block of text.
 */
export enum GuideType {
  Document = 'Document',
  Website = 'Website',
  Text = 'Text',
}

export const GuideTypeList: GuideType[] = [
  GuideType.Document,
  GuideType.Website,
  GuideType.Text,
]

/** Labels shown in the table and in the guide type picker. */
export const GuideTypeLabels: Record<GuideType, string> = {
  [GuideType.Document]: 'Document',
  [GuideType.Website]: 'Website',
  [GuideType.Text]: 'Text',
}

/** An NGO of the coalition the guide has been shared with. */
export interface GuideAccessModel {
  ngoId: string
  name: string
}

export interface GuidesObserverModel {
  id: string
  title: string
  guideType: GuideType
  /** Original name of the uploaded file. Only set for `Document` guides. */
  fileName: string
  mimeType: string
  /** HTML sanitized by the backend. Only set for `Text` guides. */
  text: string
  /** Only set for `Website` guides. */
  websiteUrl: string
  /** Last update, falling back to the creation date when never updated. */
  createdOn: string
  /** Display name of the last person who touched the guide. */
  createdBy: string
  /** Short lived download link built by the backend for `Document` guides. */
  presignedUrl: string
  urlValidityInSeconds: number
  filePath: string
  uploadedFileName: string
  /**
   * `false` for guides received through a coalition. Those are read only: the
   * API answers with a 404 when someone who is not the owner tries to update
   * or delete them.
   */
  isGuideOwner: boolean
  guideAccess: GuideAccessModel[]
}

/**
 * Search params kept in the URL.
 *
 * The list endpoint only takes the election round id, so none of these reach
 * the API: they drive the client side filtering, sorting and pagination of the
 * full list the backend returns in one go.
 */
export const guidesObserversSearchSchema = z.object({
  searchText: z.string().optional(),
  guideTypeFilter: z.enum(GuideType).optional(),
  sortColumnName: z.string().optional(),
  sortOrder: z.enum(SortOrder).optional(),
  pageNumber: z.number().default(1),
  pageSize: z.number().default(25),
})

/** Limits enforced by the API validators, mirrored here for instant feedback. */
const GUIDE_TITLE_MAX_LENGTH = 256
const GUIDE_WEBSITE_URL_MAX_LENGTH = 2048
const GUIDE_FILE_MAX_SIZE = 50 * 1024 * 1024 // 50 MB

/**
 * Shape of the create form. The three content fields are optional at this level
 * because only the one matching the selected type is required, which is what
 * the refinement below enforces.
 */
export const createGuideSchema = z
  .object({
    guideType: z.enum(GuideType),
    title: z
      .string()
      .trim()
      .min(1, { message: 'Title is required' })
      .max(GUIDE_TITLE_MAX_LENGTH, {
        message: `Title cannot exceed ${GUIDE_TITLE_MAX_LENGTH} characters`,
      }),
    file: z.instanceof(File).optional(),
    websiteUrl: z
      .string()
      .trim()
      .max(GUIDE_WEBSITE_URL_MAX_LENGTH, {
        message: `Url cannot exceed ${GUIDE_WEBSITE_URL_MAX_LENGTH} characters`,
      })
      .optional(),
    text: z.string().optional(),
  })
  .superRefine(({ guideType, file, websiteUrl, text }, ctx) => {
    if (guideType === GuideType.Document) {
      if (!file) {
        ctx.addIssue({
          code: 'custom',
          message: 'Select a file to upload',
          path: ['file'],
        })
      } else if (file.size > GUIDE_FILE_MAX_SIZE) {
        ctx.addIssue({
          code: 'custom',
          message: 'File cannot be larger than 50 MB',
          path: ['file'],
        })
      }
    }

    if (guideType === GuideType.Website) {
      if (!websiteUrl) {
        ctx.addIssue({
          code: 'custom',
          message: 'Website url is required',
          path: ['websiteUrl'],
        })
      } else if (!URL.canParse(websiteUrl)) {
        // The backend feeds this straight into `new Uri(...)`, which throws on
        // anything that is not an absolute url.
        ctx.addIssue({
          code: 'custom',
          message: 'Enter a valid url, including https://',
          path: ['websiteUrl'],
        })
      }
    }

    if (guideType === GuideType.Text && !text?.trim()) {
      ctx.addIssue({
        code: 'custom',
        message: 'Text is required',
        path: ['text'],
      })
    }
  })

export type CreateGuideForm = z.infer<typeof createGuideSchema>

/**
 * Shape of the edit form. Neither the type nor the uploaded file can be changed
 * after creation, so `Document` guides only expose their title here.
 */
export const updateGuideSchema = z
  .object({
    guideType: z.enum(GuideType),
    title: z
      .string()
      .trim()
      .min(1, { message: 'Title is required' })
      .max(GUIDE_TITLE_MAX_LENGTH, {
        message: `Title cannot exceed ${GUIDE_TITLE_MAX_LENGTH} characters`,
      }),
    websiteUrl: z
      .string()
      .trim()
      .max(GUIDE_WEBSITE_URL_MAX_LENGTH, {
        message: `Url cannot exceed ${GUIDE_WEBSITE_URL_MAX_LENGTH} characters`,
      })
      .optional(),
    text: z.string().optional(),
  })
  .superRefine(({ guideType, websiteUrl, text }, ctx) => {
    if (guideType === GuideType.Website) {
      if (!websiteUrl) {
        ctx.addIssue({
          code: 'custom',
          message: 'Website url is required',
          path: ['websiteUrl'],
        })
      } else if (!URL.canParse(websiteUrl)) {
        ctx.addIssue({
          code: 'custom',
          message: 'Enter a valid url, including https://',
          path: ['websiteUrl'],
        })
      }
    }

    if (guideType === GuideType.Text && !text?.trim()) {
      ctx.addIssue({
        code: 'custom',
        message: 'Text is required',
        path: ['text'],
      })
    }
  })

export type UpdateGuideForm = z.infer<typeof updateGuideSchema>
