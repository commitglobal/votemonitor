import { Route } from "@/routes/hello/$name";
import Greeter from "./components/Greeter";

export interface HelloPageProps {}
function HelloPage({}: HelloPageProps) {
  const { name } = Route.useParams();
  return <Greeter name={name} />;
}

export default HelloPage;
