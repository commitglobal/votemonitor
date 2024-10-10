import React, { useMemo, useState } from "react";
import { Screen } from "../../components/Screen";
import Header from "../../components/Header";
import { Icon } from "../../components/Icon";
import { Typography } from "../../components/Typography";
import { useTranslation } from "react-i18next";
import { ScrollView, Spinner, XStack, YStack } from "tamagui";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import Button from "../../components/Button";
import { Selector } from "../../components/Selector";
import { useGetCitizenElectionRounds } from "../../services/queries/citizen.query";
import { useCitizenUserData } from "../../contexts/citizen-user/CitizenUserContext.provider";
import { router } from "expo-router";

const Footer = ({
  primaryAction,
  primaryActionLabel,
  isPrimaryButtonDisabled,
  handleGoBack,
}: {
  primaryAction?: () => void;
  primaryActionLabel: string;
  isPrimaryButtonDisabled?: boolean;
  handleGoBack: () => void;
}) => {
  const insets = useSafeAreaInsets();

  return (
    <XStack
      marginBottom={insets.bottom + 32}
      justifyContent="center"
      alignItems="center"
      paddingRight="$xl"
    >
      <XStack
        justifyContent="center"
        alignItems="center"
        height="100%"
        flex={0.2}
        pressStyle={{ opacity: 0.5 }}
        paddingLeft="$xl"
        onPress={handleGoBack}
      >
        <Icon icon="chevronLeft" size={24} color="$purple5" />
      </XStack>
      <Button
        flex={0.8}
        disabled={isPrimaryButtonDisabled || !primaryAction}
        onPress={primaryAction}
      >
        {primaryActionLabel}
      </Button>
    </XStack>
  );
};

const SelectElectionEvent = () => {
  const { t } = useTranslation("select_election_event");

  const { selectedElectionRound, setSelectedElectionRound } = useCitizenUserData();
  const [selectedElectionRoundLocal, setSelectedElectionRoundLocal] = useState<string | null>(
    selectedElectionRound,
  );

  const {
    data: electionEvents,
    isLoading: isLoadingElectionEvents,
    isError: isErrorElectionEvents,
    refetch: refetchElectionEvents,
    isRefetching: isRefetchingElectionEvents,
  } = useGetCitizenElectionRounds();

  const handleGoBack = () => {
    router.push("select-app-mode");
  };

  const renderHeader = useMemo(() => {
    return () => (
      <Header barStyle="light-content" backgroundColor="white">
        <XStack paddingTop="$xxl" justifyContent="center" alignItems="center">
          <Icon icon="vmCitizenLogo" width={295} height={82} />
        </XStack>
        <Typography preset="heading" fontWeight="500" marginVertical="$xl">
          {t("heading")}
        </Typography>
      </Header>
    );
  }, []);

  // error state
  if (isErrorElectionEvents && !isLoadingElectionEvents) {
    return (
      <Screen
        preset="fixed"
        contentContainerStyle={{
          flex: 1,
          backgroundColor: "white",
        }}
      >
        {renderHeader()}

        <YStack flex={1}>
          <ScrollView
            showsVerticalScrollIndicator={false}
            contentContainerStyle={{
              flexGrow: 1,
              paddingHorizontal: "$xl",
              flexDirection: "column",
              alignItems: "center",
              gap: 64,
              backgroundColor: "white",
            }}
          >
            <Typography textAlign="center" color="$gray8">
              {t("error_description")}
            </Typography>
            <Icon icon="warning" color="$purple5" size={100} />
          </ScrollView>
        </YStack>

        <Footer
          primaryAction={refetchElectionEvents}
          primaryActionLabel={t("retry")}
          isPrimaryButtonDisabled={isRefetchingElectionEvents}
          handleGoBack={handleGoBack}
        />
      </Screen>
    );
  }

  //   empty state
  if (!isLoadingElectionEvents && (!electionEvents || electionEvents.length === 0)) {
    return (
      <Screen
        preset="fixed"
        contentContainerStyle={{
          flex: 1,
          backgroundColor: "white",
        }}
      >
        {renderHeader()}

        <YStack flex={1} marginBottom="$xl">
          <ScrollView
            showsVerticalScrollIndicator={false}
            contentContainerStyle={{
              flexGrow: 1,
              paddingHorizontal: "$xl",
              flexDirection: "column",
              alignItems: "center",
              gap: 32,
              backgroundColor: "white",
            }}
          >
            <Typography textAlign="center" color="$gray8">
              {t("empty_description")}
            </Typography>
            <Icon icon="peopleAddingVote" />
          </ScrollView>
        </YStack>

        {/* //todo: continue */}
        <Footer primaryActionLabel={t("continue")} handleGoBack={handleGoBack} />
      </Screen>
    );
  }

  return (
    <Screen
      preset="fixed"
      contentContainerStyle={{
        flex: 1,
        backgroundColor: "white",
      }}
    >
      {renderHeader()}

      <YStack flex={1} paddingBottom="$md">
        <ScrollView
          showsVerticalScrollIndicator={false}
          contentContainerStyle={{
            gap: 16,
            flexDirection: "column",
            paddingHorizontal: "$xl",
            flexGrow: 1,
          }}
        >
          {isLoadingElectionEvents ? (
            <YStack justifyContent="center" alignItems="center" flex={1}>
              <Spinner color="$purple5" />
            </YStack>
          ) : (
            <>
              <Typography color="$gray8">{t("description")}</Typography>

              {electionEvents?.map((event) => (
                <Selector
                  key={event.id}
                  description={event.title}
                  displayMode="light"
                  selected={selectedElectionRoundLocal === event.id}
                  onPress={() => setSelectedElectionRoundLocal(event.id)}
                />
              ))}
            </>
          )}
        </ScrollView>
      </YStack>

      <Footer
        primaryActionLabel={t("continue")}
        isPrimaryButtonDisabled={!selectedElectionRoundLocal}
        primaryAction={() => {
          if (selectedElectionRoundLocal) {
            setSelectedElectionRound(selectedElectionRoundLocal, {
              onSuccess: () => {
                router.push("citizen/main");
              },
            });
          }
        }}
        handleGoBack={handleGoBack}
      />
    </Screen>
  );
};

export default SelectElectionEvent;
