import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { useCreateGuideMutation } from '@/mutations/guides-observers-mutations'
import { Route } from '@/routes/(app)/elections/$electionRoundId/guides'
import {
  createGuideSchema,
  GuideType,
  type CreateGuideForm,
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

export function CreateGuideDialog() {
  const { open, setOpen, newGuideType } = useGuides()
  const { electionRoundId } = Route.useParams()
  const createGuideMutation = useCreateGuideMutation(electionRoundId)

  // The type is chosen from the upload menu, so the dialog stays closed until
  // one has been picked.
  const isOpen = open === 'create' && newGuideType !== null

  const form = useForm<CreateGuideForm>({
    resolver: zodResolver(createGuideSchema),
    mode: 'all',
    defaultValues: {
      guideType: newGuideType ?? GuideType.Document,
      title: '',
      file: undefined,
      websiteUrl: '',
      text: '',
    },
  })

  // Start from a blank form on every open, otherwise a cancelled attempt would
  // come back with its old values, its errors, and the previously picked type.
  useEffect(() => {
    if (isOpen && newGuideType) {
      form.reset({
        guideType: newGuideType,
        title: '',
        file: undefined,
        websiteUrl: '',
        text: '',
      })
    }
  }, [form, isOpen, newGuideType])

  const onSubmit = (values: CreateGuideForm) => {
    createGuideMutation.mutate(
      {
        title: values.title,
        guideType: values.guideType,
        file: values.file,
        websiteUrl: values.websiteUrl,
        text: values.text,
      },
      {
        onSuccess: () => {
          setOpen(null)
          toast.success('Upload was successful')
        },
        onError: () => {
          toast.error('Error uploading guide', {
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
        // A misclick outside the dialog should not throw away a half filled
        // form, which is how the same dialog behaves in the current admin app.
        onInteractOutside={(event) => event.preventDefault()}
      >
        <DialogHeader>
          <DialogTitle>New guide</DialogTitle>
        </DialogHeader>
        <Separator />

        <Form {...form}>
          <form
            id='create-guide-form'
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
                    <Input placeholder='Title' {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            {newGuideType === GuideType.Document && (
              <FormField
                control={form.control}
                name='file'
                // `value` is left out on purpose: a file input cannot be given
                // one, only read from.
                render={({ field: { name, onBlur, onChange, ref } }) => (
                  <FormItem>
                    <FormLabel>Guide</FormLabel>
                    <FormControl>
                      <Input
                        type='file'
                        name={name}
                        ref={ref}
                        onBlur={onBlur}
                        onChange={(event) => onChange(event.target.files?.[0])}
                        disabled={createGuideMutation.isPending}
                      />
                    </FormControl>
                    <FormDescription>Up to 50 MB.</FormDescription>
                    <FormMessage />
                  </FormItem>
                )}
              />
            )}

            {newGuideType === GuideType.Website && (
              <FormField
                control={form.control}
                name='websiteUrl'
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Guide url</FormLabel>
                    <FormControl>
                      <Input placeholder='https://' {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            )}

            {newGuideType === GuideType.Text && (
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
            disabled={createGuideMutation.isPending}
          >
            Cancel
          </Button>
          <Button
            type='submit'
            form='create-guide-form'
            disabled={createGuideMutation.isPending}
          >
            {createGuideMutation.isPending && <Spinner className='mr-2' />}
            Upload guide
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
