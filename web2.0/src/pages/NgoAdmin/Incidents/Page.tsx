import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import QuickReportsTable from "./components/QuickReports/Table";
import IncidentsTable from "./components/IncidentReports/Table";

function Page() {
  return (
    <Tabs className="w-full" defaultValue="quick-reports">
      <TabsList>
        <TabsTrigger value="quick-reports">Quick reports</TabsTrigger>
        <TabsTrigger value="incident-reports">Incident reports</TabsTrigger>
      </TabsList>

      <TabsContent value="quick-reports">
        <QuickReportsTable />
      </TabsContent>
      <TabsContent value="incident-reports">
        <IncidentsTable />
      </TabsContent>
    </Tabs>
  );
}

export default Page;
