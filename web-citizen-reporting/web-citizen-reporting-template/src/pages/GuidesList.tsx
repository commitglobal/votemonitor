import { useGuides } from "@/queries/use-guides";

function GuidesList() {
  const { data: guides } = useGuides();

  return <>{JSON.stringify(guides)}</>;
}

export default GuidesList;
