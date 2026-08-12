import type { GuidesObserverModel } from '@/types/guides-observer'
import { Ellipsis } from 'lucide-react'
import { Button } from '@/components/ui/button'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import { useGuides } from './GuidesProvider'

type GuideRowActionsProps = {
  guide: GuidesObserverModel
}

export function GuideRowActions({ guide }: GuideRowActionsProps) {
  const { setOpen, setCurrentRow } = useGuides()

  // Guides shared by another NGO of the coalition are read only: the API only
  // resolves them for their owner and answers 404 to anybody else.
  const isReadOnly = !guide.isGuideOwner

  return (
    <DropdownMenu modal={false}>
      <DropdownMenuTrigger asChild>
        <Button
          variant='ghost'
          className='data-[state=open]:bg-muted flex h-8 w-8 p-0'
        >
          <Ellipsis className='h-4 w-4' />
          <span className='sr-only'>Open menu</span>
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align='end' className='w-[160px]'>
        <DropdownMenuItem
          disabled={isReadOnly}
          onClick={() => {
            setCurrentRow(guide)
            setOpen('update')
          }}
        >
          Update
        </DropdownMenuItem>
        <DropdownMenuItem
          variant='destructive'
          disabled={isReadOnly}
          onClick={() => {
            setCurrentRow(guide)
            setOpen('delete')
          }}
        >
          Delete guide
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  )
}
