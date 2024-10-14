import RenderHtml from "react-native-render-html";
import { useWindowDimensions } from "react-native";
import { ScrollView, YStack } from "tamagui";
import { Guide } from "../services/api/get-guides.api";

interface ResourceScreenProps {
  resource: Guide;
}

const ResourceScreen = ({ resource }: ResourceScreenProps) => {
  const { width } = useWindowDimensions();
  return (
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
            html: resource.text || "",
          }}
          contentWidth={width - 32}
          // @ts-ignore
          tagsStyles={tagsStyles}
        />
      </ScrollView>
    </YStack>
  );
};

export default ResourceScreen;

// TODO: this needs to be moved
const tagsStyles = {
  body: {
    color: "hsl(240, 5%, 34%)",
  },
  p: {
    lineHeight: 24,
  },
  h1: {
    fontSize: 24,
    marginVertical: 16,
  },
  a: {
    color: "hsl(272, 56%, 45%)",
    fontWeight: "700",
    textDecoration: "none",
    // for some reason textDecoration: "none" doesn't seem to work
    textDecorationColor: "transparent",
  },
};
