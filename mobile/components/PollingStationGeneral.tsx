import { router } from "expo-router";
import { XStack, YStack } from "tamagui";
import PollingStationInfo from "./PollingStationInfo";
import PollingStationInfoDefault from "./PollingStationInfoDefault";
import TimeSelect from "./TimeSelect";
import {
  PollingStationInformationAPIResponse,
  PollingStationInformationFormAPIResponse,
} from "../services/definitions.api";
import CardFooter from "./CardFooter";
import Card from "./Card";
import { useMutatePollingStationGeneralData } from "../services/mutations/psi-general.mutation";
import { ApiFormAnswer } from "../services/interfaces/answer.type";
import { useTranslation } from "react-i18next";
import Toast from "react-native-toast-message";
import ContentLoader, { Rect, Circle } from "react-content-loader/native";

interface PollingStationGeneralProps {
  electionRoundId: string;
  pollingStationId: string;
  psiData: PollingStationInformationAPIResponse | null | undefined;
  psiFormQuestions: PollingStationInformationFormAPIResponse;
}

// TODO: move and reuse for mutation fn
type PollingStationInformationVM = {
  arrivalTime: string;
  departureTime: string;
  answers: ApiFormAnswer[];
};

export const PollingStationGeneral: React.FC<PollingStationGeneralProps> = ({
  electionRoundId,
  pollingStationId,
  psiData: psi,
  psiFormQuestions,
}) => {
  const { t } = useTranslation("observation");

  const { mutate } = useMutatePollingStationGeneralData({
    electionRoundId,
    pollingStationId,
    scopeId: `PS_General_${electionRoundId}_${pollingStationId}_dates`,
  });

  const updateArrivalDepartureTime = (
    payload: Partial<Pick<PollingStationInformationVM, "arrivalTime" | "departureTime">>,
  ) => {
    mutate(
      { electionRoundId, pollingStationId, ...payload },
      {
        onError: () => {
          Toast.show({
            type: "error",
            text2: t("polling_stations_information.time_select.error.server"),
          });
        },
      },
    );
  };

  return (
    <>
      <XStack gap="$xxs">
        <Card flex={0.5} paddingVertical="$xs">
          <TimeSelect
            type="arrival"
            time={psi?.arrivalTime ? new Date(psi.arrivalTime) : undefined}
            departureTime={psi?.departureTime ? new Date(psi?.departureTime) : undefined}
            setTime={(data: Date) =>
              updateArrivalDepartureTime({
                arrivalTime: data?.toISOString(),
                ...(psi?.departureTime ? { departureTime: psi?.departureTime } : {}),
              })
            }
          />
        </Card>
        <Card flex={0.5} paddingVertical="$xs">
          <TimeSelect
            type="departure"
            time={psi?.departureTime ? new Date(psi?.departureTime) : undefined}
            arrivalTime={psi?.arrivalTime ? new Date(psi.arrivalTime) : undefined}
            setTime={(data: Date) =>
              updateArrivalDepartureTime({
                departureTime: data?.toISOString(),
                ...(psi?.arrivalTime ? { arrivalTime: psi?.arrivalTime } : {}),
              })
            }
          />
        </Card>
      </XStack>

      <Card
        gap="$md"
        onPress={router.push.bind(null, "/polling-station-questionnaire")}
        marginTop="$xxs"
      >
        {!psi?.answers?.length ? (
          <PollingStationInfoDefault
            onPress={router.push.bind(null, "/polling-station-questionnaire")}
          />
        ) : (
          <PollingStationInfo
            nrOfAnswers={psi?.answers?.length}
            nrOfQuestions={psiFormQuestions?.questions?.length}
          />
        )}
        <CardFooter
          text={t("polling_stations_information.polling_station_form.form_details_button_label")}
        ></CardFooter>
      </Card>

      <YStack gap="$xxs">
        <XStack gap="$xxs">
          <ContentLoader
            width="50%"
            height={100}
            backgroundColor="white"
            foregroundColor="hsl(265, 100%, 95%)"
          >
            <Rect width="100%" height={"100%"} />
          </ContentLoader>

          <ContentLoader
            width="50%"
            height={100}
            backgroundColor="white"
            foregroundColor="hsl(265, 100%, 95%)"
          >
            <Rect width="100%" height={"100%"} />
          </ContentLoader>
        </XStack>
        <ContentLoader
          height={150}
          animate={true}
          backgroundColor="white"
          foregroundColor="hsl(265, 100%, 95%)"
        >
          <Rect width="100%" height={"100%"} />
        </ContentLoader>
      </YStack>
    </>
  );
};
