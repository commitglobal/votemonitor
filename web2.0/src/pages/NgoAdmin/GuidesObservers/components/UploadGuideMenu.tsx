import { GuideType, GuideTypeList } from '@/types/guides-observer'
import { ChevronDown, Upload } from 'lucide-react'
import { Button } from '@/components/ui/button'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import { GuideTypeIcon } from './GuideTypeIcon'
import { useGuides } from './GuidesProvider'

/** Wording of each entry of the upload menu. */
const uploadMenuLabels: Record<GuideType, string> = {
  [GuideType.Document]: 'Document guide',
  [GuideType.Website]: 'Url guide',
  [GuideType.Text]: 'Text guide',
}

type UploadGuideMenuProps = {
  disabled?: boolean
}

/**
 * Entry point for creating a guide. The type is picked here rather than inside
 * the dialog because it cannot be changed later and it decides which fields the
 * form has to show.
 */
export function UploadGuideMenu({ disabled }: UploadGuideMenuProps) {
  const { setOpen, setNewGuideType } = useGuides()

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild disabled={disabled}>
        <Button>
          <Upload className='mr-1.5 h-4 w-4' />
          Upload observer guide
          <ChevronDown className='h-4 w-4 opacity-50' />
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align='end' className='w-56'>
        {GuideTypeList.map((guideType) => (
          <DropdownMenuItem
            key={guideType}
            className='flex flex-row gap-2'
            onClick={() => {
              setNewGuideType(guideType)
              setOpen('create')
            }}
          >
            <GuideTypeIcon guideType={guideType} />
            {uploadMenuLabels[guideType]}
          </DropdownMenuItem>
        ))}
      </DropdownMenuContent>
    </DropdownMenu>
  )
}
