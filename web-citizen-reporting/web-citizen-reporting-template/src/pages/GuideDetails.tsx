import { Spinner } from "@/components/Spinner";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { downloadFile } from "@/lib/utils";
import { useGuides } from "@/queries/use-guides";
import { Route } from "@/routes/guides/$guideId";
import { notFound } from "@tanstack/react-router";
import { Download, ExternalLink } from "lucide-react";

function GuideDetails() {
  const { guideId } = Route.useParams();

  const { data: guide, isLoading } = useGuides((guides) =>
    guides.find((g) => g.id === guideId)
  );

  if (isLoading) return <Spinner show={isLoading} />;

  if (!guide) throw notFound();
  return (
    <>
      <Card>
        <CardHeader>
          <CardTitle>{guide.title}</CardTitle>
        </CardHeader>
        {guide.guideType === "Text" && (
          <CardContent className="h-full w-full">
            <div dangerouslySetInnerHTML={{ __html: guide.text }} />
          </CardContent>
        )}
        {guide.guideType === "Website" && (
          <Button variant="link" className="cursor-pointer">
            <a href={guide.websiteUrl}>Visit Website</a>
            <ExternalLink className="h-4 w-4" />
          </Button>
        )}
        {guide.guideType === "Document" && (
          <Button
            variant="link"
            className="cursor-pointer"
            onClick={() => downloadFile(guide.presignedUrl)}
          >
            Download
            <Download className="h-4 w-4" />
          </Button>
        )}
      </Card>
    </>
  );
}

export default GuideDetails;
