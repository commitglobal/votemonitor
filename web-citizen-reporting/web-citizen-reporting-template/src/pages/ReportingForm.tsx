import type {
  BaseQuestion,
  MultiSelectQuestion,
  SingleSelectQuestion,
} from "@/common/types";
import { Button } from "@/components/ui/button";
import { Calendar } from "@/components/ui/calendar";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Checkbox } from "@/components/ui/checkbox";
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
import { Textarea } from "@/components/ui/textarea";
import {
  cn,
  isDateQuestion,
  isMultiSelectQuestion,
  isNumberQuestion,
  isSingleSelectQuestion,
  isTextQuestion,
} from "@/lib/utils";
import { formsOptions } from "@/queries/use-forms";
import { Route } from "@/routes/forms/$formId";
import { DevTool } from "@hookform/devtools";
import { useSuspenseQuery } from "@tanstack/react-query";
import { notFound } from "@tanstack/react-router";
import { format } from "date-fns";
import { CalendarIcon } from "lucide-react";
import { useCallback, useEffect } from "react";
import { useForm } from "react-hook-form";

function QuestionText({
  languageCode,
  question,
}: {
  languageCode: string;
  question: BaseQuestion;
}) {
  return (
    <FormLabel className="flex gap-1">
      <span> {question.code}</span>
      <span>-</span>
      <span>{question.text[languageCode]}</span>
    </FormLabel>
  );
}

function QuestionDescription({
  languageCode,
  question,
}: {
  languageCode: string;
  question: BaseQuestion;
}) {
  return (
    <FormDescription>{question?.helptext?.[languageCode]}</FormDescription>
  );
}

