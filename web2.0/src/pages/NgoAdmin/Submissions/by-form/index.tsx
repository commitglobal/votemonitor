import { ContentSection } from "../components/content-section";
import { Table } from "./table";

export function SubmissionsByForm() {
  return (
    <ContentSection title="View aggregated by form">
      <Table />
    </ContentSection>
  );
}
