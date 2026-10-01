import { useSafeAreaInsets } from "react-native-safe-area-context";
import { useNetInfoContext } from "../contexts/net-info-banner/NetInfoContext";

/**
 * Bottom safe area inset for screen footers.
 * When the net info banner is displayed, it already covers the bottom inset, so we return 0.
 */
export const useBottomInset = () => {
  const insets = useSafeAreaInsets();
  const { shouldDisplayBanner } = useNetInfoContext();

  return shouldDisplayBanner ? 0 : insets.bottom;
};