function ReportingForm() {
  const { formId } = Route.useParams();
  const { data: citizenReportFoms } = useSuspenseQuery(formsOptions());

  const citizenReportForm = citizenReportFoms.find((f) => f.id === formId);

  if (citizenReportForm === undefined) {
    throw notFound({ throw: false });
  }

  const form = useForm();
  const formValues = form.watch();

  useEffect(() => {
    citizenReportForm.questions.forEach((question) => {
      if (isMultiSelectQuestion(question)) {
        form.setValue(`question-${question.id}`, []);
      }

      if (isTextQuestion(question)) {
        form.setValue(`question-${question.id}`, "");
      }
    });
  }, [form.setValue, citizenReportForm]);

  const questionHasFreeTextOption = useCallback(
    (question: SingleSelectQuestion | MultiSelectQuestion) => {
      return question.options.some((option) => option.isFreeText);
    },
    []
  );

  const getFreeTextOption = useCallback(
    (question: SingleSelectQuestion | MultiSelectQuestion) => {
      return question.options.find((option) => option.isFreeText);
    },
    []
  );

  const isFreeTextOptionSelected = useCallback(
    (question: SingleSelectQuestion | MultiSelectQuestion) => {
      console.log("formValues", formValues);
      const freeTextOption = getFreeTextOption(question);
      const answer = formValues[`question-${question.id}`];
      if (!answer) {
        return false;
      }

      if (isSingleSelectQuestion(question)) {
        return !!answer || answer === freeTextOption?.id;
      } else {
        const selection = answer as string[];
        return selection.some((s) => s === freeTextOption?.id);
      }
    },
    [formValues]
  );

  function onSubmit() {
    console.log("submit");
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>
          {citizenReportForm.name[citizenReportForm.defaultLanguage]}
        </CardTitle>
        <CardDescription>
          {citizenReportForm.description[citizenReportForm.defaultLanguage]}
        </CardDescription>
      </CardHeader>

      <CardContent className="flex flex-col gap-4">
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-8">
            {citizenReportForm.questions.map((question) => (
              <div className="w-full" key={question.id}>
                {isTextQuestion(question) && (
                  <FormField
                    control={form.control}
                    name={`question-${question.id}`}
                    render={({ field }) => (
                      <FormItem>
                        <QuestionText
                          question={question}
                          languageCode={citizenReportForm.defaultLanguage}
                        />
                        <QuestionDescription
                          question={question}
                          languageCode={citizenReportForm.defaultLanguage}
                        />

                        <FormControl>
                          <Textarea
                            placeholder={
                              question?.inputPlaceholder?.[
                                citizenReportForm.defaultLanguage
                              ]
                            }
                            {...field}
                          />
                        </FormControl>

                        <FormMessage />
                      </FormItem>
                    )}
                  />
                )}
                {isNumberQuestion(question) && (
                  <FormField
                    control={form.control}
                    name={`question-${question.id}`}
                    render={({ field }) => (
                      <FormItem>
                        <QuestionText
                          question={question}
                          languageCode={citizenReportForm.defaultLanguage}
                        />
                        <QuestionDescription
                          question={question}
                          languageCode={citizenReportForm.defaultLanguage}
                        />
                        <FormControl>
                          <Input
                            type="number"
                            placeholder={
                              question?.inputPlaceholder?.[
                                citizenReportForm.defaultLanguage
                              ]
                            }
                            {...field}
                          />
                        </FormControl>

                        <FormMessage />
                      </FormItem>
                    )}
                  />
                )}

                {isDateQuestion(question) && (
                  <FormField
                    control={form.control}
                    name={`question-${question.id}`}
                    render={({ field }) => (
                      <FormItem className="flex flex-col">
                        <QuestionText
                          question={question}
                          languageCode={citizenReportForm.defaultLanguage}
                        />
                        <QuestionDescription
                          question={question}
                          languageCode={citizenReportForm.defaultLanguage}
                        />
                        <Popover>
                          <PopoverTrigger asChild>
                            <FormControl>
                              <Button
                                variant={"outline"}
                                className={cn(
                                  "w-[240px] pl-3 text-left font-normal",
                                  !field.value && "text-muted-foreground"
                                )}
                              >
                                {field.value ? (
                                  format(field.value, "PPP")
                                ) : (
                                  <span>Pick a date</span>
                                )}
                                <CalendarIcon className="ml-auto h-4 w-4 opacity-50" />
                              </Button>
                            </FormControl>
                          </PopoverTrigger>
                          <PopoverContent className="w-auto p-0" align="start">
                            <Calendar
                              mode="single"
                              selected={field.value}
                              onSelect={field.onChange}
                              disabled={(date) =>
                                date > new Date() ||
                                date < new Date("1900-01-01")
                              }
                              initialFocus
                            />
                          </PopoverContent>
                        </Popover>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                )}

                {isSingleSelectQuestion(question) && (
                  <div className="flex flex-col gap-4">
                    <FormField
                      control={form.control}
                      name={`question-${question.id}`}
                      render={({ field }) => (
                        <FormItem className="space-y-3">
                          <QuestionText
                            question={question}
                            languageCode={citizenReportForm.defaultLanguage}
                          />
                          <QuestionDescription
                            question={question}
                            languageCode={citizenReportForm.defaultLanguage}
                          />
                          <FormControl>
                            <RadioGroup
                              onValueChange={field.onChange}
                              defaultValue={field.value}
                              className="flex flex-col space-y-1"
                            >
                              {question.options.map((option) => (
                                <FormItem
                                  className="flex items-center space-x-3 space-y-0"
                                  key={`${question.id}-${option.id}`}
                                >
                                  <FormControl>
                                    <RadioGroupItem value={option.id} />
                                  </FormControl>
                                  <FormLabel className="font-normal">
                                    {
                                      option.text[
                                        citizenReportForm.defaultLanguage
                                      ]
                                    }
                                  </FormLabel>
                                </FormItem>
                              ))}
                            </RadioGroup>
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                    {questionHasFreeTextOption(question) &&
                      isFreeTextOptionSelected(question) && (
                        <FormField
                          control={form.control}
                          name={`question-${question.id}-freeText`}
                          render={({ field }) => (
                            <FormItem>
                              <FormControl>
                                <Textarea
                                  {...field}
                                  placeholder={
                                    getFreeTextOption(question)?.text?.[
                                      citizenReportForm.defaultLanguage
                                    ]
                                  }
                                />
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                      )}
                  </div>
                )}

                {isMultiSelectQuestion(question) && (
                  <div className="flex flex-col gap-4">
                    <FormField
                      control={form.control}
                      name={`question-${question.id}`}
                      render={() => (
                        <FormItem>
                          <div className="mb-4">
                            <QuestionText
                              question={question}
                              languageCode={citizenReportForm.defaultLanguage}
                            />
                            <QuestionDescription
                              question={question}
                              languageCode={citizenReportForm.defaultLanguage}
                            />
                          </div>
                          {question.options.map((option) => (
                            <FormField
                              key={option.id}
                              control={form.control}
                              name={`question-${question.id}`}
                              render={({ field }) => {
                                return (
                                  <FormItem
                                    key={option.id}
                                    className="flex flex-row items-start space-x-3 space-y-0"
                                  >
                                    <FormControl>
                                      <Checkbox
                                        checked={field.value?.includes(
                                          option.id
                                        )}
                                        onCheckedChange={(checked) => {
                                          return checked
                                            ? field.onChange([
                                                ...field.value,
                                                option.id,
                                              ])
                                            : field.onChange(
                                                field.value?.filter(
                                                  (value: string) =>
                                                    value !== option.id
                                                )
                                              );
                                        }}
                                      />
                                    </FormControl>
                                    <FormLabel className="text-sm font-normal">
                                      {
                                        option.text[
                                          citizenReportForm.defaultLanguage
                                        ]
                                      }
                                    </FormLabel>
                                  </FormItem>
                                );
                              }}
                            />
                          ))}
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                    {questionHasFreeTextOption(question) &&
                      isFreeTextOptionSelected(question) && (
                        <FormField
                          control={form.control}
                          name={`question-${question.id}-freeText`}
                          render={({ field }) => (
                            <FormItem>
                              <FormControl>
                                <Textarea
                                  {...field}
                                  placeholder={
                                    getFreeTextOption(question)?.text?.[
                                      citizenReportForm.defaultLanguage
                                    ]
                                  }
                                />
                              </FormControl>
                              <FormMessage />
                            </FormItem>
                          )}
                        />
                      )}
                  </div>
                )}
              </div>
            ))}
          </form>
          <DevTool control={form.control} /> {/* set up the dev tool */}
        </Form>
        <div className="w-full flex justify-end">
          <div className="flex gap-4">
            <Button variant="outline">Cancel</Button>
            <Button type="submit" className="cursor-pointer">
              Submit
            </Button>
          </div>
        </div>
      </CardContent>
    </Card>
  );
}

export default ReportingForm;
