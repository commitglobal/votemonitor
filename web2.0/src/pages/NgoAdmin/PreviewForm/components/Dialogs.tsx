import { useState, useEffect } from 'react'
import { ConfirmDialog } from '@/components/ConfirmDialog'
import { getTranslatedStringOrDefault } from '@/lib/translated-string'
import {
  useDeleteFormMutation,
  useDuplicateFormMutation,
  useObsoleteFormMutation,
  usePublishFormMutation,
  useUpdateFormLanguagesMutation
} from '@/mutations/form-mutations'
import { Route } from '@/routes/(app)/elections/$electionRoundId/forms/$formId'
import { toast } from 'sonner'
import { usePreviewForm } from './PreviewFormProvider'
import { MultiSelect } from '@/components/ui/multi-select'
import { LanguageList, Language } from '@/types/language'
import { mapLanguageNameByCode } from '@/lib/i18n'
import { useGetFormDetails } from '@/queries/forms'
import { router } from '@/main'

export function PreviewFormDialogs() {
  const { open, setOpen } = usePreviewForm()
  const { electionRoundId, formId } = Route.useParams()
  const { formLanguage } = Route.useSearch()
  const navigate = Route.useNavigate()

  const { data: form } = useGetFormDetails(electionRoundId, formId)

  const publishMutation = usePublishFormMutation(electionRoundId)
  const obsoleteMutation = useObsoleteFormMutation(electionRoundId)
  const deleteMutation = useDeleteFormMutation(electionRoundId)
  const duplicateMutation = useDuplicateFormMutation(electionRoundId)
  const updateLanguagesMutation = useUpdateFormLanguagesMutation(electionRoundId)

  const [selectedLanguages, setSelectedLanguages] = useState<string[]>([])

  // Reset selected languages when dialog closes
  useEffect(() => {
    if (open !== 'addTranslations') {
      setSelectedLanguages([])
    }
  }, [open])

  if (!form) {
    return null
  }

  const formName = getTranslatedStringOrDefault(
    form.name,
    form.defaultLanguage,
    formLanguage as Language
  )

  const handlePublish = () => {
    publishMutation.mutate(form.id, {
      onSuccess: () => {
        setOpen(null)
      },
      onError: () => {
        toast.error('Failed to publish form', {
          description: 'Please try again or contact support if the problem persists.',
        })
      },
    })
  }

  const handleArchive = () => {
    obsoleteMutation.mutate(form.id, {
      onSuccess: () => {
        setOpen(null)
      },
      onError: () => {
        toast.error('Failed to archive form', {
          description: 'Please try again or contact support if the problem persists.',
        })
      },
    })
  }

  const handleDelete = () => {
    deleteMutation.mutate(form.id, {
      onSuccess: () => {
        setOpen(null)
        // Navigate back to forms list after deletion
        navigate({
          to: '/elections/$electionRoundId/forms',
          params: { electionRoundId },
        })
      },
      onError: () => {
        toast.error('Failed to delete form', {
          description: 'Please try again or contact support if the problem persists.',
        })
      },
    })
  }

  const handleDuplicate = () => {
    duplicateMutation.mutate(form.id, {
      onSuccess: (duplicatedForm) => {
        setOpen(null)
        // Navigate to the duplicated form
        navigate({
          to: '/elections/$electionRoundId/forms/$formId',
          params: { electionRoundId, formId: duplicatedForm.id },
        })
      },
      onError: () => {
        toast.error('Failed to duplicate form', {
          description: 'Please try again or contact support if the problem persists.',
        })
      },
    })
  }

  const handleAddTranslations = () => {
    if (!form || selectedLanguages.length === 0) {
      return
    }

    // Merge existing languages with selected new languages and remove duplicates
    const updatedLanguages = [
      ...form.languages,
      ...selectedLanguages,
    ].filter((lang, index, self) => self.indexOf(lang) === index) as Language[]

    updateLanguagesMutation.mutate(
      {
        formId: form.id,
        languages: updatedLanguages,
      },
      {
        onSuccess: () => {
          setOpen(null)
          setSelectedLanguages([])
          toast.success('Languages added successfully', {
            description: 'The selected languages have been added to the form.',
          })
          router.invalidate()
        },
        onError: () => {
          toast.error('Failed to add languages', {
            description: 'Please try again or contact support if the problem persists.',
          })
        },
      }
    )
  }

  const handleDeleteTranslation = () => {
    if (!form || !formLanguage) {
      return
    }

    // Filter out the language to be deleted
    const updatedLanguages = form.languages.filter(
      (lang) => lang !== formLanguage
    ) as Language[]

    updateLanguagesMutation.mutate(
      {
        formId: form.id,
        languages: updatedLanguages,
      },
      {
        onSuccess: () => {
          setOpen(null)
          toast.success('Translation deleted successfully', {
            description: `The ${mapLanguageNameByCode(formLanguage as Language)} translation has been removed from the form.`,
          })
          router.invalidate()
          // Navigate to remove formLanguage from URL, which will default to defaultLanguage
          navigate({
            to: '.',
            search: (prev) => {
              const { formLanguage: _, ...rest } = prev
              return rest
            },
          })
        },
        onError: () => {
          toast.error('Failed to delete translation', {
            description: 'Please try again or contact support if the problem persists.',
          })
        },
      }
    )
  }

  // Calculate grouped languages: current (disabled) and available (enabled)
  const groupedOptions =
    form && open === 'addTranslations'
      ? [
          {
            heading: 'Current Languages',
            options: form.languages.map((lang) => ({
              value: lang,
              label: mapLanguageNameByCode(lang),
              disabled: true,
            })),
          },
          {
            heading: 'Available Languages',
            options: LanguageList.filter(
              (lang) => !form.languages.includes(lang)
            ).map((lang) => ({
              value: lang,
              label: mapLanguageNameByCode(lang),
            })),
          },
        ]
      : []

  return (
    <>
      <ConfirmDialog
        open={open === 'publish'}
        onOpenChange={(isOpen) => {
          if (!isOpen) {
            setOpen(null)
          }
        }}
        handleConfirm={handlePublish}
        isLoading={publishMutation.isPending}
        className='max-w-md'
        title='Publish Form'
        desc={`Are you sure you want to publish "${formName}"? Once published, the form cannot be edited.`}
        confirmText='Publish'
      />

      <ConfirmDialog
        open={open === 'obsolete'}
        onOpenChange={(isOpen) => {
          if (!isOpen) {
            setOpen(null)
          }
        }}
        handleConfirm={handleArchive}
        isLoading={obsoleteMutation.isPending}
        className='max-w-md'
        title='Archive Form'
        desc={`Are you sure you want to archive "${formName}"? The form will be marked as obsolete and cannot be edited.`}
        confirmText='Archive'
      />

      <ConfirmDialog
        open={open === 'duplicate'}
        onOpenChange={(isOpen) => {
          if (!isOpen) {
            setOpen(null)
          }
        }}
        handleConfirm={handleDuplicate}
        isLoading={duplicateMutation.isPending}
        className='max-w-md'
        title='Duplicate Form'
        desc={`Are you sure you want to duplicate "${formName}"? A copy of this form will be created.`}
        confirmText='Duplicate'
      />

      <ConfirmDialog
        destructive
        open={open === 'delete'}
        onOpenChange={(isOpen) => {
          if (!isOpen) {
            setOpen(null)
          }
        }}
        handleConfirm={handleDelete}
        isLoading={deleteMutation.isPending}
        className='max-w-md'
        title={`Delete Form`}
        desc={`You are about to delete "${formName}". This action cannot be undone.`}
        confirmText='Delete'
      />

      <ConfirmDialog
        open={open === 'addTranslations'}
        onOpenChange={(isOpen) => {
          if (!isOpen) {
            setOpen(null)
            setSelectedLanguages([])
          }
        }}
        handleConfirm={handleAddTranslations}
        isLoading={updateLanguagesMutation.isPending}
        disabled={selectedLanguages.length === 0}
        className='max-w-md'
        title='Add Translations'
        desc={`Select languages to add to "${formName}". The selected languages will be added to the form.`}
        confirmText='Add Languages'
      >
        <div className='mt-4'>
          <MultiSelect
            options={groupedOptions}
            onValueChange={setSelectedLanguages}
            defaultValue={form.languages}
            placeholder='Select languages to add'
            searchable={true}
            hideSelectAll={true}
            animation={0}
            maxCount={100}
            animationConfig={{
              badgeAnimation: "none",  
              popoverAnimation: "none",  
              optionHoverAnimation: "none",  
              duration: 0,
              delay: 0,
            }}
          />
        </div>
      </ConfirmDialog>

      <ConfirmDialog
        destructive
        open={open === 'deleteTranslation'}
        onOpenChange={(isOpen) => {
          if (!isOpen) {
            setOpen(null)
          }
        }}
        handleConfirm={handleDeleteTranslation}
        isLoading={updateLanguagesMutation.isPending}
        className='max-w-md'
        title='Delete Translation'
        desc={
          formLanguage
            ? `Are you sure you want to delete the ${mapLanguageNameByCode(formLanguage as Language)} translation from "${formName}"? This action cannot be undone.`
            : `Are you sure you want to delete this translation from "${formName}"? This action cannot be undone.`
        }
        confirmText='Delete Translation'
      />
    </>
  )
}
