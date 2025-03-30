import { Spinner } from "@/components/Spinner";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useGuides } from "@/queries/use-guides";
import { Route } from "@/routes/guides/$guideId";
import { notFound } from "@tanstack/react-router";

function GuideDetails() {
  const { guideId } = Route.useParams();

  const { data: guide, isLoading } = useGuides((guides) =>
    guides.find((g) => g.id === guideId)
  );

  {
    console.log(guide);
  }
  // return <>{JSON.stringify(guide)}</>;

  /* 
    {guide.guideType === GuideType.Text && (
              <BookOpen className="h-5 w-5" />
            )}
            {guide.guideType === GuideType.Website && (
              <Globe className="h-5 w-5" />
            )}
            {guide.guideType === GuideType.Document && (
              <FileDown className="h-5 w-5" />
            )}
  */
  if (isLoading) return <Spinner show={isLoading} />;

  if (!guide) throw notFound();
  return (
    <>
      <Card>
        <CardHeader>
          <CardTitle>{guide.title}</CardTitle>
        </CardHeader>
        {guide.guideType === "Text" && (
          <CardContent>
            <div dangerouslySetInnerHTML={{ __html: guide.text }} />
          </CardContent>
        )}
      </Card>
    </>
  );
}

export default GuideDetails;
