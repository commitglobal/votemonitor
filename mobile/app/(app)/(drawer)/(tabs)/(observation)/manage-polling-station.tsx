import { Screen } from "../../../../../components/Screen";
import Header from "../../../../../components/Header";
import { Icon } from "../../../../../components/Icon";
import { YStack, XStack, View } from "tamagui";
import { Typography } from "../../../../../components/Typography";
import { router } from "expo-router";
import Card from "../../../../../components/Card";
import { useUserData } from "../../../../../contexts/user/UserContext.provider";
import { Dialog } from "../../../../../components/Dialog";
import Button from "../../../../../components/Button";
import { useState } from "react";
import { PollingStationVisitVM } from "../../../../../common/models/polling-station.model";

const ManagePollingStation = () => {
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [selectedPollingStation, setSelectedPollingStation] = useState("");
  const { visits } = useUserData();
  if (visits === undefined || visits.length === 0) {
    return <Typography>No visits</Typography>;
  }

  console.log(visits[0]);

  return (
    <Screen
      preset="scroll"
      ScrollViewProps={{
        showsVerticalScrollIndicator: false,
        stickyHeaderIndices: [0],
        bounces: false,
      }}
      backgroundColor="white"
    >
      <Header
        title={"Manage my polling station"}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="chevronLeft" color="white" />}
        onLeftPress={() => router.back()}
      />

      <YStack gap={24} paddingTop={24} paddingHorizontal={16}>
        {visits.map((visit) => (
          <Card
            key={visit.pollingStationId}
            onPress={() => {
              setDeleteDialogOpen(true);
              setSelectedPollingStation(visit.number);
            }}
          >
            <PollingStationInfo visit={visit} />
          </Card>
        ))}
      </YStack>

      {/* TODO: API call for delete polling-station */}
      {deleteDialogOpen && (
        <DeletePollingStationDialog
          pollingStationNumber={selectedPollingStation}
          onCancel={() => setDeleteDialogOpen(false)}
          onDelete={() => {
            console.log("DELETED");
            setDeleteDialogOpen(false);
          }}
        />
      )}
    </Screen>
  );
};

interface DeletePollingStationDialogProps {
  pollingStationNumber: string;
  onCancel: () => void;
  onDelete: () => void;
}

const DeletePollingStationDialog = (props: DeletePollingStationDialogProps) => {
  const { onCancel, onDelete, pollingStationNumber } = props;

  return (
    <Dialog
      open
      header={
        <Typography preset="heading">Delete Polling station #{pollingStationNumber}</Typography>
      }
      content={
        <View>
          <Typography preset="body1" color="$gray8">
            This action will delete all answers entered so far for this polling station (including
            polling station information, form answers, notes and media files). {"\n\n"}
            To avoid unintended information loss, please ensure you check the polling station
            details before performing this action.
          </Typography>
        </View>
      }
      footer={
        <XStack gap="$md">
          <Button preset="chromeless" textStyle={{ color: "black" }} onPress={onCancel}>
            Cancel
          </Button>
          <Button preset="red" onPress={onDelete} flex={1}>
            Delete
          </Button>
        </XStack>
      }
    />
  );
};

interface PollingStationCardProps {
  visit: PollingStationVisitVM;
}

const PollingStationInfo = (props: PollingStationCardProps) => {
  const { visit } = props;

  return (
    <YStack gap={16}>
      <XStack justifyContent="space-between" alignItems="center">
        <Typography preset="body1" fontWeight="700">
          Polling station #{visit.number}
        </Typography>
        <Icon icon="bin" color="white"></Icon>
      </XStack>

      <Typography>
        [Location L1]: <Typography fontWeight="500">{visit.level1}</Typography>{" "}
      </Typography>
      <Typography>
        [Location L2]: <Typography fontWeight="500"> {visit.level2} </Typography>{" "}
      </Typography>
      <Typography>
        [Location L3]: <Typography fontWeight="500">{visit.level3}</Typography>
      </Typography>
      <Typography>
        [Street]: <Typography fontWeight="500">{visit.address}</Typography>
      </Typography>
      <Typography>
        Polling station number: <Typography fontWeight="500">{visit.number}</Typography>
      </Typography>
    </YStack>
  );
};

export default ManagePollingStation;
