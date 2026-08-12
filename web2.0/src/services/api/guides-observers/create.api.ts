import API from '@/services/api'
import { GuideType, type GuidesObserverModel } from '@/types/guides-observer'

export type CreateGuideRequest = {
  title: string
  guideType: GuideType
  /** Required for `Document` guides, ignored for the other types. */
  file?: File
  /** Required for `Website` guides, ignored for the other types. */
  websiteUrl?: string
  /** Required for `Text` guides, ignored for the other types. */
  text?: string
}

/**
 * Creates an observer guide.
 *
 * The endpoint accepts file uploads, so the payload always goes out as
 * multipart even for the two types that carry no attachment. Only the field
 * matching the selected type is appended: sending an empty `websiteUrl` for a
 * document would fail the `new Uri(...)` conversion on the backend.
 */
export const createGuide = (
  electionRoundId: string,
  guide: CreateGuideRequest
): Promise<GuidesObserverModel> => {
  const formData = new FormData()
  formData.append('Title', guide.title)
  formData.append('GuideType', guide.guideType)

  if (guide.guideType === GuideType.Document && guide.file) {
    formData.append('Attachment', guide.file)
  }

  if (guide.guideType === GuideType.Website && guide.websiteUrl) {
    formData.append('WebsiteUrl', guide.websiteUrl)
  }

  if (guide.guideType === GuideType.Text && guide.text) {
    formData.append('Text', guide.text)
  }

  return API.post<GuidesObserverModel>(
    `election-rounds/${electionRoundId}/observer-guide`,
    formData
  ).then((res) => res.data)
}
