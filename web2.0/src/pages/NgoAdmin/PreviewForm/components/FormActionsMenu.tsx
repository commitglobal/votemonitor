import { useState, useEffect } from 'react'
import {
  Archive,
  Copy,
  Languages,
  MoreVertical,
  Plus,
  Send,
  Trash2,
} from 'lucide-react'
import { FormStatus } from '@/types/form'
import { Button } from '@/components/ui/button'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import { useSuspenseGetFormDetails } from '@/queries/forms'
import { usePreviewForm } from './PreviewFormProvider'
import { Route } from '@/routes/(app)/elections/$electionRoundId/forms/$formId'
import { Language } from '@/types/language'

export function FormActionsMenu() {
  const { open, setOpen } = usePreviewForm()
  const [dropdownOpen, setDropdownOpen] = useState(false)
  const { electionRoundId, formId } = Route.useParams()
  const { formLanguage } = Route.useSearch()
  const { data: form } = useSuspenseGetFormDetails(electionRoundId, formId)

  // Reset dropdown when dialog closes
  useEffect(() => {
    if (open === null) {
      setDropdownOpen(false)
    }
  }, [open])

  const shouldShowDeleteTranslation =
    formLanguage &&
    form.defaultLanguage !== formLanguage &&
    form.languages.includes(formLanguage as Language)

  return (
    <DropdownMenu open={dropdownOpen} onOpenChange={setDropdownOpen}>
      <DropdownMenuTrigger asChild>
        <Button variant='outline' size='icon'>
          <MoreVertical className='h-5 w-5' />
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align='end' className='w-48'>
        {/* Add Languages - only for Draft forms */}
        {form.status === FormStatus.Drafted && (
          <DropdownMenuItem
            onClick={() => {
              setDropdownOpen(false)
              setOpen('addTranslations')
            }}
            className='gap-2'
          >
            <Plus className='h-4 w-4' />
            Add translations
          </DropdownMenuItem>
        )}

        {/* Delete Translation - when viewing non-default language */}
        {shouldShowDeleteTranslation && (
          <DropdownMenuItem
            onClick={() => {
              setDropdownOpen(false)
              setOpen('deleteTranslation')
            }}
            className='gap-2 text-destructive focus:text-destructive'
          >
            <Languages className='h-4 w-4' />
            Delete translation
          </DropdownMenuItem>
        )}

        {/* Publish - only for Draft forms */}
        {form.status === FormStatus.Drafted && (
          <DropdownMenuItem
            onClick={() => {
              setDropdownOpen(false)
              setOpen('publish')
            }}
            className='gap-2'
          >
            <Send className='h-4 w-4' />
            Publish
          </DropdownMenuItem>
        )}

        {/* Archive - only for Published forms */}
        {form.status === FormStatus.Published && (
          <DropdownMenuItem
            onClick={() => {
              setDropdownOpen(false)
              setOpen('obsolete')
            }}
            className='gap-2'
          >
            <Archive className='h-4 w-4' />
            Archive
          </DropdownMenuItem>
        )}

        {/* Duplicate - available for all forms */}
        <DropdownMenuItem
          onClick={() => {
            setDropdownOpen(false)
            setOpen('duplicate')
          }}
          className='gap-2'
        >
          <Copy className='h-4 w-4' />
          Duplicate
        </DropdownMenuItem>

        {/* Delete - for all forms, but only action for Obsolete */}
        <DropdownMenuSeparator />
        <DropdownMenuItem
          onClick={() => {
            setDropdownOpen(false)
            setOpen('delete')
          }}
          className='text-destructive focus:text-destructive gap-2'
        >
          <Trash2 className='h-4 w-4' />
          Delete
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  )
}

