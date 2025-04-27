import {
  type BaseQuestion,
  type MultiSelectQuestion,
  type SingleSelectQuestion,
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
import { FileUploader } from "@/components/ui/file-uploader";
import {
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
import { Separator } from "@/components/ui/separator";
import { Textarea } from "@/components/ui/textarea";
import { useUploadFile } from "@/hooks/use-upload-file";
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
import { useSuspenseQuery } from "@tanstack/react-query";
import { notFound } from "@tanstack/react-router";
import { format } from "date-fns";
import { CalendarIcon } from "lucide-react";
import React, { useCallback, useEffect, useState } from "react";
import { useFormContext } from "react-hook-form";
import { NumberInput } from "./ui/number-input";
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "./ui/select";

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

function ReportAnswersStep() {
  const { formId } = Route.useParams();
  const { data: citizenReportFoms } = useSuspenseQuery(formsOptions());
  const [loading, setLoading] = React.useState(false);
  const { onUpload, progresses, uploadedFiles, isUploading } = useUploadFile({
    defaultUploadedFiles: [],
  });

  const citizenReportForm = citizenReportFoms.find((f) => f.id === formId);

  if (citizenReportForm === undefined) {
    throw notFound({ throw: false });
  }

  const form = useFormContext();

  const formValues = form.watch();

  const [selectedLanguage, setSelectedLanguage] = useState<string>(
    citizenReportForm.defaultLanguage
  );

  useEffect(() => {
    const dirtyFieldsSet = new Set(Object.keys(form.formState.dirtyFields));
    citizenReportForm.questions.forEach((question) => {
      // do not reset if the user typed anything in that field
      if (dirtyFieldsSet.has(`question-${question.id}`)) return;

      if (isMultiSelectQuestion(question)) {
        form.setValue(`question-${question.id}.selection`, []);
      }

      if (isTextQuestion(question) || isNumberQuestion(question)) {
        form.setValue(`question-${question.id}`, "");
      }
    });
  }, [form.setValue, form.formState.dirtyFields, citizenReportForm]);

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
      const freeTextOption = getFreeTextOption(question);
      const selection = formValues[`question-${question.id}.selection`];

      if (!selection) {
        return false;
      }

      if (isSingleSelectQuestion(question)) {
        return !!selection || selection === freeTextOption?.id;
      } else {
        return selection.some((s: string) => s === freeTextOption?.id);
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
        <CardTitle className="flex justify-between mb-4">
          <div>{citizenReportForm.name[selectedLanguage]}</div>

          <div className="flex flex-row gap-2 items-center">
            <div className="p-1 mb-1">Language:</div>
            <Select
              onValueChange={setSelectedLanguage}
              defaultValue={selectedLanguage}
              value={selectedLanguage}
            >
              <SelectTrigger>
                <SelectValue placeholder="Language" />
              </SelectTrigger>
              <SelectContent>
                <SelectGroup>
                  {citizenReportForm.languages.map((language) => (
                    <SelectItem key={language} value={language}>
                      {language}
                    </SelectItem>
                  ))}
                </SelectGroup>
              </SelectContent>
            </Select>
          </div>
        </CardTitle>
        <CardDescription>
          {citizenReportForm.description[selectedLanguage]}
        </CardDescription>
      </CardHeader>

      <CardContent className="flex flex-col gap-8">
        {citizenReportForm.questions.map((question) => (
          <div className="w-full flex flex-col gap-4" key={question.id}>
            {isTextQuestion(question) && (
              <FormField
                control={form.control}
                name={`question-${question.id}`}
                rules={{ required: true }}
                render={({ field }) => (
                  <FormItem>
                    <QuestionText
                      question={question}
                      languageCode={selectedLanguage}
                    />
                    <QuestionDescription
                      question={question}
                      languageCode={selectedLanguage}
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
                rules={{ required: true }}
                render={({ field }) => (
                  <FormItem>
                    <QuestionText
                      question={question}
                      languageCode={selectedLanguage}
                    />
                    <QuestionDescription
                      question={question}
                      languageCode={selectedLanguage}
                    />
                    <FormControl>
                      <NumberInput
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
                rules={{ required: true }}
                render={({ field }) => (
                  <FormItem className="flex flex-col">
                    <QuestionText
                      question={question}
                      languageCode={selectedLanguage}
                    />
                    <QuestionDescription
                      question={question}
                      languageCode={selectedLanguage}
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
                            date > new Date() || date < new Date("1900-01-01")
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
                  name={`question-${question.id}.selection`}
                  rules={{ required: true }}
                  render={({ field }) => (
                    <FormItem className="space-y-3">
                      <QuestionText
                        question={question}
                        languageCode={selectedLanguage}
                      />
                      <QuestionDescription
                        question={question}
                        languageCode={selectedLanguage}
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
                                {option.text[selectedLanguage]}
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
                      name={`question-${question.id}.${
                        getFreeTextOption(question)?.id
                      }`}
                      rules={{ required: true }}
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
                  name={`question-${question.id}.selection`}
                  render={() => (
                    <FormItem>
                      <div className="mb-4">
                        <QuestionText
                          question={question}
                          languageCode={selectedLanguage}
                        />
                        <QuestionDescription
                          question={question}
                          languageCode={selectedLanguage}
                        />
                      </div>
                      {question.options.map((option) => (
                        <FormField
                          key={option.id}
                          control={form.control}
                          name={`question-${question.id}.selection`}
                          rules={{ required: true, minLength: 1 }}
                          render={({ field }) => {
                            return (
                              <FormItem
                                key={option.id}
                                className="flex flex-row items-start space-x-3 space-y-0"
                              >
                                <FormControl>
                                  <Checkbox
                                    checked={field.value?.includes(option.id)}
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
                      name={`question-${question.id}.${
                        getFreeTextOption(question)?.id
                      }`}
                      rules={{ required: true }}
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

            <FormField
              control={form.control}
              name={`question-${question.id}.attachments`}
              render={({ field }) => (
                <div className="space-y-6">
                  <FormItem className="w-full">
                    <FormLabel>Attachments </FormLabel>
                    <FormControl>
                      <FileUploader
                        value={field.value}
                        onValueChange={field.onChange}
                        maxFileCount={undefined}
                        maxSize={undefined}
                        progresses={progresses}
                        // pass the onUpload function here for direct upload
                        // onUpload={uploadFiles}
                        disabled={isUploading}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                  {/* {uploadedFiles.length > 0 ? (
                        <UploadedFilesCard uploadedFiles={uploadedFiles} />
                      ) : null} */}
                </div>
              )}
            />
            <Separator />
          </div>
        ))}
      </CardContent>
    </Card>
  );
}

export default ReportAnswersStep;
