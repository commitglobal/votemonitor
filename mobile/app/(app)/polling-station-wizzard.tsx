import { router } from "expo-router";
import { Screen } from "../../components/Screen";
import Header from "../../components/Header";
import { Icon } from "../../components/Icon";
import { Keyboard, ViewStyle, useWindowDimensions } from "react-native";
import { Input, Spinner, XStack, YStack, styled } from "tamagui";
import { Typography } from "../../components/Typography";
import { pollingStationsKeys, usePollingStationByParentID } from "../../services/queries.service";
import React, { useCallback, useMemo, useState } from "react";
import {
  PollingStationNomenclatorNodeVM,
  PollingStationVisitVM,
} from "../../common/models/polling-station.model";
import { useTranslation } from "react-i18next";
import { useUserData } from "../../contexts/user/UserContext.provider";
import { useQueryClient } from "@tanstack/react-query";
import WizzardControls from "../../components/WizzardControls";
import { ListView } from "../../components/ListView";
import { useSafeAreaInsets } from "react-native-safe-area-context";

const mapPollingStationOptionsToSelectValues = (
  options: PollingStationNomenclatorNodeVM[],
): { id: string | number; value: string; label: string }[] => {
  return options.map((option) => ({
    id: option.id,
    value: `${option.id}_${option.name}`,
    label: option.pollingStationId ? `${option.number} - ${option.name}` : option.name,
  }));
};

type PollingStationStep = {
  id: number;
  name: string;
};

const PollingStationWizzard = () => {
  const { t } = useTranslation("add_polling_station");
  const [steps, setSteps] = useState<PollingStationStep[]>([]);

  const onNextPress = (nextStep: PollingStationStep) => {
    setSteps([...steps, nextStep]);
  };

  const onPreviousPress = (): PollingStationStep | undefined => {
    const updatedSteps = [...steps];
    const lastStep = updatedSteps.pop();
    setSteps(updatedSteps);
    return lastStep;
  };

  const activeStep = useMemo(() => [...steps].pop(), [steps]);

  const locations = useMemo(() => steps.map((step) => step.name).join(", "), [steps]);

  return (
    <Screen backgroundColor="white" contentContainerStyle={$containerStyle} preset="fixed">
      <Header
        title={t("header.title")}
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={router.back}
      />
      <PollingStationWizzardContent
        activeStep={activeStep}
        onPreviousPress={onPreviousPress}
        onNextPress={onNextPress}
        locations={locations}
      />
    </Screen>
  );
};

interface PollingStationWizzardContentProps {
  onPreviousPress: () => PollingStationStep | undefined;
  onNextPress: (nextStep: PollingStationStep) => void;
  activeStep?: PollingStationStep;
  locations: string;
}

