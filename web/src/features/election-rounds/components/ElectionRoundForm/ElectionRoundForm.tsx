import { DateOnlyFormat } from '@/common/formats';
import { Button } from '@/components/ui/button';
import { Calendar } from '@/components/ui/calendar';
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from '@/components/ui/command';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { useCountries } from '@/hooks/countries';
import { cn } from '@/lib/utils';
import { CalendarIcon, CheckIcon, ChevronDownIcon } from '@heroicons/react/24/outline';
import { zodResolver } from '@hookform/resolvers/zod';
import { format } from 'date-fns/format';
import { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { ElectionRoundModel } from '../../models/types';
import { parse } from 'date-fns';

const electionRoundSchema = z.object({
  countryId: z.string().min(1, {
    message: 'Country is mandatory',
  }),
  title: z
    .string()
    .min(1, {
      message: 'Title is mandatory',
    })
    .max(256, {
      message: 'Title should not exceed 256 characters',
    }),
  englishTitle: z
    .string()
    .min(1, {
      message: 'English title is mandatory',
    })
    .max(256, {
      message: 'English title not exceed 256 characters',
    }),
  startDate: z.date(),
});

export type ElectionRoundRequest = z.infer<typeof electionRoundSchema>;
export interface ElectionRoundFormProps {
  children: React.ReactNode;
  onSubmit: (electionRound: ElectionRoundRequest) => void;
  electionRound?: ElectionRoundModel;
}
function ElectionRoundForm({ electionRound, children, onSubmit }: ElectionRoundFormProps) {
  const [countrySelectOpen, setCountrySelectOpen] = useState(false);
  const [startDateSelectOpen, setStartDateSelectOpen] = useState(false);
  const { data: countries } = useCountries();

  const form = useForm<ElectionRoundRequest>({
    resolver: zodResolver(electionRoundSchema),
    defaultValues: {
      countryId: electionRound?.countryId ?? '',
      title: electionRound?.title ?? '',
      englishTitle: electionRound?.englishTitle ?? '',
      startDate: electionRound?.startDate ? parse(electionRound?.startDate, DateOnlyFormat, new Date()) : new Date(),
    },
  });

  // const createElectionRoundMutation = useMutation({
  //   mutationFn: (electionRound: ElectionRoundRequest) => {
  //     return authApi.post<ElectionRoundModel>(`/election-rounds`, {
  //       ...electionRound,
  //       startDate: format(electionRound.startDate, DateOnlyFormat),
  //     });
  //   },

  //   onSuccess: async ({ data }) => {
  //     await queryClient.invalidateQueries({ queryKey: electionRoundKeys.lists() });
  //     router.invalidate();
  //     router.navigate({
  //       to: '/election-rounds/$electionRoundId',
  //       params: { electionRoundId: data.id },
  //     });

  //     toast({
  //       title: 'Success',
  //       description: 'Election round created',
  //     });
  //   },

  //   onError: () => {
  //     toast({
  //       title: 'Error creating election round',
  //       description: 'Please contact Platform admins',
  //       variant: 'destructive',
  //     });
  //   },
  // });

  useEffect(() => {
    if (form.formState.isSubmitSuccessful) {
      form.reset({}, { keepValues: true });
    }
  }, [form.formState.isSubmitSuccessful, form.reset]);

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className='w-full space-y-4'>
        <FormField
          control={form.control}
          name='title'
          render={({ field, fieldState }) => (
            <FormItem>
              <FormLabel>
                Election event title <span className='text-red-500'>*</span>
              </FormLabel>
              <FormControl>
                <Input placeholder='Title' {...field} {...fieldState} />
              </FormControl>
              <FormMessage className='mt-2' />
            </FormItem>
          )}
        />

        <FormField
          control={form.control}
          name='englishTitle'
          render={({ field, fieldState }) => (
            <FormItem>
              <FormLabel>
                Election event English title <span className='text-red-500'>*</span>
              </FormLabel>
              <FormControl>
                <Input placeholder='English title' {...field} {...fieldState} />
              </FormControl>
              <FormMessage className='mt-2' />
            </FormItem>
          )}
        />

        <FormField
          control={form.control}
          name='countryId'
          render={({ field }) => (
            <FormItem className='flex flex-col space-y-1'>
              <FormLabel>
                Country <span className='text-red-500'>*</span>
              </FormLabel>
              <Popover open={countrySelectOpen} onOpenChange={setCountrySelectOpen}>
                <PopoverTrigger asChild>
                  <FormControl>
                    <Button
                      variant='outline'
                      role='combobox'
                      className={cn('w-full pl-3 justify-between', !field.value && 'text-muted-foreground')}>
                      {field.value
                        ? countries?.find((country) => country.id === field.value)?.fullName
                        : 'Select country'}
                      <ChevronDownIcon className='w-4 h-4 ml-auto opacity-50' />
                    </Button>
                  </FormControl>
                </PopoverTrigger>
                <PopoverContent className='w-[450px] p-0'>
                  <Command
                    filter={(value, search, keywords) => {
                      const extendValue = keywords?.join(' ') ?? '';
                      if (extendValue.toLocaleLowerCase().includes(search.toLocaleLowerCase())) return 1;
                      return 0;
                    }}>
                    <CommandInput placeholder='Search country...' className='h-9' />
                    <CommandList>
                      <CommandEmpty>No country found.</CommandEmpty>
                      <CommandGroup>
                        {countries?.map((country) => (
                          <CommandItem
                            value={country.id}
                            key={country.id}
                            keywords={[country.name, country.fullName]}
                            onSelect={() => {
                              form.setValue('countryId', country.id);
                              setCountrySelectOpen(false);
                            }}>
                            {country.fullName}
                            <CheckIcon
                              className={cn(
                                'ml-auto h-4 w-4',
                                country.id === field.value ? 'opacity-100' : 'opacity-0'
                              )}
                            />
                          </CommandItem>
                        ))}
                      </CommandGroup>
                    </CommandList>
                  </Command>
                </PopoverContent>
              </Popover>
              <FormMessage />
            </FormItem>
          )}
        />

        <FormField
          control={form.control}
          name='startDate'
          render={({ field }) => (
            <FormItem className='flex flex-col space-y-1'>
              <FormLabel>
                Election start date <span className='text-red-500'>*</span>
              </FormLabel>
              <Popover open={startDateSelectOpen} onOpenChange={setStartDateSelectOpen}>
                <PopoverTrigger asChild>
                  <FormControl>
                    <Button
                      variant='outline'
                      className={cn('w-[200px] pl-3 text-left font-normal', !field.value && 'text-muted-foreground')}>
                      {field.value ? format(field.value, DateOnlyFormat) : <span>Select start date</span>}
                      <CalendarIcon className='w-4 h-4 ml-auto opacity-50' />
                    </Button>
                  </FormControl>
                </PopoverTrigger>
                <PopoverContent className='w-auto p-0' align='start'>
                  <Calendar
                    mode='single'
                    selected={field.value}
                    onSelect={(selected) => {
                      field.onChange(selected);
                      setStartDateSelectOpen(false);
                    }}
                    disabled={(date) => date < new Date('1900-01-01')}
                  />
                </PopoverContent>
              </Popover>
              <FormMessage />
            </FormItem>
          )}
        />

        {children}
      </form>
    </Form>
  );
}

export default ElectionRoundForm;
