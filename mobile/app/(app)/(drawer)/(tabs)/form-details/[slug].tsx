import { Typography } from "../../../../../components/Typography";
import { useLocalSearchParams } from "expo-router";
import { Screen } from "../../../../../components/Screen";

const FormDetails = () => {
  const { slug } = useLocalSearchParams();
  return (
    <Screen preset="scroll">
      <Typography>This is form details {slug}</Typography>
      {/* <Button onPress={() => router.replace(`/form-questionnaire/${+(slug || 0) + 1}`)}>
        Next
      </Button> */}
    </Screen>
  );
};

export default FormDetails;
