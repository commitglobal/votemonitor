import { DrawerActions } from "@react-navigation/native";
import { useNavigation } from "expo-router";
import { useMemo, useState } from "react";
import { useTranslation } from "react-i18next";
import { YStack } from "tamagui";
import Header from "../../../components/Header";
import { Icon } from "../../../components/Icon";
import PastElectionsList from "../../../components/PastElectionsList";
import { Screen } from "../../../components/Screen";
import SearchInput from "../../../components/SearchInput";
import { electionRoundSorter } from "../../../helpers/election-rounds";
import { useElectionRoundsQuery } from "../../../services/queries.service";

const PastElections = () => {
  const { t } = useTranslation("er-statistics");
  const navigation = useNavigation();

  const [search, setSearch] = useState("");

  const {
    data: electionRounds,
    isLoading: isLoadingElectionRounds,
    refetch: refetchGuides,
  } = useElectionRoundsQuery();

  const filteredElectionRounds = useMemo(() => {
    return (
      electionRounds
        ?.filter((er) => er.status === "Archived")
        ?.filter((x) => x.title.toLocaleLowerCase().includes(search.toLocaleLowerCase()))
        .sort(electionRoundSorter) ?? []
    );
  }, [electionRounds, search]);

  const handleSearch = (text: string) => {
    setSearch(text);
  };

  return (
    <Screen
      preset="fixed"
      contentContainerStyle={{
        flexGrow: 1,
      }}
    >
      <YStack>
        <Header
          title={t("title")}
          titleColor="white"
          barStyle="light-content"
          leftIcon={<Icon icon="menuAlt2" color="white" />}
          onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
        />
        <YStack backgroundColor="$purple5" padding="$md">
          <SearchInput onSearch={handleSearch} />
        </YStack>
      </YStack>
      <PastElectionsList
        key={`past-elections`}
        isLoading={isLoadingElectionRounds}
        elections={filteredElectionRounds}
        refetch={refetchGuides}
        emptyContainerMarginTop="30%"
      />
    </Screen>
  );
};

export default PastElections;
