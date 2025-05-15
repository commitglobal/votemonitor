import { ActionSheetIOS, Linking } from "react-native";
import Toast from "react-native-toast-message";
import { PollingStationNomenclatorNodeVM } from "../common/models/polling-station.model";
import i18n from "../common/config/i18n";

export const openMaps = (selectedPollingStation: PollingStationNomenclatorNodeVM) => {
  const mapApps = [
    {
      title: "Apple Maps",
      url: `maps://app?daddr=${selectedPollingStation?.latitude},${selectedPollingStation?.longitude}&label=${selectedPollingStation?.number}`,
    },
    {
      title: "Google Maps",
      url: `comgooglemaps://?q=${selectedPollingStation?.number}&daddr=${selectedPollingStation?.latitude},${selectedPollingStation?.longitude}`,
    },
    {
      title: "Waze",
      url: `https://waze.com/ul?ll=${selectedPollingStation?.latitude},${selectedPollingStation?.longitude}`,
    },
  ];

  const mapOptions = mapApps.map((app) => app.title);
  ActionSheetIOS.showActionSheetWithOptions(
    {
      options: [`${i18n.t("general:cancel")}`, ...mapOptions],
      cancelButtonIndex: 0,
    },
    (buttonIndex) => {
      if (buttonIndex > 0) {
        const selectedApp = mapApps[buttonIndex - 1];
        Linking.openURL(selectedApp.url).catch(() =>
          Toast.show({
            type: "error",
            text1: `${i18n.t("meetings:details.map_error")}`,
          }),
        );
      }
    },
  );
};