const PollingStationWizzardContent = ({
  onPreviousPress,
  onNextPress,
  activeStep,
  locations,
}: PollingStationWizzardContentProps) => {
  const { t } = useTranslation("add_polling_station");
  const [selectedOption, setSelectedOption] = useState<PollingStationStep>();
  const [searchTerm, setSearchTerm] = useState<string>("");
  const [sliceNumber, setSliceNumber] = useState(30);
  const { height } = useWindowDimensions();
  const insets = useSafeAreaInsets();
  const { activeElectionRound, setSelectedPollingStationId } = useUserData();

  const queryClient = useQueryClient();

  const {
    data: pollingStationOptions,
    isLoading: isLoadingPollingStations,
    isFetching: isFetchingPollingStations,
    error: pollingStationsError,
  } = usePollingStationByParentID(
    activeStep?.id ? +activeStep.id.toFixed(1) : -1,
    activeElectionRound?.id,
  );

  const pollingStationsMappedOptions = useMemo(
    () => mapPollingStationOptionsToSelectValues(pollingStationOptions),
    [pollingStationOptions],
  );

  const filteredOptions = useMemo(() => {
    if (!searchTerm) return pollingStationsMappedOptions.slice(0, sliceNumber);
    return pollingStationsMappedOptions
      .filter((option) => option.label.toLowerCase().includes(searchTerm.toLowerCase()))
      .slice(0, sliceNumber);
  }, [pollingStationsMappedOptions, searchTerm, sliceNumber]);

  const loadMore = () => {
    setSliceNumber((sliceNum) => sliceNum + 50);
  };

  const isLastElement: boolean = useMemo(
    () => !!pollingStationOptions[0]?.pollingStationId,
    [pollingStationOptions],
  );

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
      onNextPress(selectedOption);
      setSearchTerm("");
      setSliceNumber(30);
      setSelectedOption(undefined);
    }
  };

  const onBackButtonPress = () => {
    const lastStep = onPreviousPress();
    setSelectedOption(lastStep);
    setSearchTerm("");
  };

  const onFinishButtonPress = async () => {
    if (!selectedOption) {
      return;
    }
    const pollingStation = pollingStationOptions.find((option) => option.id === selectedOption.id);

    if (pollingStation?.pollingStationId && activeElectionRound) {
      await queryClient.cancelQueries({
        queryKey: pollingStationsKeys.visits(activeElectionRound.id),
      });
      let previousData =
        queryClient.getQueryData<PollingStationVisitVM[]>(
          pollingStationsKeys.visits(activeElectionRound.id),
        ) ?? [];

      // Remove the pollingStation if already exists, to be added again as new visit and prevent duplicates
      previousData = previousData.filter((item) => {
        if (item.pollingStationId === pollingStation.pollingStationId) {
          return false;
        }
        return true;
      });

      queryClient.setQueryData<PollingStationVisitVM[]>(
        pollingStationsKeys.visits(activeElectionRound.id),
        [
          ...previousData,
          {
            pollingStationId: pollingStation.pollingStationId,
            visitedAt: new Date().toISOString(),
            address: pollingStation.name,
            number: pollingStation?.number || "",
          },
        ],
      );

      setSelectedPollingStationId(null);
    }

    router.back();
  };

  // TODO: To be handled
  if (isLoadingPollingStations) {
    return <Typography>Loading...</Typography>;
  }

  // TODO: To be handled
  if (pollingStationsError) {
    return <Typography>Something went wrong!</Typography>;
  }

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

  return (
    <>
      <YStack paddingHorizontal="$md" gap={"$1"}>
        {activeStep && (
          <YStack paddingTop={"$sm"} minHeight="$xl">
            <Typography color="$gray5">{t("progress.location", { value: locations })}</Typography>
          </YStack>
        )}

        <XStack backgroundColor="$purple1" marginTop={"$sm"} borderRadius={8} alignItems="center">
          <Icon icon="search" color="transparent" size={20} marginLeft="$sm" />
          <SearchInput flex={1} value={searchTerm} onChangeText={setSearchTerm} />
        </XStack>
      </YStack>
      <YStack
        paddingHorizontal="$md"
        paddingTop="$sm"
        height={height - 300 - insets.top - insets.bottom}
        paddingBottom={"$md"}
      >
        {isFetchingPollingStations && <Spinner size="large" color="$purple5" />}
        {!isFetchingPollingStations && (
          <ListView<{ id: string | number; value: string; label: string }>
            data={filteredOptions}
            showsVerticalScrollIndicator={false}
            bounces={false}
            estimatedItemSize={64}
            extraData={selectedOption}
            ListEmptyComponent={<Typography>{t("list.empty")}</Typography>}
            keyExtractor={(item) => item.value}
            onEndReached={loadMore}
            onEndReachedThreshold={0.5}
            renderItem={SelectItem}
          />
        )}
      </YStack>
      <WizzardControls
        isFirstElement={!activeStep?.id}
        isLastElement={isLastElement}
        onPreviousButtonPress={onBackButtonPress}
        isNextDisabled={!selectedOption}
        onNextButtonPress={isLastElement ? onFinishButtonPress : onNextButtonPress}
      />
    </>
  );
};

const $containerStyle: ViewStyle = {
  flex: 1,
  justifyContent: "space-between",
};

const SearchInput = styled(Input, {
  backgroundColor: "$purple1",
  placeholder: "Search",
  color: "$purple5",
  placeholderTextColor: "$purple5",
  focusStyle: {
    borderColor: "transparent",
  },
});

export default PollingStationWizzard;
