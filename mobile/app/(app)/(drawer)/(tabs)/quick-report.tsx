import React from "react";
import { Text } from "react-native";
import { Typography } from "../../../../components/Typography";
import { Stack } from "tamagui";
import Select from "../../../../components/Select";
import Button from "../../../../components/Button";
import { Icon } from "../../../../components/Icon";
import Badge from "../../../../components/Badge";
import Card from "../../../../components/Card";
import { Screen } from "../../../../components/Screen";
import LinearProgress from "../../../../components/LinearProgress";
import CardFooter from "../../../../components/CardFooter";
import SelectPollingStation from "../../../../components/SelectPollingStation";

const QuickReport = () => {
  return (
    <Screen preset="auto" backgroundColor="white" contentContainerStyle={{ gap: 20 }}>
      <SelectPollingStation placeholder="Select polling station" options={pollingStationAdresses} />
      <Text>Quick Report</Text>
      <Card>
        <Typography
          preset="heading"
          color="$red10"
          marginBottom="$sm"
          numberOfLines={1}
          style={{ backgroundColor: "yellow" }}
        >
          Hello from typographyHello from typographyHello from typographyHello from typographyHello
          from typographyHello from typographyHello from typography
        </Typography>
        <Typography>
          Hello from typographyHello from typographyHello from typographyHello from typographyHello
          from typographyHello from typographyHello from typography
        </Typography>
      </Card>
      <Card padding="$md">
        <Typography>Card component</Typography>
        <CardFooter marginTop="$sm" text="Card footer" />
      </Card>
      <Stack padding="$sm" gap="$xs">
        <Typography preset="heading">Button</Typography>

        <Stack gap="$xs">
          <Typography preset="subheading">Default</Typography>
          <Button onPress={() => console.log("filled")}>Filled</Button>
          <Button preset="outlined" onPress={() => console.log("outlined")}>
            Outlined
          </Button>
          <Button preset="red" onPress={() => console.log("Danger")}>
            Danger
          </Button>
          <Button preset="chromeless" onPress={() => console.log("Chromeless")}>
            Danger
          </Button>
        </Stack>
        <Stack gap="$xs">
          <Typography preset="subheading">Disabled</Typography>
          <Button disabled onPress={() => console.log("filled")}>
            Filled
          </Button>
          <Button disabled preset="outlined" onPress={() => console.log("outlined")}>
            Outlined
          </Button>
          <Button disabled preset="red" onPress={() => console.log("Danger")}>
            Danger
          </Button>
          <Button disabled preset="chromeless" onPress={() => console.log("Chromeless")}>
            Danger
          </Button>
        </Stack>
        <Stack gap="$xs">
          <Typography preset="subheading">With Icons</Typography>
          <Button
            icon={<Icon size={24} icon="chevronLeft" color="white" />}
            iconAfter={<Icon size={24} icon="chevronRight" color="white" />}
            onPress={() => console.log("filled")}
          >
            Filled
          </Button>
          <Button
            icon={<Icon size={24} icon="chevronLeft" color="purple" />}
            iconAfter={<Icon size={24} icon="chevronRight" color="purple" />}
            preset="outlined"
            onPress={() => console.log("outlined")}
          >
            Outlined
          </Button>
          <Button
            icon={<Icon size={24} icon="chevronLeft" color="white" />}
            iconAfter={<Icon size={24} icon="chevronRight" color="white" />}
            preset="red"
            onPress={() => console.log("Danger")}
          >
            Danger
          </Button>
          <Button
            icon={<Icon size={24} icon="chevronLeft" color="purple" />}
            iconAfter={<Icon size={24} icon="chevronRight" color="purple" />}
            preset="chromeless"
            onPress={() => console.log("Chromeless")}
          >
            Danger
          </Button>
        </Stack>
        <Stack gap="$xs" padding="$sm">
          <Typography preset="subheading">Select</Typography>
          <Select options={regionData} placeholder="Select option" defaultValue={"West"} />
          <Select options={countryData} placeholder="Select option" />
        </Stack>

        <Stack padding="$sm" gap="$xs" backgroundColor="white">
          <Typography preset="heading">Badge</Typography>
          <Badge status="not started" />
          <Badge status="completed" />
          <Badge status="in progress" />
          <Badge status="danger" />
        </Stack>

        <Stack padding="$sm" gap="$xs" backgroundColor="white">
          <Typography preset="heading">LinearProgress</Typography>
          <LinearProgress total={5} current={1}></LinearProgress>
          <LinearProgress total={5} current={2}></LinearProgress>
          <LinearProgress total={5} current={3}></LinearProgress>
          <LinearProgress total={5} current={4}></LinearProgress>
          <LinearProgress total={5} current={5}></LinearProgress>
        </Stack>
      </Stack>
    </Screen>
  );
};

const regionData = [
  { id: 1, value: "North", label: "North" },
  { id: 2, value: "North-West", label: "North-West" },
];

const countryData = [
  { id: 3, value: "Russia", label: "Russia" },
  { id: 4, value: "France", label: "France" },
  { id: 5, value: "China", label: "China" },
];

const pollingStationAdresses = [
  { id: 1, value: "Secția 123, Str. Moldovei, nr. 30, Târgu Mureș, Romania" },
  {
    id: 2,
    value: "Secția 456, Str. Transilvaniei, nr. 45, Cluj-Napoca, Romania",
  },
  { id: 3, value: "Secția 789, Str. București, nr. 12, București, Romania" },
  { id: 4, value: "Secția 101, Str. Timișoarei, nr. 20, Timișoara, Romania" },
  { id: 5, value: "Secția 234, Str. Iași, nr. 15, Iași, Romania" },
  { id: 6, value: "Secția 345, Str. Crișana, nr. 10, Oradea, Romania" },
  {
    id: 7,
    value: "Secția 567, Str. Maramureșului, nr. 25, Baia Mare, Romania",
  },
  { id: 8, value: "Secția 890, Str. Dobrogei, nr. 8, Constanța, Romania" },
  { id: 9, value: "Secția 111, Str. Ardealului, nr. 5, Sibiu, Romania" },
  { id: 10, value: "Secția 222, Str. Olteniei, nr. 18, Craiova, Romania" },
  { id: 11, value: "Secția 333, Str. Banatului, nr. 22, Arad, Romania" },
  { id: 12, value: "Secția 444, Str. Mureșului, nr. 11, Deva, Romania" },
  { id: 13, value: "Secția 555, Str. Dobrogei, nr. 7, Tulcea, Romania" },
  { id: 14, value: "Secția 667, Str. Moldovei, nr. 9, Bacău, Romania" },
  { id: 15, value: "Secția 777, Str. Crișului, nr. 13, Satu Mare, Romania" },
  { id: 16, value: "Secția 888, Str. Olteniei, nr. 4, Pitești, Romania" },
  { id: 17, value: "Secția 999, Str. Bucovinei, nr. 16, Suceava, Romania" },
  {
    id: 18,
    value: "Secția 1010, Str. Transilvaniei, nr. 32, Alba Iulia, Romania",
  },
  { id: 19, value: "Secția 1111, Str. Banatului, nr. 3, Reșița, Romania" },
  { id: 20, value: "Secția 1212, Str. București, nr. 7, Galați, Romania" },
];
export default QuickReport;
