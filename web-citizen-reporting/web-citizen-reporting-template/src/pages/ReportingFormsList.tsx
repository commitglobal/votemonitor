import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { formsOptions } from "@/queries/use-forms";
import { useSuspenseQuery } from "@tanstack/react-query";
import { Link } from "@tanstack/react-router";

function ReportingFormsList() {
  const { data: forms } = useSuspenseQuery(formsOptions());

  const sortedForms = [...forms].sort(
    (a, b) => a.displayOrder - b.displayOrder
  );

  return (
    <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
      {sortedForms.map((form) => (
        <Card
          key={form.id}
          className="overflow-hidden flex flex-col gap-3 h-full"
        >
          <CardHeader className="flex flex-row items-center gap-3 pb-2">
            {form.icon && (
              <div
                className="flex h-8 w-8 items-center justify-center rounded-full bg-primary/10"
                dangerouslySetInnerHTML={{ __html: form.icon }}
              />
            )}
            <CardTitle className="line-clamp-1 text-lg">
              {form.name[form.defaultLanguage]}
            </CardTitle>
          </CardHeader>
          <CardContent className="h-full flex-1 flex flex-col justify-between">
            <section>
              <p className="leading-7 [&:not(:first-child)]:mt-6 line-clamp-3">
                {form.description[form.defaultLanguage]}
              </p>
              <div className="mt-4 flex items-center text-xs text-muted-foreground">
                <span>{form.numberOfQuestions} questions</span>
              </div>
            </section>

            <Button asChild className="mt-4 w-24">
              <Link to={"/forms/$formId"} params={{ formId: form.id }}>
                Fill in
              </Link>
            </Button>
          </CardContent>
        </Card>
      ))}
    </div>
  );
}

export default ReportingFormsList;
