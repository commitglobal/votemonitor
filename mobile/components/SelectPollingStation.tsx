import React, { useMemo, useState } from "react";
import { Adapt, Select, Sheet, View, YStack } from "tamagui";
import { Icon } from "./Icon";
import { Typography } from "./Typography";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import Button from "../components/Button";
import { useUserData } from "../contexts/user/UserContext.provider";
import { router } from "expo-router";
import { useTranslation } from "react-i18next";

const SelectPollingStation = () => {
  const { visits, selectedPollingStation, setSelectedPollingStationId } = useUserData();
  const [open, setOpen] = useState(false);
  const insets = useSafeAreaInsets();

  const { t } = useTranslation(["observation", "common"]);

  return (
    <YStack paddingVertical="$xs" paddingHorizontal="$md" backgroundColor="white">
      <Select
        onValueChange={setSelectedPollingStationId}
        disablePreventBodyScroll
        open={open}
        onOpenChange={setOpen}
        value={selectedPollingStation?.pollingStationId}
      >
        <Select.Trigger
          justifyContent="center"
          alignItems="center"
          backgroundColor="$purple1"
          borderRadius="$10"
          iconAfter={
            <Icon icon="chevronRight" size={24} transform="rotate(90deg)" color="$purple5" />
          }
        >
          <Select.Value
            width={"90%"}
            color="$purple5"
            placeholder={
              selectedPollingStation
                ? `${selectedPollingStation?.number} - ${selectedPollingStation?.name}`
                : t("loading", { ns: "common" })
            }
            fontWeight="500"
          ></Select.Value>
        </Select.Trigger>

        <Adapt platform="touch">
          <Sheet modal snapPoints={[80]} dismissOnSnapToBottom zIndex={100_100}>
            <Sheet.Frame>
              <YStack
                paddingVertical="$xl"
                paddingLeft="$lg"
                paddingRight="$xxxl"
                borderBottomWidth={1}
                borderBottomColor="$gray3"
              >
                <Typography preset="body2" color="$gray5">
                  {t("my_polling_stations.heading")}
                </Typography>
                {/* //TODO: not sure how many nroflines we should leave here */}
                <Typography numberOfLines={7} color="$gray5" marginTop="$xxs">
                  {/* //TODO: translation here */}
                  {t("my_polling_stations.paragraph")}
                </Typography>
              </YStack>

              <Sheet.ScrollView paddingHorizontal="$sm">
                <Adapt.Contents />
              </Sheet.ScrollView>

              <View
                paddingVertical="$xl"
                paddingHorizontal={40}
                borderTopWidth={1}
                borderTopColor="$gray3"
                marginBottom={insets.bottom}
              >
                <Button
                  preset="outlined"
                  onPress={() => {
                    setOpen(false);
                    router.push.bind(null, "/polling-station-wizzard")();
                  }}
                >
                  {t("my_polling_stations.add")}
                </Button>
              </View>
            </Sheet.Frame>
            <Sheet.Overlay />
          </Sheet>
        </Adapt>

        <Select.Content>
          <Select.Viewport>
            <Select.Group>
              {/* //TODO: texts from translation */}
              {useMemo(
                () =>
                  visits?.map((entry, i) => {
                    return (
                      <Select.Item
                        index={i}
                        key={entry.pollingStationId}
                        value={entry.pollingStationId}
                        gap="$3"
                      >
                        {/* //TODO: change number of lines to 2 if that's what we want */}
                        <Select.ItemText
                          numberOfLines={2}
                          color={
                            entry.pollingStationId === selectedPollingStation?.pollingStationId
                              ? "$purple5"
                              : "$gray9"
                          }
                        >
                          {entry.number} - {entry.address}
                        </Select.ItemText>
                      </Select.Item>
                    );
                  }),
                [visits, selectedPollingStation],
              )}
            </Select.Group>
          </Select.Viewport>
        </Select.Content>
      </Select>
    </YStack>
  );
};

export default SelectPollingStation;
