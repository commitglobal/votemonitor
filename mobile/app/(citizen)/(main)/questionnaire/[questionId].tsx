import { Text } from "tamagui";
import { Screen } from "../../../../components/Screen";
import Header from "../../../../components/Header";
import { useLocalSearchParams, useRouter } from "expo-router";
import { Icon } from "../../../../components/Icon";

const Questionnaire = () => {
  const { questionId } = useLocalSearchParams<{ questionId: string }>();
  console.log("questionId", questionId);
  const router = useRouter();

  return (
    <Screen>
      <Header
        title="Questionnaire"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => {
          router.back();
        }}
      />
      <Text>Questionnaire</Text>
    </Screen>
  );
};

export default Questionnaire;
