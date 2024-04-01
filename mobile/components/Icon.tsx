import * as React from "react";
import EyeOff from "../assets/icons/Eye off.svg";
import Eye from "../assets/icons/Eye.svg";
import Observation from "../assets/icons/observation.svg";
import QuickReport from "../assets/icons/quick-report.svg";
import Learning from "../assets/icons/Learning.svg";
import Inbox from "../assets/icons/Inbox.svg";
import More from "../assets/icons/More.svg";
import ChevronRight from "../assets/icons/Chevron right.svg";
import ChevronLeft from "../assets/icons/Chevron left.svg";
import AddNote from "../assets/icons/add-note.svg";
import Trash from "../assets/icons/Trash.svg";
import Logout from "../assets/icons/logout.svg";
import PencilAlt from "../assets/icons/Pencil alt.svg";
import XCircle from "../assets/icons/x-circle.svg";
import MenuAlt2 from "../assets/icons/menu-alt-2.svg";
import DotsVertical from "../assets/icons/dots-vertical.svg";

import { styled } from "tamagui";
import { View } from "tamagui";
import { StyleProp, ViewStyle } from "react-native";

interface IconProps {
  /**
   * The name of the icon
   */
  icon: string;

  /**
   * An optional tint color for the icon
   */
  color?: string;

  /**
   * An optional size for the icon. If not provided, the icon will be sized to the icon's resolution.
   */
  size?: number;

  /**
   * Style overrides for the view container
   */
  style?: StyleProp<ViewStyle>;
}

/**
 * A component to render a registered icon.
 * It is wrapped in a <TouchableOpacity /> if `onPress` is provided, otherwise a <View />.
 * @see [Documentation and Examples]{@link https://docs.infinite.red/ignite-cli/boilerplate/components/Icon/}
 * @param {IconProps} props - The props for the `Icon` component.
 * @returns {JSX.Element} The rendered `Icon` component.
 */

type IconRegistry = {
  [key: string]: React.ReactNode;
};

export const defaultIcon = (props: IconProps) => {
  const {
    icon,
    color,
    size,
    style: $viewStyleOverride,
    ...tamaguiProps
  } = props;

  const iconRegistry: IconRegistry = {
    eyeOff: (
      <EyeOff fill={color || "black"} width={size || 24} height={size || 24} />
    ),
    eye: <Eye fill={color || "black"} width={size || 24} height={size || 24} />,
    observation: (
      <Observation
        fill={color || "black"}
        width={size || 24}
        height={size || 24}
      />
    ),

    quickReport: (
      <QuickReport
        fill={color || "black"}
        width={size || 24}
        height={size || 24}
      />
    ),
    learning: (
      <Learning
        fill={color || "black"}
        width={size || 24}
        height={size || 24}
      />
    ),
    inbox: (
      <Inbox fill={color || "black"} width={size || 24} height={size || 24} />
    ),
    more: (
      <More fill={color || "black"} width={size || 24} height={size || 24} />
    ),
    chevronRight: (
      <ChevronRight
        fill={color || "black"}
        width={size || 24}
        height={size || 24}
      />
    ),
    chevronLeft: (
      <ChevronLeft
        fill={color || "black"}
        width={size || 24}
        height={size || 24}
      />
    ),
    addNote: (
      <AddNote fill={color || "black"} width={size || 24} height={size || 24} />
    ),
    trash: (
      <Trash fill={color || "black"} width={size || 24} height={size || 24} />
    ),
    logout: (
      <Logout fill={color || "black"} width={size || 24} height={size || 24} />
    ),
    pencilAlt: (
      <PencilAlt
        fill={color || "black"}
        width={size || 24}
        height={size || 24}
      />
    ),
    xCircle: (
      <XCircle fill={color || "black"} width={size || 24} height={size || 24} />
    ),
    menuAlt2: (
      <MenuAlt2
        fill={color || "black"}
        width={size || 24}
        height={size || 24}
      />
    ),
    dotsVertical: (
      <DotsVertical
        fill={color || "black"}
        width={size || 24}
        height={size || 24}
      />
    ),
  };

  return (
    <View {...tamaguiProps} style={$viewStyleOverride}>
      {iconRegistry[icon]}
    </View>
  );
};

export const Icon = styled(
  defaultIcon,
  {},
  {
    accept: {
      color: "color",
    },
  }
);
