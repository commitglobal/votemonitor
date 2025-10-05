import { H1, P } from "@/components/ui/typography";
import QuickReportsTable from "./components/Table";

function Page() {
  return (
    <>
      <div>
        <H1>Quick reports</H1>
        <P>Here&apos;s all quick reports from your observers</P>
      </div>
      <QuickReportsTable />
    </>
  );
}

export default Page;
