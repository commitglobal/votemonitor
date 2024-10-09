import { Input, Spinner, styled, useWindowDimensions, XStack, YStack } from "tamagui";
import { Screen } from "../../../components/Screen";
import Header from "../../../components/Header";
import { Icon } from "../../../components/Icon";
import { router, useLocalSearchParams } from "expo-router";
import i18n from "../../../common/config/i18n";
import { Typography } from "../../../components/Typography";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { ListView } from "../../../components/ListView";
import WizzardControls from "../../../components/WizzardControls";
import { useCallback, useMemo, useState } from "react";
import { useTranslation } from "react-i18next";
import { useGetCitizenLocationsByParentId } from "../../../services/queries/citizen.query";
import { useCitizenUserData } from "../../../contexts/citizen-user/CitizenUserContext.provider";
import { CitizenLocationVM } from "../../../common/models/citizen-locations.model";
import { Keyboard, ViewStyle } from "react-native";

type LocationSelectionStep = {
  id: number;
  name: string;
};

const mapCitizenLocationsToSelectValues = (
  options: CitizenLocationVM[],
): { id: string | number; value: string; label: string }[] => {
  return options.map((option) => ({
    id: option.id,
    value: `${option.id}_${option.name}`,
    label: option.name,
  }));
};

export default function CitizenSelectLocation() {
  const { formId, questionId } = useLocalSearchParams<{ formId: string; questionId: string }>();

  if (!formId || !questionId) {
    return <Typography>CitizenSelectLocation - Incorrect page params</Typography>;
  }

  console.log("👀 [CitizenSelectLocation] formId", formId);

  const { t } = useTranslation(["add_polling_station", "common"]); // TODO: change to citizen

  const { height } = useWindowDimensions();
  const insets = useSafeAreaInsets();

  const { selectedElectionRound } = useCitizenUserData();

  const [steps, setSteps] = useState<LocationSelectionStep[]>([]);
  const [searchTerm, setSearchTerm] = useState<string>("");
  const [sliceNumber, setSliceNumber] = useState(30);
  const [selectedOption, setSelectedOption] = useState<LocationSelectionStep>();

  const activeStep = useMemo(() => [...steps].pop(), [steps]);

  const locations = useMemo(
    () =>
      [...steps, selectedOption]
        .filter((step) => step !== undefined)
        .map((step) => step.name)
        .join(", "),
    [steps, selectedOption],
  );

  const {
    data: citizenLocations,
    isFetching: isFetchingCitizenLocations,
    error: citizenLocationsError,
  } = useGetCitizenLocationsByParentId(
    activeStep?.id ? +activeStep.id.toFixed(1) : -1,
    selectedElectionRound,
  );

  const filteredOptions = useMemo(() => {
    const options = mapCitizenLocationsToSelectValues(citizenLocations);
    if (!searchTerm) return options.slice(0, sliceNumber);
    return options
      .filter((option) => option.label.toLowerCase().includes(searchTerm.toLowerCase()))
      .slice(0, sliceNumber);
  }, [citizenLocations, searchTerm, sliceNumber]);

  const isLastElement: boolean = useMemo(
    () => !!citizenLocations[0]?.locationId,
    [citizenLocations],
  );

  const loadMore = () => {
    setSliceNumber((sliceNum) => sliceNum + 50);
  };

  const onSelectOption = (option: string) => {
    Keyboard.dismiss();
    const [id, value] = option.split("_");
    setSelectedOption({
      id: +id,
      name: value,
    });
  };

  const onNextButtonPress = () => {
    if (selectedOption) {
      setSteps([...steps, selectedOption]);
      setSearchTerm("");
      setSliceNumber(30);
      setSelectedOption(undefined);
    }
  };

  const onBackButtonPress = () => {
    const updatedSteps = [...steps];
    const lastStep = updatedSteps.pop();
    setSteps(updatedSteps);
    setSelectedOption(lastStep);
    setSearchTerm("");
  };

  const onFinishButtonPress = async () => {
    const selectedLocationId = citizenLocations.find(
      (location) => location.id === selectedOption?.id,
    )?.locationId;
    router.push(
      `citizen/main/form/?formId=${formId}&selectedLocationId=${selectedLocationId}&questionId=${questionId}`,
    );
  };

  const SelectItem = useCallback(
    ({ item, extraData }: any) => {
      return (
        <Typography
          style={{ borderRadius: 8 }}
          padding="$xs"
          backgroundColor={
            `${extraData?.id}_${extraData?.name}` === item.value ? "$purple1" : "white"
          }
          pressStyle={{ opacity: 0.85, backgroundColor: "$purple1" }}
          onPress={() => onSelectOption(item.value)}
        >
          {item.label}
        </Typography>
      );
    },
    [selectedOption],
  );

  if (citizenLocationsError) {
    return <Typography>{t("error")}</Typography>;
  }

  return (
    <Screen backgroundColor="white" contentContainerStyle={$containerStyle} preset="fixed">
      <Header
        title={"Select location"}
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={router.back}
      />
      <YStack paddingHorizontal="$md" gap={"$1"}>
        {locations && (
          <YStack paddingTop={"$sm"} minHeight="$xl">
            <Typography color="$gray5">{t("progress.location", { value: locations })}</Typography>
          </YStack>
        )}

        <XStack backgroundColor="$purple1" marginTop={"$sm"} borderRadius={8} alignItems="center">
          <Icon icon="search" color="transparent" size={20} marginLeft="$sm" />
          <SearchInput
            flex={1}
            value={searchTerm}
            onChangeText={setSearchTerm}
            maxFontSizeMultiplier={1.2}
          />
        </XStack>
      </YStack>
      <YStack
        paddingHorizontal="$md"
        paddingTop="$sm"
        height={height - 300 - insets.top - insets.bottom}
        style={{ flex: 1, flexGrow: 1, flexDirection: "row" }}
        paddingBottom={"$md"}
      >
        {isFetchingCitizenLocations && <Spinner size="large" color="$purple5" />}
        {!isFetchingCitizenLocations && (
          <ListView<{ id: string | number; value: string; label: string }>
            data={filteredOptions}
            showsVerticalScrollIndicator={false}
            bounces={false}
            estimatedItemSize={64}
            extraData={selectedOption}
            ListEmptyComponent={<Typography>{t("no_data", { ns: "common" })}</Typography>}
            keyExtractor={(item) => item.value}
            onEndReached={loadMore}
            onEndReachedThreshold={0.5}
            renderItem={SelectItem}
            keyboardShouldPersistTaps="handled"
          />
        )}
      </YStack>
      <WizzardControls
        isFirstElement={!activeStep?.id}
        isLastElement={isLastElement}
        onPreviousButtonPress={onBackButtonPress}
        isNextDisabled={!selectedOption}
        onActionButtonPress={isLastElement ? onFinishButtonPress : onNextButtonPress}
      />
    </Screen>
  );
}

const $containerStyle: ViewStyle = {
  flex: 1,
  justifyContent: "space-between",
};

const SearchInput = styled(Input, {
  backgroundColor: "$purple1",
  placeholder: i18n.t("search", { ns: "common" }),
  color: "$purple5",
  placeholderTextColor: "$purple5",
  focusStyle: {
    borderColor: "transparent",
  },
});
