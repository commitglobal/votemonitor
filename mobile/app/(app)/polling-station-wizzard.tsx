import { router } from "expo-router";
import { Screen } from "../../components/Screen";
import Header from "../../components/Header";
import { Icon } from "../../components/Icon";
import { TextStyle, ViewStyle } from "react-native";
import { XStack, YStack } from "tamagui";
import { Typography } from "../../components/Typography";
import Select from "../../components/Select";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import Button from "../../components/Button";
import { pollingStationsKeys, usePollingStationByParentID } from "../../services/queries.service";
import { useMemo, useState } from "react";
import { PollingStationNomenclatorNodeVM } from "../../common/models/polling-station.model";
import { useTranslation } from "react-i18next";
import { useUserData } from "../../contexts/user/UserContext.provider";
import { useQueryClient } from "@tanstack/react-query";
import { PollingStationVisitsAPIResponse } from "../../services/definitions.api";

const mapPollingStationOptionsToSelectValues = (
  options: PollingStationNomenclatorNodeVM[],
): { id: string | number; value: string; label: string }[] => {
  return options.map((option) => ({
    id: option.id,
    value: `${option.id}_${option.name}`,
    label: option.name,
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
    <Screen style={$screenStyle} contentContainerStyle={$containerStyle} preset="fixed">
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
  const insets = useSafeAreaInsets();
  const [selectedOption, setSelectedOption] = useState<PollingStationStep>();
  const { electionRounds } = useUserData();

  const queryClient = useQueryClient();

  const {
    data: pollingStationOptions,
    isLoading: isLoadingPollingStations,
    isFetching: isFetchingPollingStations,
    error: pollingStationsError,
  } = usePollingStationByParentID(activeStep?.id ? +activeStep.id.toFixed(1) : -1);

  const pollingStationsMappedOptions = useMemo(
    () => mapPollingStationOptionsToSelectValues(pollingStationOptions),
    [pollingStationOptions],
  );

  const isLastElement: boolean = useMemo(
    () => !!pollingStationOptions[0]?.pollingStationId,
    [pollingStationOptions],
  );

  const onSelectOption = (option: string) => {
    const [id, value] = option.split("_");
    setSelectedOption({
      id: +id,
      name: value,
    });
  };

  const onNextButtonPress = () => {
    if (selectedOption) {
      onNextPress(selectedOption);
      setSelectedOption(undefined);
    }
  };

  const onBackButtonPress = () => {
    const lastStep = onPreviousPress();
    setSelectedOption(lastStep);
  };

  const onFinishButtonPress = () => {
    if (!selectedOption) {
      return;
    }
    const pollingStation = pollingStationOptions.find((option) => option.id === selectedOption.id);

    if (pollingStation?.pollingStationId) {
      queryClient.setQueryData(
        pollingStationsKeys.visits(electionRounds[0].id),
        (current: PollingStationVisitsAPIResponse) => {
          console.log(current);
          return {
            visits: [
              ...current.visits,
              {
                pollingStationId: pollingStation?.pollingStationId,
                visitedAt: new Date().toISOString(),
              },
            ],
          };
        },
      );
    }

    router.back();
  };

  // TODO: To be handled
  if (isLoadingPollingStations) {
    return <Typography>Loading...</Typography>;
  }

  // TODO: To be handled
  if (pollingStationsError) {
    return <Typography>Error handling here...</Typography>;
  }

  return (
    <YStack style={$containerStyle} justifyContent="space-between">
      <YStack paddingHorizontal="$md" paddingTop="$xl">
        <YStack gap="$md" minHeight="$xxl">
          {activeStep && (
            <Typography color="$gray5">{t("progress.location", { value: locations })}</Typography>
          )}
        </YStack>
        <YStack paddingTop={140} gap="$lg" justifyContent="center">
          <Typography preset="body2" style={$labelStyle}>
            {t("form.region.title")}
          </Typography>
          {!isFetchingPollingStations && (
            <Select
              key={activeStep?.id}
              options={pollingStationsMappedOptions}
              placeholder={t("form.region.placeholder")}
              onValueChange={onSelectOption}
              value={selectedOption ? `${selectedOption.id}_${selectedOption.name}` : undefined}
            />
          )}
        </YStack>
      </YStack>
      <XStack
        elevation={2}
        backgroundColor="white"
        gap="$sm"
        padding="$md"
        paddingBottom={insets.bottom}
      >
        {activeStep?.id && (
          <XStack flex={0.25}>
            <Button
              width="100%"
              icon={() => <Icon icon="chevronLeft" color="$purple5" />}
              preset="chromeless"
              onPress={onBackButtonPress}
            >
              {t("actions.back")}
            </Button>
          </XStack>
        )}
        <XStack flex={!activeStep?.id ? 1 : 0.75} marginBottom="$md">
          {!isLastElement && (
            <Button disabled={!selectedOption} width="100%" onPress={onNextButtonPress}>
              {t("actions.next_step")}
            </Button>
          )}
          {isLastElement && (
            <Button width="100%" onPress={onFinishButtonPress}>
              {t("actions.finalize")}
            </Button>
          )}
        </XStack>
      </XStack>
    </YStack>
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
