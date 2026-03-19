import { updateCitizenGuide } from '@/api/election-event/update-citizen-guide';
import { updateObserverGuide } from '@/api/election-event/update-observer-guide';
import { FunctionComponent } from '@/common/types';
import { RichTextEditor } from '@/components/rich-text-editor';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle
} from "@/components/ui/alert-dialog";
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { isNilOrWhitespace, isNotNilOrWhitespace } from '@/lib/utils';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation, useQueryClient, useSuspenseQuery } from '@tanstack/react-query';
import { useBlocker } from '@tanstack/react-router';
import { ReactNode } from 'react';
import { useForm } from 'react-hook-form';
import { toast } from 'sonner';
import { z } from 'zod';
import { citizenGuideDetailsQueryOptions, citizenGuidesKeys } from '../../hooks/citizen-guides-hooks';
import { observerGuideDetailsQueryOptions, observerGuidesKeys } from '../../hooks/observer-guides-hooks';
import { GuidePageType, GuideType } from '../../models/guide';

export interface EditGuideFormProps {
  guidePageType: GuidePageType;
  guideType: GuideType;
  guideId: string;
  onSuccess?: () => void;
  onError?: () => void;
  children: ReactNode;
}

export default function EditGuideForm({
  guidePageType,
  guideType,
  guideId,
  children,
  onError,
  onSuccess,
}: EditGuideFormProps): FunctionComponent {
  const queryClient = useQueryClient();
  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);

  const { data: guide } =
    guidePageType === GuidePageType.Observer
      ? useSuspenseQuery(observerGuideDetailsQueryOptions(currentElectionRoundId, guideId))
      : useSuspenseQuery(citizenGuideDetailsQueryOptions(currentElectionRoundId, guideId));


  const editGuideFormSchema = z
    .object({
      guidePageType: z.nativeEnum(GuidePageType),
      guideType: z.nativeEnum(GuideType),
      title: z.string().min(1, {
        message: 'Title is mandatory',
      }),
      websiteUrl: z
        .string()
        .max(2048, {
          message: 'Url exceeds 2048 chars limit',
        })
        .optional(),
      text: z.string().optional(),
    })
    .superRefine(({ guideType, websiteUrl, text }, ctx) => {
      if (guideType === GuideType.Website) {
        if (isNilOrWhitespace(websiteUrl)) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: `Website url is required`,
            path: ['websiteUrl'],
          });
        }

        if (
          isNotNilOrWhitespace(websiteUrl) &&
          !/^(https?):\/\/(?=.*\.[a-z]{2,})[^\s$.?#].[^\s]*$/i.test(websiteUrl!)
        ) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: `Please enter a valid URL`,
            path: ['websiteUrl'],
          });
        }
      }

      if (guideType === GuideType.Text && isNilOrWhitespace(text)) {
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          message: `Guide text is mandatory`,
          path: ['text'],
        });
      }
    });

  type EditGuideType = z.infer<typeof editGuideFormSchema>;
  const form = useForm<EditGuideType>({
    resolver: zodResolver(editGuideFormSchema),
    mode: 'all',
    defaultValues: {
      guidePageType: guidePageType,
      guideType: guideType,
      title: guide.title,
      text: guide.text ?? '',
      websiteUrl: guide.websiteUrl ?? '',
    },
  });

  const updateTextGuideMutation = useMutation({
    mutationFn: ({
      electionRoundId,
      guide,
      guideId,
    }: {
      electionRoundId: string;
      guide: EditGuideType;
      guideId: string;
    }) => {
      if (guide.guidePageType === GuidePageType.Observer) {
        return updateObserverGuide(electionRoundId, guideId, guide);
      }

      return updateCitizenGuide(electionRoundId, guideId, guide);
    },

    onSuccess: (_, { electionRoundId, guide }) => {
      if (guide.guidePageType === GuidePageType.Observer) {
        queryClient.invalidateQueries({ queryKey: observerGuidesKeys.all(electionRoundId) });
      } else {
        queryClient.invalidateQueries({ queryKey: citizenGuidesKeys.all(electionRoundId) });
      }

      onSuccess?.();

      toast('Update was successful');
    },

    onError: () => {
      onError?.();

      toast.error('Error updating guide', {
        description: 'Please contact tech support',
      });
    },
  });

  const isDirty = form.formState.isDirty;

  const { proceed, reset, status } = useBlocker({
  shouldBlockFn: () => isDirty,
    withResolver: true,
  });


  async function onSubmit(guide: EditGuideType) {
    await updateTextGuideMutation.mutateAsync({ electionRoundId: currentElectionRoundId, guide, guideId });
    form.reset(form.getValues(), { keepValues: true, keepDirty: false, keepIsSubmitSuccessful: false });
  }

  return (
    <>
      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} className='w-full space-y-4'>
          <FormField
            control={form.control}
            name='title'
            render={({ field, fieldState }) => (
              <FormItem className='w-1/2'>
                <FormLabel className='text-left'>
                  Title <span className='text-red-500'>*</span>
                </FormLabel>
                <FormControl>
                  <Input {...field} {...fieldState} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          {guide.guideType === GuideType.Website && (
            <FormField
              control={form.control}
              name='websiteUrl'
              render={({ field, fieldState }) => (
                <FormItem>
                  <FormLabel>
                    Guide url <span className='text-red-500'>*</span>
                  </FormLabel>
                  <FormControl>
                    <Input placeholder='Guide url' {...field} {...fieldState} />
                  </FormControl>
                  <FormMessage className='mt-2' />
                </FormItem>
              )}
            />
          )}

          {guide.guideType === GuideType.Text && (
            <FormField
              control={form.control}
              name='text'
              render={({ field, fieldState }) => (
                <FormItem>
                  <FormLabel>
                    Text <span className='text-red-500'>*</span>
                  </FormLabel>
                  <FormControl>
                    <RichTextEditor {...field} onValueChange={field.onChange} />
                  </FormControl>
                  <FormMessage className='mt-2' />
                </FormItem>
              )}
            />
          )}

          {children}
        </form>
      </Form>

      <AlertDialog open={status === 'blocked'} onOpenChange={(open) => {
        if (!open) reset?.()
      }}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Are you absolutely sure?</AlertDialogTitle>
            <AlertDialogDescription>
              You have unsaved changes. If you leave this page, your changes will be lost. Are you sure you want to continue?
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel onClick={() => reset?.()}>
              Cancel
            </AlertDialogCancel>
            <AlertDialogAction onClick={() => proceed?.()}>
              Continue
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
}
