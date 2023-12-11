import FormsTable from "@/components/FormsTable";
import Heading from "@/components/Heading";
import Section from "@/components/Section";

const Dashboard = () => {


  return (
    <>
    <Section className="py-4">
      <div className={"mb-10"}>
        <Heading level={"h2"}>Panou de control</Heading>
      </div>
    </Section>

    <Section>
      <FormsTable/>
    </Section>
    </>
  );
};

export default Dashboard;
