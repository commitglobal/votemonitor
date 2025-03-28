import { useGuides } from "@/queries/use-guides";
import { Route } from "@/routes/guides/$guideId";

function GuideDetails() {
  const { guideId } = Route.useParams();

  const { data: guide } = useGuides((guides) =>
    guides.find((g) => g.id === guideId)
  );

  return <>{JSON.stringify(guide)}</>;
}

export default GuideDetails;
