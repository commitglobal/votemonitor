import { Screen } from "../../../../components/Screen";
import Header from "../../../../components/Header";
import { Icon } from "../../../../components/Icon";
import { useRouter, useLocalSearchParams } from "expo-router";
import { useCitizenUserData } from "../../../../contexts/citizen-user/CitizenUserContext.provider";
import { useGuide } from "../../../../services/queries/guides.query";
import RenderHtml from "react-native-render-html";
import { tagsStyles } from "../../../../components/NotificationListItem";
import { useWindowDimensions } from "react-native";
import { ScrollView, YStack } from "tamagui";

const Resource = () => {
  const router = useRouter();
  const { resourceId } = useLocalSearchParams();
  const { width } = useWindowDimensions();
  const { selectedElectionRound } = useCitizenUserData();

  const { data: guide } = useGuide(resourceId as string, selectedElectionRound || undefined);

  const handleLeftPress = () => {
    router.back();
  };

  return (
    <Screen preset="fixed" safeAreaEdges={["bottom"]}>
      <Header
        title={guide?.title || ""}
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={handleLeftPress}
      />

      {guide && (
        <YStack paddingHorizontal={"$md"} height={"85%"}>
          <ScrollView
            showsVerticalScrollIndicator={false}
            contentContainerStyle={{
              gap: 32,
              flexGrow: 1,
            }}
          >
            <RenderHtml
              source={{
                html: guide.text || "",
              }}
              contentWidth={width - 32}
              // @ts-ignore
              tagsStyles={tagsStyles}
            />
          </ScrollView>
        </YStack>
      )}
    </Screen>
  );
};

export default Resource;
