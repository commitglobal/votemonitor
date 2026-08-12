import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { useUpdateGuideMutation } from '@/mutations/guides-observers-mutations'
import { Route } from '@/routes/(app)/elections/$electionRoundId/guides'
import {
  GuideType,
  updateGuideSchema,
  type UpdateGuideForm,
} from '@/types/guides-observer'
import { toast } from 'sonner'
import { Button } from '@/components/ui/button'
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form'
import { Input } from '@/components/ui/input'
import { Separator } from '@/components/ui/separator'
import { Spinner } from '@/components/ui/spinner'
import { Textarea } from '@/components/ui/textarea'
import { useGuides } from './GuidesProvider'

export function UpdateGuideDialog() {
  const { open, setOpen, currentRow: guide } = useGuides()
  const { electionRoundId } = Route.useParams()
  const updateGuideMutation = useUpdateGuideMutation(electionRoundId)

  const isOpen = open === 'update' && guide !== null

  const form = useForm<UpdateGuideForm>({
    resolver: zodResolver(updateGuideSchema),
    mode: 'all',
    defaultValues: {
      guideType: guide?.guideType ?? GuideType.Document,
      title: guide?.title ?? '',
      websiteUrl: guide?.websiteUrl ?? '',
      text: guide?.text ?? '',
    },
  })

  // The dialog is mounted once and reused for every row, so the form has to be
  // refilled with the guide it was opened on.
  useEffect(() => {
    if (isOpen && guide) {
      form.reset({
        guideType: guide.guideType,
        title: guide.title,
        websiteUrl: guide.websiteUrl ?? '',
        text: guide.text ?? '',
      })
    }
  }, [form, guide, isOpen])

  if (!guide) {
    return null
  }

  const onSubmit = (values: UpdateGuideForm) => {
    updateGuideMutation.mutate(
      {
        guideId: guide.id,
        guide: {
          title: values.title,
          websiteUrl: values.websiteUrl,
          text: values.text,
        },
      },
      {
        onSuccess: () => {
          setOpen(null)
          toast.success('Update was successful')
        },
        onError: () => {
          toast.error('Error updating guide', {
            description:
              'Please try again or contact support if the problem persists.',
          })
        },
      }
    )
  }

  return (
    <Dialog
      open={isOpen}
      onOpenChange={(nextOpen) => {
        if (!nextOpen) {
          setOpen(null)
        }
      }}
    >
      <DialogContent
        className='sm:max-w-[650px]'
        onInteractOutside={(event) => event.preventDefault()}
      >
        <DialogHeader>
          <DialogTitle>Update guide</DialogTitle>
        </DialogHeader>
        <Separator />

        <Form {...form}>
          <form
            id='update-guide-form'
            onSubmit={form.handleSubmit(onSubmit)}
            className='flex w-full flex-col gap-4'
          >
            <FormField
              control={form.control}
              name='title'
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Title</FormLabel>
                  <FormControl>
                    <Input {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            {/* Documents expose nothing but their title: the API keeps the
                uploaded file as it is and offers no way to replace it. */}
            {guide.guideType === GuideType.Document && (
              <FormDescription>
                Attached file: {guide.fileName}. Upload a new guide to replace
                it.
              </FormDescription>
            )}

            {guide.guideType === GuideType.Website && (
              <FormField
                control={form.control}
                name='websiteUrl'
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Guide url</FormLabel>
                    <FormControl>
                      <Input placeholder='Guide url' {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            )}

            {guide.guideType === GuideType.Text && (
              <FormField
                control={form.control}
                name='text'
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Text</FormLabel>
                    <FormControl>
                      <Textarea className='min-h-48' {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            )}
          </form>
        </Form>

        <DialogFooter>
          <Button
            variant='outline'
            onClick={() => setOpen(null)}
            disabled={updateGuideMutation.isPending}
          >
            Cancel
          </Button>
          <Button
            type='submit'
            form='update-guide-form'
            disabled={updateGuideMutation.isPending}
          >
            {updateGuideMutation.isPending && <Spinner className='mr-2' />}
            Update guide
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
