import { useCitizenUserData } from "../../contexts/citizen-user/CitizenUserContext.provider";
import { Redirect } from "expo-router";

export default function CitizenIndex() {
  console.log("🔵 1. CITIZEN - INDEX");

  const { selectedElectionRound } = useCitizenUserData();

  console.log("👀 [CitizenIndex] selectedElectionRound", selectedElectionRound);

  if (!selectedElectionRound) {
    return <Redirect href="/citizen/select-election-rounds" />;
  }

  return <Redirect href="/citizen/main" />;
}
