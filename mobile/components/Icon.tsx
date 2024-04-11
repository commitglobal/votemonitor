/* eslint-disable @typescript-eslint/no-unused-vars */
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
import Check from "../assets/icons/check.svg";
import Calendar from "../assets/icons/Calendar.svg";
import TermsConds from "../assets/icons/Terms conds.svg";
import PrivacyPolicy from "../assets/icons/Privacy policy.svg";
import ContactNGO from "../assets/icons/Contact NGO.svg";
import AboutVM from "../assets/icons/About VM.svg";
import Feedback from "../assets/icons/Feedback.svg";
import Language from "../assets/icons/Language.svg";
import ChangePassword from "../assets/icons/Change password.svg";
import Settings from "../assets/icons/Settings.svg";
import LogoutNoBackground from "../assets/icons/LogoutNoBackground.svg";

import { styled, View } from "tamagui";
import { StyleProp, ViewStyle } from "react-native";
import { Ref } from "react";

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

export const defaultIcon = React.forwardRef(
  (props: IconProps, ref?: Ref<typeof View>): JSX.Element => {
    const { icon, color = "black", size = 24, style: $viewStyleOverride, ...tamaguiProps } = props;

    const iconRegistry: IconRegistry = {
      eyeOff: <EyeOff fill={color} width={size} height={size} />,
      eye: <Eye fill={color} width={size} height={size} />,
      observation: <Observation fill={color} width={size} height={size} />,
      quickReport: <QuickReport fill={color} width={size} height={size} />,
      learning: <Learning fill={color} width={size} height={size} />,
      inbox: <Inbox fill={color} width={size} height={size} />,
      more: <More fill={color} width={size} height={size} />,
      chevronRight: <ChevronRight fill={color} width={size} height={size} />,
      chevronLeft: <ChevronLeft fill={color} width={size} height={size} />,
      addNote: <AddNote fill={color} width={size} height={size} />,
      trash: <Trash fill={color} width={size} height={size} />,
      logout: <Logout fill={color} width={size} height={size} />,
      pencilAlt: <PencilAlt fill={color} width={size} height={size} />,
      xCircle: <XCircle fill={color} width={size} height={size} />,
      menuAlt2: <MenuAlt2 fill={color} width={size} height={size} />,
      dotsVertical: <DotsVertical fill={color} width={size} height={size} />,
      check: <Check fill={color} width={size} height={size} />,
      calendar: <Calendar fill={color} width={size} height={size} />,
      termsConds: <TermsConds fill={color} width={size} height={size} />,
      privacyPolicy: <PrivacyPolicy fill={color} width={size} height={size} />,
      contactNGO: <ContactNGO fill={color} width={size} height={size} />,
      aboutVM: <AboutVM fill={color} width={size} height={size} />,
      feedback: <Feedback fill={color} width={size} height={size} />,
      language: <Language fill={color} width={size} height={size} />,
      changePassword: <ChangePassword fill={color} width={size} height={size} />,
      settings: <Settings fill={color} width={size} height={size} />,
      logoutNoBackground: <LogoutNoBackground fill={color} width={size} height={size} />,
    };

    return (
      <View {...tamaguiProps} style={$viewStyleOverride}>
        {iconRegistry[icon]}
      </View>
    );
  },
);

export const Icon = styled(
  defaultIcon,
  {},
  {
    accept: {
      color: "color",
    },
  },
);
