import { authApi } from '@/common/auth-api';
import { FunctionComponent } from '@/common/types';
import { useConfirm } from '@/components/ui/alert-dialog-provider';
import { FileUploader } from '@/components/ui/file-uploader';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { toast } from '@/components/ui/use-toast';
import { useCurrentElectionRoundStore } from '@/context/election-round.store';
import { queryClient } from '@/main';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation } from '@tanstack/react-query';
import { useBlocker } from '@tanstack/react-router';
import { ReactNode, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { citizenGuidesKeys } from '../../hooks/citizen-guides-hooks';
import { GuideModel, GuidePageType, GuideType } from '../../models/guide';
import { isNilOrWhitespace, isNotNilOrWhitespace } from '@/lib/utils';
import { observerGuidesKeys } from '../../hooks/observer-guides-hooks';
export interface AddGuideFormProps {
  guidePageType: GuidePageType;
  guideType: GuideType;
  onSuccess?: (guide: GuideModel) => void;
  onError?: () => void;
  children: ReactNode;
}
export default function AddGuideForm({
  guidePageType,
  guideType,
  children,
  onSuccess,
  onError,
}: AddGuideFormProps): FunctionComponent {
  const confirm = useConfirm();

  const currentElectionRoundId = useCurrentElectionRoundStore((s) => s.currentElectionRoundId);

  const newGuideFormSchema = z
    .object({
      guideType: z.nativeEnum(GuideType),
      title: z.string().min(1, {
        message: 'Title is mandatory',
      }),
      file: z.optional(z.array(z.instanceof(File))),
      websiteUrl: z
        .string()
        .max(2048, {
          message: 'Url exceeds 2048 chars limit',
        })
        .optional(),
      text: z.string().optional(),
    })
    .superRefine(({ guideType, file, websiteUrl, text }, ctx) => {
      if (guideType === GuideType.Document && (!file || file.length === 0)) {
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          message: `Select a file to upload`,
          path: ['file'],
        });
      }

      if (guideType === GuideType.Website) {
        if (isNilOrWhitespace(websiteUrl)) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: `Website url is required`,
            path: ['websiteUrl'],
          });
        }

        if (isNotNilOrWhitespace(websiteUrl) && /^(https?):\/\/(?=.*\.[a-z]{2,})[^\s$.?#].[^\s]*$/i.test(websiteUrl!)) {
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

  type NewGuideType = z.infer<typeof newGuideFormSchema>;

  const form = useForm<NewGuideType>({
    resolver: zodResolver(newGuideFormSchema),
    defaultValues: {
      guideType: guideType,
      title: '',
      websiteUrl: '',
      text: '',
      file: [],
    },
  });

  const uploadGuideMutation = useMutation({
    mutationFn: ({
      electionRoundId,
      guide,
      guidePageType,
    }: {
      electionRoundId: string;
      guide: NewGuideType;
      guidePageType: GuidePageType;
    }) => {
      const formData = new FormData();
      formData.append('Title', guide.title);
      formData.append('GuideType', guide.guideType);

      if (guide.guideType === GuideType.Website) {
        formData.append('WebsiteUrl', guide.websiteUrl!);
      }
      if (guide.guideType === GuideType.Document) {
        formData.append('Attachment', guide.file?.[0]!);
      }

      if (guide.guideType === GuideType.Text) {
        formData.append('Text', guide.text!);
      }

      const url =
        guidePageType === GuidePageType.Observer
          ? `/election-rounds/${electionRoundId}/observer-guide`
          : `/election-rounds/${electionRoundId}/citizen-guides`;

      return authApi.post(url, formData, {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      });
    },

    onSuccess: ({ data }, { electionRoundId, guidePageType }) => {
      if (guidePageType === GuidePageType.Observer) {
        queryClient.invalidateQueries({ queryKey: observerGuidesKeys.all(electionRoundId) });
      } else {
        queryClient.invalidateQueries({ queryKey: citizenGuidesKeys.all(electionRoundId) });
      }

      onSuccess?.(data);

      toast({
        title: 'Success',
        description: 'Upload was successful',
      });
    },

    onError: () => {
      toast({
        title: 'Error uploading citizen guide',
        description: 'Please contact Platform admins',
        variant: 'destructive',
      });

      onError?.();
    },
  });

  function onSubmit(guide: NewGuideType): void {
    uploadGuideMutation.mutate({ electionRoundId: currentElectionRoundId, guidePageType, guide });
  }

  useBlocker(
    () =>
      confirm({
        title: `Unsaved Changes Detected`,
        body: 'You have unsaved changes. If you leave this page, your changes will be lost. Are you sure you want to continue?',
        actionButton: 'Leave',
        cancelButton: 'Stay',
      }),
    form.formState.isDirty
  );

  useEffect(() => {
    if (form.formState.isSubmitSuccessful) {
      form.reset({}, { keepValues: true });
    }
  }, [form.formState.isSubmitSuccessful, form.reset]);

  return (
    <>
      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} className='w-full space-y-4'>
          <FormField
            control={form.control}
            name='title'
            render={({ field, fieldState  }) => (
              <FormItem className='w-1/2'>
                <FormLabel>
                  Title <span className='text-red-500'>*</span>
                </FormLabel>
                <FormControl>
                  <Input placeholder='Title' {...field} {...fieldState} />
                </FormControl>
                <FormMessage className='mt-2' />
              </FormItem>
            )}
          />
          {guideType === GuideType.Website && (
            <FormField
              control={form.control}
              name='websiteUrl'
              render={({ field, fieldState }) => (
                <FormItem>
                  <FormLabel>
                    Guide url <span className='text-red-500'>*</span>
                  </FormLabel>
                  <FormControl>
                    <Input placeholder='https://' {...field}  {...fieldState}/>
                  </FormControl>
                  <FormMessage className='mt-2' />
                </FormItem>
              )}
            />
          )}
          {guideType === GuideType.Document && (
            <FormField
              control={form.control}
              name='file'
              render={({ field, fieldState }) => (
                <div className='space-y-6'>
                  <FormItem className='w-full'>
                    <FormLabel>
                      Guide <span className='text-red-500'>*</span>
                    </FormLabel>
                    <FormControl>
                      <FileUploader
                        value={field.value}
                        onValueChange={field.onChange}
                        maxFileCount={1}
                        maxSize={50 * 1024 * 1024}
                        accept={{ '*/*': [] }}
                        disabled={uploadGuideMutation.isPending}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                </div>
              )}
            />
          )}
          {guideType === GuideType.Text && (
            <FormField
              control={form.control}
              name='text'
              render={({ field, fieldState }) => (
                <FormItem>
                  <FormLabel className='text-left'>
                    Text <span className='text-red-500'>*</span>
                  </FormLabel>
                  <FormControl>
                    <Textarea {...field} {...fieldState} rows={10} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
          )}
          {children}
        </form>
      </Form>
    </>
  );
}
