import { GuideType } from '@/types/guides-observer'
import { FileText, Link2, Paperclip } from 'lucide-react'
import { cn } from '@/lib/utils'

/** One icon per guide type, shared by the table and the upload menu. */
const guideTypeIcons = {
  [GuideType.Document]: Paperclip,
  [GuideType.Website]: Link2,
  [GuideType.Text]: FileText,
}

type GuideTypeIconProps = {
  guideType: GuideType
  className?: string
}

export function GuideTypeIcon({ guideType, className }: GuideTypeIconProps) {
  // Guides created before a new type is added would render nothing, so fall
  // back to the plain text icon.
  const Icon = guideTypeIcons[guideType] ?? FileText

  return <Icon className={cn('h-4 w-4 opacity-50', className)} />
}
