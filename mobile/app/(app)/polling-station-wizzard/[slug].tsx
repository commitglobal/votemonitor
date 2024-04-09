import { router, useLocalSearchParams } from "expo-router";
import { Screen } from "../../../components/Screen";
import Header from "../../../components/Header";
import { Icon } from "../../../components/Icon";
import { TextStyle, ViewStyle } from "react-native";
import { XStack, YStack } from "tamagui";
import { Typography } from "../../../components/Typography";
import Select from "../../../components/Select";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import Button from "../../../components/Button";
import { usePollingStationByParentID } from "../../../services/queries.service";
import { useMemo, useState } from "react";
import { PollingStationNomenclatorNodeVM } from "../../../common/models/polling-station.model";

const mapPollingStationOptionsToSelectValues = (
  options: PollingStationNomenclatorNodeVM[],
): { id: string | number; value: string; label: string }[] => {
  return options.map((option) => ({
    id: option.id,
    value: `${option.id}_${option.name}`,
    label: option.name,
  }));
};

const PollingStationWizzard = () => {
  const insets = useSafeAreaInsets();
  // Slug will come as -1 or {id}_{name} format for this screen
  const { slug, parentId, locations } = useLocalSearchParams();
  // selection state initialized by search params
  const [selectedOption, setSelectedOption] = useState<string | undefined>(
    (slug as string) || undefined,
  );

  // Call polling station
  const { data: pollingStationOptions } = usePollingStationByParentID(slug ? +slug : -1);

  const isLastElement: boolean = useMemo(
    () => !!pollingStationOptions[0]?.pollingStationId,
    [pollingStationOptions],
  );

  const onNextButtonPress = () => {
    const composedValue = selectedOption?.split("_") || [-1, ""];
    router.replace({
      pathname: `/polling-station-wizzard/${composedValue[0]}`,
      params: {
        parentId: pollingStationOptions[0].parentId,
        locations: locations ? `${locations}, ${composedValue[1]}` : composedValue[1],
      },
    });
  };

  const onBackButtonPress = () => {
    // Since all elements have the same parentId we go back to the first element
    const search = pollingStationOptions[0].parentId || -1;

    const locationArray = (locations as string).split(", ");
    locationArray.pop();
    const updatedLocations = locationArray.join(", ");

    router.replace({
      pathname: `/polling-station-wizzard/${search}`,
      params: {
        parentId: pollingStationOptions[0].parentId,
        locations: updatedLocations,
      },
    });
  };

  const onFinishButtonPress = () => {
    console.log("on finish button press");
  };

  return (
    <Screen style={$screenStyle} contentContainerStyle={$containerStyle} preset="fixed">
      <Header
        title={"Add polling station"}
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={router.back}
      />
      <YStack style={$containerStyle} justifyContent="space-between">
        <YStack paddingHorizontal="$md" paddingTop="$xl">
          <YStack gap="$md" minHeight="$xl">
            {parentId && (
              <Typography color="$gray5">{`Add polling station from the ${locations}`}</Typography>
            )}
          </YStack>
          <YStack paddingTop={140} gap="$lg" justifyContent="center">
            <Typography preset="body2" style={$labelStyle}>
              Select the [Location - L1] of the polling station
            </Typography>
            <Select
              options={mapPollingStationOptionsToSelectValues(pollingStationOptions)}
              placeholder="Select Region"
              onValueChange={setSelectedOption}
              defaultValue={selectedOption}
            />
          </YStack>
        </YStack>
        <XStack
          elevation={2}
          backgroundColor="white"
          gap="$sm"
          padding="$md"
          paddingBottom={insets.bottom}
        >
          {parentId && (
            <XStack flex={0.25}>
              <Button
                width="100%"
                icon={() => <Icon icon="chevronLeft" color="$purple5" />}
                preset="chromeless"
                onPress={onBackButtonPress}
              >
                Back
              </Button>
            </XStack>
          )}
          <XStack flex={!parentId ? 1 : 0.75}>
            {!isLastElement && (
              <Button width="100%" onPress={onNextButtonPress}>
                Next Step
              </Button>
            )}
            {isLastElement && (
              <Button width="100%" onPress={onFinishButtonPress}>
                Finalize
              </Button>
            )}
          </XStack>
        </XStack>
      </YStack>
    </Screen>
  );
};

const $screenStyle: ViewStyle = {
  backgroundColor: "white",
  justifyContent: "space-between",
};

const $containerStyle: ViewStyle = {
  flex: 1,
};

const $labelStyle: TextStyle = {
  color: "black",
  fontWeight: "700",
};

export default PollingStationWizzard;
