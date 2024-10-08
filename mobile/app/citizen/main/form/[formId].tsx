import { Text } from "tamagui";
import { Screen } from "../../../../components/Screen";
import Header from "../../../../components/Header";
import { useLocalSearchParams, useRouter } from "expo-router";
import { Icon } from "../../../../components/Icon";
import { useGetCitizenReportingFormById } from "../../../../services/queries/citizen.query";
import { useCitizenUserData } from "../../../../contexts/citizen-user/CitizenUserContext.provider";
import { Typography } from "../../../../components/Typography";

const CitizenForm = () => {
  const { formId, selectedLocationId } = useLocalSearchParams<{
    formId: string;
    selectedLocationId: string;
  }>();

  if (!formId || !selectedLocationId) {
    return <Typography>Incorrect page params</Typography>;
  }

  console.log("🔵 [CitizenForm] formId", formId);
  console.log("🔵 [CitizenForm] selectedLocationId", selectedLocationId);
  const router = useRouter();

  const { selectedElectionRound } = useCitizenUserData();

  const {
    data: currentForm,
    isLoading: isLoadingCurrentForm,
    error: currentFormError,
  } = useGetCitizenReportingFormById(selectedElectionRound, formId);

  console.log("🔵 [CitizenForm] currentForm", currentForm);

  if (isLoadingCurrentForm) {
    return <Typography>Loading...</Typography>;
  }

  if (currentFormError) {
    return <Typography>Error loading form {JSON.stringify(currentFormError)}</Typography>;
  }

  return (
    <Screen>
      <Header
        title="CitizenForm"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => {
          router.back();
        }}
      />
      <Text>CitizenForm</Text>
    </Screen>
  );
};

export default CitizenForm;
