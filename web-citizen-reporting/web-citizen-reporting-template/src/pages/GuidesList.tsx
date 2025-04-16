import { GuideType, type GuideModel } from "@/common/types";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { downloadFile } from "@/lib/utils";
import { useGuides } from "@/queries/use-guides";
import { Link } from "@tanstack/react-router";
import {
  BookOpen,
  ChevronRight,
  Download,
  ExternalLink,
  FileDown,
  Globe,
} from "lucide-react";

function GuidesList() {
  const { data: guides } = useGuides();

  return (
    <div className="w-full flex flex-col gap-4 mb-4">
      <div className="w-full">
        <h2 className="mt-10 scroll-m-20 border-b pb-2 text-3xl font-semibold tracking-tight transition-colors first:mt-0">
          Citizen guides
        </h2>
        <p className="leading-7 [&:not(:first-child)]:mt-6">
          Access our comprehensive collection of guides to help you navigate the
          electoral process. From registration to casting your vote, we provide
          all the information you need.
        </p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 lg:grid-cols-4 gap-6">
        {guides?.map((guide) => (
          <GuideCard key={guide.id} guide={guide} />
        ))}
      </div>
    </div>
  );
}

function GuideCard({ guide }: { guide: GuideModel }) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>
          <div className="flex flex-row gap-3">
            {guide.guideType === GuideType.Text && (
              <BookOpen className="h-5 w-5" />
            )}
            {guide.guideType === GuideType.Website && (
              <Globe className="h-5 w-5" />
            )}
            {guide.guideType === GuideType.Document && (
              <FileDown className="h-5 w-5" />
            )}
            <span>{guide.title}</span>
          </div>
        </CardTitle>
      </CardHeader>
      <CardContent>
        {guide.guideType === GuideType.Text && (
          <Button variant="link" asChild>
            <Link
              to="/guides/$guideId"
              params={{ guideId: guide.id }}
              className="flex"
            >
              Read Guide
              <ChevronRight className="h-4 w-4" />
            </Link>
          </Button>
        )}
        {guide.guideType === GuideType.Website && (
          <Button variant="link" className="cursor-pointer">
            <a href={guide.websiteUrl}>Visit Website</a>
            <ExternalLink className="h-4 w-4" />
          </Button>
        )}
        {guide.guideType === GuideType.Document && (
          <Button
            variant="link"
            className="cursor-pointer"
            onClick={() => downloadFile(guide.presignedUrl)}
          >
            Download
            <Download className="h-4 w-4" />
          </Button>
        )}
      </CardContent>
    </Card>
  );
}

export default GuidesList;
