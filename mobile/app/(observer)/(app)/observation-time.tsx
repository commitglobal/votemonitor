import React, { useEffect, useRef, useState } from "react";
import { Screen } from "../../../components/Screen";
import { useTranslation } from "react-i18next";
import Header from "../../../components/Header";
import { router } from "expo-router";
import { Icon } from "../../../components/Icon";
import { View, YStack } from "tamagui";
import DateFormInput from "../../../components/FormInputs/DateFormInput";
import WizzardControls from "../../../components/WizzardControls";
import { Controller, useForm } from "react-hook-form";
import Button from "../../../components/Button";
import { useUserData } from "../../../contexts/user/UserContext.provider";
import {
  pollingStationsKeys,
  usePollingStationInformation,
} from "../../../services/queries.service";
import { BreakItem } from "../../../components/BreakItem";
import { Break } from "../../../services/definitions.api";
import { useMutatePollingStationGeneralData } from "../../../services/mutations/psi-general.mutation";
import { useQueryClient } from "@tanstack/react-query";
import { ScrollView } from "react-native";
import WarningDialog from "../../../components/WarningDialog";
import OptionsSheet from "../../../components/OptionsSheet";
import { Typography } from "../../../components/Typography";

const ObservationTime = () => {
  const { t } = useTranslation("observation");
  const scrollViewRef = useRef<ScrollView>(null);

  const [breakToDelete, setBreakToDelete] = useState<number | null>(null);
  const [isSaveChangesModalOpen, setIsSaveChangesModalOpen] = useState(false);
  const [isOptionsSheetOpen, setIsOptionsSheetOpen] = useState(false);
  const [isClearingForm, setIsClearingForm] = useState(false);

  const { selectedPollingStation, activeElectionRound } = useUserData();
  const queryClient = useQueryClient();

  const { data: psiData } = usePollingStationInformation(
    activeElectionRound?.id,
    selectedPollingStation?.pollingStationId,
  );

  const { mutate, isPending } = useMutatePollingStationGeneralData({
    electionRoundId: activeElectionRound?.id,
    pollingStationId: selectedPollingStation?.pollingStationId,
    scopeId: `PS_General_${activeElectionRound?.id}_${selectedPollingStation?.pollingStationId}_dates`,
  });

  const {
    control,
    watch,
    handleSubmit,
    setValue,
    getValues,
    reset,
    formState: { isDirty },
  } = useForm({
    defaultValues: {
      arrivalTime: psiData?.arrivalTime ? new Date(psiData.arrivalTime) : undefined,
      departureTime: psiData?.departureTime ? new Date(psiData.departureTime) : undefined,
      breaks: psiData?.breaks
        ? psiData.breaks
            .map((b) => ({
              ...b,
              start: b.start ? new Date(b.start) : undefined,
              end: b.end ? new Date(b.end) : undefined,
            }))
            .sort((date1, date2) => {
              if (date1.start && date2.start) {
                return date1.start > date2.start ? 1 : -1;
              }
              return 0;
            })
        : [],
    },
  });

  // reset form data every time the psiData changes, so that the form is no longer dirty after submitting it
  useEffect(() => {
    if (psiData) {
      reset(getValues());
    }
  }, [psiData, getValues, reset]);

  const breaks = watch("breaks");

  const handleGoBack = () => {
    if (isDirty) {
      setIsSaveChangesModalOpen(true);
    } else {
      router.back();
    }
  };

  const handleAddBreak = () => {
    const newBreak = {
      start: undefined,
      end: undefined,
    };

    setValue("breaks", [...breaks, newBreak], { shouldDirty: true });

    // scroll to the bottom in order to see the newly added break
    setTimeout(() => {
      if (scrollViewRef.current) {
        scrollViewRef.current.scrollToEnd({ animated: true });
      }
    }, 0);
  };

  const handleDeleteBreak = (index: number) => {
    const updatedBreaks = breaks.filter((_, i) => i !== index);
    setValue("breaks", [...updatedBreaks], { shouldDirty: true });
    // close modal
    setBreakToDelete(null);
  };

  const onSubmit = (formData: any) => {
    const definedBreaks = formData.breaks.filter(
      (breakItem: Break) => breakItem.start !== undefined,
    );
    setValue("breaks", [...definedBreaks]);

    mutate(
      {
        electionRoundId: activeElectionRound?.id,
        pollingStationId: selectedPollingStation?.pollingStationId,
        answers: psiData?.answers || [], // we need to send the answers in the request body, otherwise the API will ignore the new changes
        ...formData,
        breaks: definedBreaks,
      },

      {
        onSettled: () => {
          queryClient.invalidateQueries({
            queryKey: pollingStationsKeys.pollingStationInformation(
              activeElectionRound?.id,
              selectedPollingStation?.pollingStationId,
            ),
          });
        },
      },
    );
  };

  const handleClearForm = () => {
    reset({
      arrivalTime: null,
      departureTime: null,
      breaks: [],
    });
  };

  return (
    <Screen
      preset="fixed"
      contentContainerStyle={{
        flex: 1,
        backgroundColor: "white",
      }}
    >
      <Header
        title={t("polling_stations_information.observation_time.observation_time")}
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={handleGoBack}
        rightIcon={<Icon icon="dotsVertical" color="white" />}
        onRightPress={() => setIsOptionsSheetOpen(true)}
      />
      <YStack flex={1} paddingVertical="$lg">
        <ScrollView
          ref={scrollViewRef}
          showsVerticalScrollIndicator={false}
          bounces={false}
          contentContainerStyle={{
            gap: 56,
            paddingHorizontal: 16,
          }}
        >
          <YStack gap="$lg">
            <Controller
              name="arrivalTime"
              control={control}
              render={({ field: { onChange, value } }) => (
                <DateFormInput
                  title={t("polling_stations_information.observation_time.arrival_time")}
                  placeholder={t(
                    "polling_stations_information.observation_time.select_arrival_time",
                  )}
                  onChange={onChange}
                  value={value}
                  disabled={isPending}
                />
              )}
            />

            <Controller
              name="departureTime"
              control={control}
              render={({ field: { onChange, value } }) => (
                <DateFormInput
                  title={t("polling_stations_information.observation_time.departure_time")}
                  placeholder={t(
                    "polling_stations_information.observation_time.select_departure_time",
                  )}
                  onChange={onChange}
                  value={value}
                  disabled={isPending}
                />
              )}
            />

            <Button
              preset="chromeless"
              paddingHorizontal={0}
              paddingRight="$lg"
              icon={<Icon icon="coffeeBreak" color="$purple5" size={24} />}
              alignSelf="flex-start"
              disabled={!watch("arrivalTime") || isPending}
              onPress={handleAddBreak}
            >
              {t("polling_stations_information.observation_time.add_break")}
            </Button>
          </YStack>

          {/* BREAKS SECTION */}
          <YStack gap="$xl">
            {breaks.map((b, index) => {
              return (
                <BreakItem
                  key={index}
                  control={control}
                  index={index}
                  onDelete={handleDeleteBreak}
                  watch={watch}
                  isPending={isPending}
                  setBreakToDelete={setBreakToDelete}
                />
              );
            })}
          </YStack>
        </ScrollView>
      </YStack>

      <WizzardControls
        isFirstElement
        onActionButtonPress={handleSubmit(onSubmit)}
        actionBtnLabel={t("polling_stations_information.observation_time.save")}
        marginTop="auto"
        isNextDisabled={isPending}
      />

      {breakToDelete !== null && (
        <WarningDialog
          title={t(
            "polling_stations_information.observation_time.confirmation_modals.delete.title",
          )}
          description={t(
            "polling_stations_information.observation_time.confirmation_modals.delete.description",
          )}
          actionBtnText={t(
            "polling_stations_information.observation_time.confirmation_modals.delete.actions.delete",
          )}
          cancelBtnText={t(
            "polling_stations_information.observation_time.confirmation_modals.delete.actions.cancel",
          )}
          onCancel={() => setBreakToDelete(null)}
          action={() => handleDeleteBreak(breakToDelete)}
        />
      )}
      {isSaveChangesModalOpen && (
        <WarningDialog
          theme="info"
          title={t(
            "polling_stations_information.observation_time.confirmation_modals.unsaved_changes.title",
          )}
          description={t(
            "polling_stations_information.observation_time.confirmation_modals.unsaved_changes.description",
          )}
          actionBtnText={t(
            "polling_stations_information.observation_time.confirmation_modals.unsaved_changes.actions.save",
          )}
          cancelBtnText={t(
            "polling_stations_information.observation_time.confirmation_modals.unsaved_changes.actions.discard",
          )}
          onCancel={() => {
            setIsSaveChangesModalOpen(false);
            reset();
            router.back();
          }}
          action={() => {
            handleSubmit(onSubmit)();
            setIsSaveChangesModalOpen(false);
            router.back();
          }}
        />
      )}
      {isOptionsSheetOpen && (
        <OptionsSheet open setOpen={setIsOptionsSheetOpen}>
          <OptionSheetContent
            onClear={() => {
              setIsOptionsSheetOpen(false);
              setIsClearingForm(true);
            }}
          />
        </OptionsSheet>
      )}
      {isClearingForm && (
        <WarningDialog
          title={t("polling_stations_information.observation_time.confirmation_modals.clear.title")}
          description={t(
            "polling_stations_information.observation_time.confirmation_modals.clear.description",
          )}
          actionBtnText={t(
            "polling_stations_information.observation_time.confirmation_modals.clear.actions.clear",
          )}
          cancelBtnText={t(
            "polling_stations_information.observation_time.confirmation_modals.clear.actions.cancel",
          )}
          onCancel={() => setIsClearingForm(false)}
          action={() => {
            setIsClearingForm(false);
            handleClearForm();
          }}
        />
      )}
    </Screen>
  );
};

const OptionSheetContent = ({ onClear }: { onClear: () => void }) => {
  const { t } = useTranslation("observation");

  return (
    <View paddingVertical="$xxs" paddingHorizontal="$sm">
      <Typography preset="body1" color="$gray7" lineHeight={24} onPress={onClear}>
        {t("polling_stations_information.observation_time.clear")}
      </Typography>
    </View>
  );
};

export default ObservationTime;
