import React, { useMemo, useState } from "react";
import { Screen } from "../components/Screen";
import Header from "../components/Header";
import { Icon } from "../components/Icon";
import { Typography } from "../components/Typography";
import { useTranslation } from "react-i18next";
import { ScrollView, Spinner, XStack, YStack } from "tamagui";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import Button from "../components/Button";
import { Selector } from "../components/Selector";
import { useGetCitizenElectionEvents } from "../services/queries/citizen.query";
import { IElectionRound } from "../services/api/citizen/get-citizen-election-rounds";

const Footer = ({
  primaryAction,
  primaryActionLabel,
  isPrimaryButtonDisabled,
  handleGoBack,
}: {
  primaryAction: () => void;
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
        <Icon icon="chevronLeft" size={24} />
      </XStack>
      <Button flex={0.8} disabled={isPrimaryButtonDisabled} onPress={primaryAction}>
        {primaryActionLabel}
      </Button>
    </XStack>
  );
};

export const SelectElectionEvent = () => {
  const { t } = useTranslation("select_election_event");

  const [selectedElectionEvent, setSelectedElectionEvent] = useState<IElectionRound | null>(null);

  const {
    data: electionEvents,
    isLoading: isLoadingElectionEvents,
    isError: isErrorElectionEvents,
    refetch: refetchElectionEvents,
  } = useGetCitizenElectionEvents();

  // todo: go back
  const handleGoBack = () => {
    console.log("go back");
  };

  const renderHeader = useMemo(() => {
    return () => (
      <Header barStyle="light-content" backgroundColor="white">
        <Icon icon="vmCitizenLogo" paddingTop="$xxxl" />
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
          isPrimaryButtonDisabled={isLoadingElectionEvents}
          handleGoBack={handleGoBack}
        />
      </Screen>
    );
  }

  //   empty state
  if (!isLoadingElectionEvents && (!electionEvents || electionEvents.electionRounds.length === 0)) {
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
          {isLoadingElectionEvents && (
            <YStack justifyContent="center" alignItems="center" flex={1}>
              <Spinner color="$purple5" />
            </YStack>
          )}

          {!isLoadingElectionEvents && (
            <>
              <Typography color="$gray8">{t("description")}</Typography>

              {electionEvents?.electionRounds.map((event) => (
                <Selector
                  key={event.id}
                  description={event.title}
                  displayMode="light"
                  selected={selectedElectionEvent?.id === event.id}
                  onPress={() => setSelectedElectionEvent(event)}
                />
              ))}
            </>
          )}
        </ScrollView>
      </YStack>

      {/* //todo: continue */}
      <Footer primaryActionLabel={t("continue")} handleGoBack={handleGoBack} />
    </Screen>
  );
};
