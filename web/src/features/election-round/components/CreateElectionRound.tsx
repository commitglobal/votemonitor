import { zodResolver } from '@hookform/resolvers/zod';
import CreateDialog, { CreateDialogFooter } from '@/components/dialogs/CreateDialog';
import type { ReactNode } from 'react';
import { useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { electionRoundFormSchema, type ElectionRoundFormValues } from '../models/ElectionRound';
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { Button } from '@/components/ui/button';
import { CalendarIcon, CheckIcon, ChevronDownIcon } from '@heroicons/react/24/outline';
import { Calendar } from '@/components/ui/calendar';
import { cn } from '@/lib/utils';
import { format } from 'date-fns';
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem } from '@/components/ui/command';
import { useMutation } from '@tanstack/react-query';
import axios from 'axios';

// This can come from the API.
const defaultValues: Partial<ElectionRoundFormValues> = {
  // title: "Election Round title",
  // englishTitle: "Election Round English title",
  countryId: 'ROU',
  // startDate: new Date("2024-03-22"),
};

const countries = [
  { 'value': 'ROU', 'label': 'Romania' },
  { 'value': 'FRA', 'label': 'France' },
  { 'value': 'DEU', 'label': 'Germany' },
  { 'value': 'GBR', 'label': 'United Kingdom' },
  { 'value': 'USA', 'label': 'United States' },
];

const CreateElectionRoundForm = (): ReactNode => {
  const { t } = useTranslation();

  const form = useForm<ElectionRoundFormValues>({
    resolver: zodResolver(electionRoundFormSchema),
    defaultValues,
  });

  const mutation = useMutation({
    mutationFn: (data) => {
      return axios.post('/election-rounds', data) // TODO: figure out proper using endpoing with auth
    },
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
                          ? countries.find(
                            (country) => country.value === field.value
                          )?.label
                          : t('app.action.select')}
                        <ChevronDownIcon className="w-4 h-4 ml-2 opacity-50 shrink-0" />
                      </Button>
                    </FormControl>
                  </PopoverTrigger>
                  <PopoverContent className="w-[200px] p-0">
                    <Command>
                      <CommandInput placeholder="Search country..." />
                      <CommandEmpty>No country found.</CommandEmpty>
                      <CommandGroup>
                        {countries.map((country) => (
                          <CommandItem
                            value={country.label}
                            key={country.value}
                            onSelect={() => {
                              form.setValue("countryId", country.value)
                            }}
                          >
                            <CheckIcon
                              className={cn(
                                "mr-2 h-4 w-4",
                                country.value === field.value
                                  ? "opacity-100"
                                  : "opacity-0"
                              )}
                            />
                            {country.label}
                          </CommandItem>
                        ))}
                      </CommandGroup>
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
                      selected={field.value}
                      onSelect={field.onChange}
                      disabled={(date) => date > new Date() || date < new Date('1900-01-01')}
                      initialFocus
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
