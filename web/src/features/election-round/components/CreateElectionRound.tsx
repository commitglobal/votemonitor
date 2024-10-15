import { zodResolver } from '@hookform/resolvers/zod';
import CreateDialog, { CreateDialogFooter } from '@/components/dialogs/CreateDialog';
import type { ReactNode } from 'react';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { ElectionRound, electionRoundFormSchema, type ElectionRoundFormValues } from '../models/ElectionRound';
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { Button } from '@/components/ui/button';
import { CalendarIcon, CheckIcon, ChevronDownIcon } from '@heroicons/react/24/outline';
import { Calendar } from '@/components/ui/calendar';
import { cn } from '@/lib/utils';
import { format, parseISO, formatISO } from 'date-fns';
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from '@/components/ui/command';
import { useMutation } from '@tanstack/react-query';
import { authApi } from '@/common/auth-api';
import { queryClient } from '@/main';
import { electionRoundKeys } from '../queries';
import { useCountries } from '@/hooks/countries';

// This can come from the API.
const defaultValues: Partial<ElectionRoundFormValues> = {
  // title: "Election Round title",
  // englishTitle: "Election Round English title",
  // countryId: '51aa4900-30a6-91b7-2728-071542a064ff',
  // startDate: new Date("2024-03-22"),
};


const CreateElectionRoundForm = (): ReactNode => {
  const { data: countries } = useCountries();

  const { t } = useTranslation();

  const form = useForm<ElectionRoundFormValues>({
    resolver: zodResolver(electionRoundFormSchema),
    defaultValues,
  });

  const mutation = useMutation({
    mutationFn: (data: ElectionRoundFormValues) => {
      return authApi.post<ElectionRoundFormValues, ElectionRound>('/election-rounds', data)
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: electionRoundKeys.all })
    }
  })

  function onSubmit(data: ElectionRoundFormValues) {
    mutation.mutate(data);
  }

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)}>
        <div className='grid gap-6 py-4 sm:grid-cols-2'>
          <FormField
            control={form.control}
            name='title'
            render={({ field }) => (
              <FormItem className='sm:col-span-2'>
                <FormLabel>{t('election-round.field.title')}</FormLabel>
                <FormControl>
                  <Input placeholder={t('election-round.placeholder.title')} {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name='englishTitle'
            render={({ field }) => (
              <FormItem className='sm:col-span-2'>
                <FormLabel>{t('election-round.field.englishTitle')}</FormLabel>
                <FormControl>
                  <Input placeholder={t('election-round.placeholder.englishTitle')} {...field} />
                </FormControl>
                <FormDescription>This is the name that will be displayed on your profile and in emails.</FormDescription>
                <FormMessage />
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name="countryId"
            render={({ field }) => (
              <FormItem className="flex flex-col sm:col-span-2">
                <FormLabel>{t('election-round.field.country')}</FormLabel>
                <Popover>
                  <PopoverTrigger asChild>
                    <FormControl>
                      <Button
                        variant="outline"
                        className={cn(
                          "w-full justify-between",
                          !field.value && "text-muted-foreground"
                        )}
                      >
                        {field.value
                          ? countries?.find(
                            (country) => country.id === field.value
                          )?.fullName
                          : t('app.action.select')}
                        <ChevronDownIcon className="w-4 h-4 ml-2 opacity-50 shrink-0" />
                      </Button>
                    </FormControl>
                  </PopoverTrigger>
                  <PopoverContent className="w-[200px] p-0">
                    <Command>
                      <CommandInput placeholder="Search country..." />
                      <CommandEmpty>No country found.</CommandEmpty>
                      {/* TODO: https://github.com/shadcn-ui/ui/issues/2944 */}
                      <CommandList>
                        <CommandGroup>
                          {!!countries && countries?.map((country) => (
                            <CommandItem
                              value={country.fullName}
                              key={country.id}
                              onSelect={() => {
                                form.setValue("countryId", country.id)
                              }}
                            >
                              <CheckIcon
                                className={cn(
                                  "mr-2 h-4 w-4",
                                  country.id === field.value
                                    ? "opacity-100"
                                    : "opacity-0"
                                )}
                              />
                              {country.fullName}
                            </CommandItem>
                          ))}
                        </CommandGroup>
                      </CommandList>
                    </Command>
                  </PopoverContent>
                </Popover>
                <FormDescription>
                  This is the country that will be used in the dashboard.
                </FormDescription>
                <FormMessage />
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name='startDate'
            render={({ field }) => (
              <FormItem className='flex flex-col'>
                <FormLabel>{t('election-round.field.startDate')}</FormLabel>
                <Popover>
                  <PopoverTrigger asChild>
                    <FormControl>
                      <Button
                        variant={'outline'}
                        className={cn('w-full pl-3 text-left font-normal', !field.value && 'text-muted-foreground')}>
                        {field.value ? format(field.value, 'PPP') : <span>Pick a date</span>}
                        <CalendarIcon className='w-4 h-4 ml-auto opacity-50' />
                      </Button>
                    </FormControl>
                  </PopoverTrigger>
                  <PopoverContent className='w-auto p-0' align='start'>
                    <Calendar
                      mode='single'
                      selected={field.value ? parseISO(field.value) : undefined}
                      onSelect={(day) => {
                        form.setValue("startDate", formatISO(day!, { representation: 'date' }))
                      }}
                      disabled={(date) => date < new Date()}
                      autoFocus
                    />
                  </PopoverContent>
                </Popover>
                <FormDescription>Your date of birth is used to calculate your age.</FormDescription>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>

        <CreateDialogFooter />
      </form>
    </Form>
  );
};

const CreateElectionRound = (): ReactNode => {
  const { t } = useTranslation();

  return (
    <CreateDialog title={t('election-round.action.create')}>
      <CreateElectionRoundForm />
    </CreateDialog>
  );
};

export default CreateElectionRound;
