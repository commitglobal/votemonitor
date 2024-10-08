import { Slot } from "expo-router";
import CitizenUserContextProvider from "../../contexts/citizen-user/CitizenUserContext.provider";

export default function CitizenLayout() {
  return (
    <CitizenUserContextProvider>
      <Slot />
    </CitizenUserContextProvider>
  );
}
