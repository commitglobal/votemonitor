/* eslint-disable @typescript-eslint/no-unused-vars */
import * as React from "react";
import EyeOff from "../assets/icons/Eye off.svg";
import Eye from "../assets/icons/Eye.svg";
import Observation from "../assets/icons/tabs/observation.svg";
import QuickReport from "../assets/icons/tabs/report.svg";
import Learning from "../assets/icons/tabs/guides.svg";
import Inbox from "../assets/icons/tabs/inbox.svg";
import More from "../assets/icons/tabs/more.svg";
import ChevronRight from "../assets/icons/Chevron right.svg";
import ChevronLeft from "../assets/icons/Chevron left.svg";
import AddNote from "../assets/icons/add-note.svg";
import Trash from "../assets/icons/Trash.svg";
import Logout from "../assets/icons/logout.svg";
import PencilAlt from "../assets/icons/Pencil-alt.svg";
import XCircle from "../assets/icons/x-circle.svg";
import MenuAlt2 from "../assets/icons/menu-alt-2.svg";
import DotsVertical from "../assets/icons/dots-vertical.svg";
import Check from "../assets/icons/check.svg";
import Calendar from "../assets/icons/Calendar.svg";
import PeopleAddingVote from "../assets/icons/people-adding-vote.svg";
import LoadingScreenDevice from "../assets/icons/loading-screen-device.svg";
import MissingPollingStation from "../assets/icons/missing-polling-station.svg";
import X from "../assets/icons/x.svg";
import TermsConds from "../assets/icons/Terms conds.svg";
import PrivacyPolicy from "../assets/icons/Privacy policy.svg";
import ContactNGO from "../assets/icons/Contact NGO.svg";
import AboutVM from "../assets/icons/About VM.svg";
import Feedback from "../assets/icons/Feedback.svg";
import Language from "../assets/icons/Language.svg";
import ChangePassword from "../assets/icons/Change password.svg";
import Settings from "../assets/icons/Settings.svg";
import LogoutNoBackground from "../assets/icons/LogoutNoBackground.svg";
import Attachment from "../assets/icons/attachment.svg";
import DragHandle from "../assets/icons/Drag handle.svg";
import Search from "../assets/icons/search.svg";
import LoginLogo from "../assets/icons/VM login logo.svg";
import InfoCircle from "../assets/icons/Information circle.svg";
import EmailSent from "../assets/icons/Email sent.svg";
import UndrawInbox from "../assets/icons/undraw_inbox_oppv.svg";
import UndrawReading from "../assets/icons/undraw_reading_book.svg";
import UndrawFlag from "../assets/icons/undraw_report_flag.svg";
import LoginError from "../assets/icons/LoginError.svg";
import PasswordConfirmation from "../assets/icons/password confirmation.svg";
import SplashLogo from "../assets/icons/splash-logo.svg";
import OnboardingLanguage from "../assets/icons/onboarding/onboarding-language.svg";
import MonitorPollingStations from "../assets/icons/onboarding/monitor-p-s.svg";
import ObservationForms from "../assets/icons/onboarding/forms.svg";
import NotesOrMedia from "../assets/icons/onboarding/notes-media.svg";
import CommitGlobal from "../assets/icons/commit-global.svg";
import PollingStationPin from "../assets/icons/polling-station-pin.svg";
import Form from "../assets/icons/form.svg";
import CheckCircle from "../assets/icons/check-circle.svg";
import Bin from "../assets/icons/Trash2.svg";
import { styled, View, ViewProps } from "tamagui";
import { StyleProp, ViewStyle } from "react-native";
import { Ref } from "react";
import Note from "../assets/icons/questionCard/note.svg";
import Photo from "../assets/icons/questionCard/photo.svg";
import Video from "../assets/icons/questionCard/video.svg";
import Audio from "../assets/icons/questionCard/audio.svg";
import VMCitizenLogo from "../assets/icons/vm-citizen-logo.svg";
import Warning from "../assets/icons/warning.svg";
import PublicResourcesProblems from "../assets/icons/public-resources-problems.svg";
import AppModeSwitch from "../assets/icons/app-mode-switch.svg";
import CoffeeBreak from "../assets/icons/coffee-break.svg";

interface IconProps extends ViewProps {
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
  width?: number;
  height?: number;

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
    const {
      icon,
      color = "black",
      size = 24,
      style: $viewStyleOverride,
      width,
      height,
      ...tamaguiProps
    } = props;

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
      peopleAddingVote: <PeopleAddingVote fill={color} width={226} height={170} />,
      loadingScreenDevice: <LoadingScreenDevice fill={color} width={226} height={170} />,
      missingPollingStation: <MissingPollingStation fill={color} width={224} height={170} />,
      x: <X fill={color} width={size} height={size} />,
      termsConds: <TermsConds fill={color} width={size} height={size} />,
      privacyPolicy: <PrivacyPolicy fill={color} width={size} height={size} />,
      contactNGO: <ContactNGO fill={color} width={size} height={size} />,
      aboutVM: <AboutVM fill={color} width={size} height={size} />,
      feedback: <Feedback fill={color} width={size} height={size} />,
      language: <Language fill={color} width={size} height={size} />,
      changePassword: <ChangePassword fill={color} width={size} height={size} />,
      settings: <Settings fill={color} width={size} height={size} />,
      logoutNoBackground: <LogoutNoBackground fill={color} width={size} height={size} />,
      attachment: <Attachment fill={color} width={size} height={size} />,
      dragHandle: <DragHandle fill={color} width={size} height={size} />,
      search: <Search fill={color} width={size} height={size} />,
      loginLogo: <LoginLogo fill={color} width={size | 294} height={size | 67} />,
      infoCircle: <InfoCircle width={size} height={size} stroke={color} />,
      emailSent: <EmailSent fill={color} width={size} height={size} />,
      undrawInbox: <UndrawInbox fill={color} width={size} height={size} />,
      undrawReading: <UndrawReading fill={color} width={size} height={size} />,
      undrawFlag: <UndrawFlag fill={color} width={size | 187} height={170 | size} />,
      loginError: <LoginError fill={color} width={size} height={size} />,
      passwordConfirmation: <PasswordConfirmation fill={color} width={size} height={size} />,
      splashLogo: <SplashLogo width={315.6} height={72} />,
      onboardingLanguage: <OnboardingLanguage width={243} height={187} />,
      monitorPollingStations: <MonitorPollingStations width={202} height={188} />,
      observationForms: <ObservationForms width={202} height={188} />,
      notesOrMedia: <NotesOrMedia width={202} height={188} />,
      bin: <Bin fill={color} width={size} height={size} />,
      commitGlobal: <CommitGlobal fill={color} />,
      pollingStationPin: <PollingStationPin fill={color} />,
      note: <Note fill={color} />,
      photo: <Photo fill={color} />,
      video: <Video fill={color} />,
      audio: <Audio fill={color} />,
      form: <Form fill={color} />,
      checkCircle: <CheckCircle fill={color} width={size} height={size} />,
      vmCitizenLogo: <VMCitizenLogo fill={color} width={width} height={height} />,
      warning: <Warning stroke={color} width={size} height={size} />,
      publicResourcesProblems: <PublicResourcesProblems fill={color} width={size} height={size} />,
      appModeSwitch: <AppModeSwitch fill={color} width={size} height={size} />,
      coffeeBreak: <CoffeeBreak fill={color} width={size} height={size} />,
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
