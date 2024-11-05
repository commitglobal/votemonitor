import { Screen } from "../../../../components/Screen";
import Header from "../../../../components/Header";
import { Icon } from "../../../../components/Icon";
import { useRouter, useLocalSearchParams } from "expo-router";
import { useCitizenUserData } from "../../../../contexts/citizen-user/CitizenUserContext.provider";
import { useResource } from "../../../../services/queries/guides.query";
import ResourceScreen from "../../../../components/ResourceScreen";
import { useSafeAreaInsets } from "react-native-safe-area-context";

const Resource = () => {
  const router = useRouter();
  const { resourceId } = useLocalSearchParams();
  const { selectedElectionRound } = useCitizenUserData();
  const insets = useSafeAreaInsets();
  const { data: guide } = useResource(resourceId as string, selectedElectionRound || undefined);

  const handleLeftPress = () => {
    router.back();
  };

  return (
    <Screen
      preset="fixed"
      contentContainerStyle={{ flexGrow: 1, paddingBottom: 16 + insets.bottom }}
      style={{ backgroundColor: "white" }}
    >
      <Header
        title={guide?.title || ""}
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={handleLeftPress}
      />

      {guide && <ResourceScreen resource={guide} />}
    </Screen>
  );
};

export default Resource;
