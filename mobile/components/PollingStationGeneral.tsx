import { router } from "expo-router";
import { XStack } from "tamagui";
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
  const { mutate } = useMutatePollingStationGeneralData({
    electionRoundId,
    pollingStationId,
    scopeId: `PS_General_${electionRoundId}_${pollingStationId}_dates`,
  });

  const updateArrivalDepartureTime = (
    payload: Partial<Pick<PollingStationInformationVM, "arrivalTime" | "departureTime">>,
  ) => {
    mutate({ electionRoundId, pollingStationId, ...payload });
  };

  return (
    <>
      <XStack gap="$xxs">
        <Card flex={0.5} paddingVertical="$xs">
          <TimeSelect
            type="arrival"
            time={psi?.arrivalTime ? new Date(psi.arrivalTime) : undefined}
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
            arrivalTime={psi?.arrivalTime ? new Date(psi.arrivalTime) : undefined}
            time={psi?.departureTime ? new Date(psi?.departureTime) : undefined}
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
        <CardFooter text="Polling station information"></CardFooter>
      </Card>
    </>
  );
};
