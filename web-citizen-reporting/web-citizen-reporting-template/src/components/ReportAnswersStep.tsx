import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { currentFormLanguageAtom } from "@/features/forms/atoms";
import { FormQuestion } from "@/features/forms/components/FormQuestion";
import { Route } from "@/routes/forms/$formId";
import { useAtom } from "jotai";
import { useEffect } from "react";
import { useFormContext } from "react-hook-form";
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "./ui/select";

function ReportAnswersStep() {
  const citizenReportForm = Route.useLoaderData();
  const [selectedLanguage, setSelectedLanguage] = useAtom(
    currentFormLanguageAtom
  );

  const { watch } = useFormContext();
  const values = watch();

  useEffect(() => {
    console.table(values);
  }, [values]);

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
          <FormQuestion question={question} />
        ))}
      </CardContent>
    </Card>
  );
}

export default ReportAnswersStep;
