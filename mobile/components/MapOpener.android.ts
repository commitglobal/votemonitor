import { Linking } from "react-native";
import Toast from "react-native-toast-message";
import i18n from "../common/config/i18n";
import { PollingStationNomenclatorNodeVM } from "../common/models/polling-station.model";

export const openMaps = (selectedPollingStation: PollingStationNomenclatorNodeVM) => {
  Linking.openURL(
    `geo:${selectedPollingStation?.latitude},${selectedPollingStation?.longitude}?q=${selectedPollingStation?.latitude},${selectedPollingStation?.longitude}(${selectedPollingStation?.number})`,
  ).catch(() =>
    Toast.show({
      type: "error",
      text1: `${i18n.t("meetings:details.map_error")}`,
    }),
  );
};
