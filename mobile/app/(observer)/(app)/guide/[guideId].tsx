import { Screen } from "../../../../components/Screen";
import Header from "../../../../components/Header";
import { Icon } from "../../../../components/Icon";
import { useRouter, useLocalSearchParams } from "expo-router";
import { useGuide } from "../../../../services/queries/guides.query";
import ResourceScreen from "../../../../components/ResourceScreen";
import { useUserData } from "../../../../contexts/user/UserContext.provider";

const GuideScreen = () => {
  const router = useRouter();
  const { guideId } = useLocalSearchParams();
  const { activeElectionRound } = useUserData();

  const { data: guide } = useGuide(guideId as string, activeElectionRound?.id);

  const handleLeftPress = () => {
    router.back();
  };

  return (
    <Screen preset="fixed" safeAreaEdges={["bottom"]} contentContainerStyle={{ flexGrow: 1 }}>
      <Header
        title={guide?.title || ""}
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={handleLeftPress}
      />
      {guide && <ResourceScreen resource={guide} />}
    </Screen>
  );
};

export default GuideScreen;
