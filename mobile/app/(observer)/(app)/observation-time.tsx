import React, { useEffect, useMemo, useRef, useState } from "react";
import { Screen } from "../../../components/Screen";
import { useTranslation } from "react-i18next";
import Header from "../../../components/Header";
import { router } from "expo-router";
import { Icon } from "../../../components/Icon";
import { View, XStack, YStack } from "tamagui";
import DateFormInput from "../../../components/FormInputs/DateFormInput";
import WizzardControls from "../../../components/WizzardControls";
import { Controller, useFieldArray, useForm } from "react-hook-form";
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
import { BackHandler, ScrollView } from "react-native";
import WarningDialog from "../../../components/WarningDialog";
import OptionsSheet from "../../../components/OptionsSheet";
import { Typography } from "../../../components/Typography";
import { Dialog } from "../../../components/Dialog";
import { useNetInfoContext } from "../../../contexts/net-info-banner/NetInfoContext";

const ObservationTime = () => {
  const { t } = useTranslation("observation");
  const scrollViewRef = useRef<ScrollView>(null);

  const [breakToDelete, setBreakToDelete] = useState<number | null>(null);
  const [isSaveChangesModalOpen, setIsSaveChangesModalOpen] = useState(false);
  const [isOptionsSheetOpen, setIsOptionsSheetOpen] = useState(false);
  const [isClearingForm, setIsClearingForm] = useState(false);
  const [isUnableToSaveObservationTime, setIsUnableToSaveObservationTime] = useState(false);

  const { selectedPollingStation, activeElectionRound } = useUserData();
  const queryClient = useQueryClient();

  const { data: psiData } = usePollingStationInformation(
    activeElectionRound?.id,
    selectedPollingStation?.pollingStationId,
  );

  const { mutate, isPending } = useMutatePollingStationGeneralData({
    electionRoundId: activeElectionRound?.id,
    pollingStationId: selectedPollingStation?.pollingStationId,
    scopeId: `PS_General_${activeElectionRound?.id}_${selectedPollingStation?.pollingStationId}`,
  });

  const { isOnline } = useNetInfoContext();
  const isLoading = useMemo(() => isPending && isOnline, [isPending, isOnline]);

  const {
    control,
    watch,
    handleSubmit,
    setValue,
    getValues,
    reset,
    formState: { isDirty, errors },
  } = useForm({
    defaultValues: {
      arrivalTime: psiData?.arrivalTime ? new Date(psiData.arrivalTime) : null,
      departureTime: psiData?.departureTime ? new Date(psiData.departureTime) : null,
      breaks: psiData?.breaks
        ? psiData.breaks
            .map((b) => ({
              ...b,
              start: b.start ? new Date(b.start) : null,
              end: b.end ? new Date(b.end) : null,
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

  const {
    fields: breaks,
    append,
    remove,
  } = useFieldArray({
    control,
    name: "breaks",
    rules: {
      validate: {
        breakValid: (breaks) => {
          return breaks.every((breakItem) => {
            if (!breakItem.start || !breakItem.end) return true;
            return breakItem.start <= breakItem.end;
          });
        },
        breakStartAfterArrivalTime: (breaks) => {
          const { arrivalTime } = getValues();
          if (!arrivalTime) return true;
          return breaks.every((breakItem) => {
            if (!breakItem.start) return true;
            return breakItem.start >= arrivalTime;
          });
        },
        breakStartBeforeDepartureTime: (breaks) => {
          const { departureTime } = getValues();
          if (!departureTime) return true;
          return breaks.every((breakItem) => {
            if (!breakItem.start) return true;
            return breakItem.start <= departureTime;
          });
        },
        breakEndAfterArrivalTime: (breaks) => {
          const { arrivalTime } = getValues();
          if (!arrivalTime) return true;
          return breaks.every((breakItem) => {
            if (!breakItem.end) return true;
            return breakItem.end >= arrivalTime;
          });
        },
        breakEndBeforeDepartureTime: (breaks) => {
          const { departureTime } = getValues();
          if (!departureTime) return true;
          return breaks.every((breakItem) => {
            if (!breakItem.end) return true;
            return breakItem.end <= departureTime;
          });
        },
      },
    },
  });

  // reset form data every time the psiData changes, so that the form is no longer dirty after submitting it
  useEffect(() => {
    if (psiData) {
      reset(getValues());
    }
  }, [psiData, getValues, reset]);

  const handleGoBack = () => {
    if (isDirty) {
      setIsSaveChangesModalOpen(true);
    } else {
      router.back();
    }
  };

  useEffect(() => {
    const backHandler = BackHandler.addEventListener("hardwareBackPress", () => {
      handleGoBack();
      return true;
    });

    return () => backHandler.remove();
  }, [isDirty]);

  const handleAddBreak = () => {
    const newBreak = {
      start: null,
      end: null,
    };

    append(newBreak);
    // scroll to the bottom in order to see the newly added break
    setTimeout(() => {
      if (scrollViewRef.current) {
        scrollViewRef.current.scrollToEnd({ animated: true });
      }
    }, 0);
  };

  const handleDeleteBreak = (index: number) => {
    remove(index);
    // close modal
    setBreakToDelete(null);
  };

  const onSubmit = (formData: any) => {
    const definedBreaks = formData.breaks.filter((breakItem: Break) => breakItem.start !== null);
    setValue("breaks", [...definedBreaks]);

    mutate(
      {
        electionRoundId: activeElectionRound?.id,
        pollingStationId: selectedPollingStation?.pollingStationId,
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
    handleSubmit(onSubmit)();
    setIsClearingForm(false);
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
                  disabled={isLoading}
                  maximumDate={watch("departureTime")}
                />
              )}
            />

            <Controller
              name="departureTime"
              control={control}
              rules={{
                validate: {
                  departureTimeAfterArrivalTime: (value) => {
                    if (!value) return true;
                    const { arrivalTime } = getValues();
                    if (!arrivalTime) return true;
                    return value >= arrivalTime;
                  },
                },
              }}
              render={({ field: { onChange, value } }) => (
                <DateFormInput
                  title={t("polling_stations_information.observation_time.departure_time")}
                  placeholder={t(
                    "polling_stations_information.observation_time.select_departure_time",
                  )}
                  onChange={onChange}
                  value={value}
                  disabled={isLoading || !watch("arrivalTime")}
                  error={
                    errors.departureTime?.type === "departureTimeAfterArrivalTime"
                      ? t("polling_stations_information.observation_time.departure_after_arrival")
                      : ""
                  }
                  minimumDate={watch("arrivalTime")}
                />
              )}
            />

            <Button
              preset="chromeless"
              paddingHorizontal={0}
              paddingRight="$lg"
              icon={<Icon icon="coffeeBreak" color="$purple5" size={24} />}
              alignSelf="flex-start"
              disabled={!watch("arrivalTime") || isLoading}
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
                  key={b.id}
                  index={index}
                  control={control}
                  watch={watch}
                  isPending={isLoading}
                  setBreakToDelete={setBreakToDelete}
                  onDelete={handleDeleteBreak}
                />
              );
            })}
          </YStack>
        </ScrollView>
      </YStack>

      <WizzardControls
        isFirstElement
        onActionButtonPress={() => {
          handleSubmit(
            async (data) => {
              await onSubmit(data);
              router.back();
            },
            (errors) => {
              if (Object.keys(errors).includes("breaks")) {
                setIsUnableToSaveObservationTime(true);
              }
            },
          )();
        }}
        actionBtnLabel={t("polling_stations_information.observation_time.save")}
        marginTop="auto"
        isNextDisabled={isLoading}
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
            handleSubmit(
              async (data) => {
                await onSubmit(data);
                router.back();
              },
              (errors) => {
                // close modal in order to see the input error displayed for departureTime
                if (errors.departureTime) {
                  return setIsSaveChangesModalOpen(false);
                }
                if (Object.keys(errors).includes("breaks")) {
                  // close this modal and display the unable to save observation time one
                  setIsSaveChangesModalOpen(false);
                  return setIsUnableToSaveObservationTime(true);
                }
              },
            )();
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
          action={handleClearForm}
        />
      )}
      {isUnableToSaveObservationTime && (
        <Dialog
          open
          content={
            <YStack maxHeight="85%" gap="$md">
              <ScrollView
                contentContainerStyle={{ gap: 16, flexGrow: 1 }}
                showsVerticalScrollIndicator={false}
                bounces={false}
              >
                <Typography preset="heading">
                  {t(
                    "polling_stations_information.observation_time.confirmation_modals.unable_to_save.title",
                  )}
                </Typography>

                <Typography color="$gray6">
                  {t(
                    "polling_stations_information.observation_time.confirmation_modals.unable_to_save.p1",
                  )}
                </Typography>

                <Typography color="$gray6">
                  {t(
                    "polling_stations_information.observation_time.confirmation_modals.unable_to_save.p2",
                  )}
                </Typography>
              </ScrollView>
            </YStack>
          }
          footer={
            <XStack justifyContent="center">
              <Button preset="chromeless" onPress={() => setIsUnableToSaveObservationTime(false)}>
                {t(
                  "polling_stations_information.observation_time.confirmation_modals.unable_to_save.ok",
                )}
              </Button>
            </XStack>
          }
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
