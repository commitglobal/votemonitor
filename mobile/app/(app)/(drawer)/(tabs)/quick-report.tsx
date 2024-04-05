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

const QuickReport = () => {
  return (
    <Screen preset="auto" backgroundColor="white" contentContainerStyle={{ gap: 20 }}>
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
        <Card gap="$xs" padding="$sm">
          <Typography preset="subheading">Select</Typography>
          <Select options={regionData} placeholder="Select option" defaultValue={"West"} />
          <Select options={countryData} placeholder="Select option" />
        </Card>

        <Stack padding="$sm" gap="$xs" backgroundColor="white">
          <Typography preset="heading">Badge</Typography>
          <Badge> Not started </Badge>
          <Badge preset="success"> Success </Badge>
          <Badge preset="warning"> In progress </Badge>
          <Badge preset="danger"> Red badge</Badge>
        </Stack>
      </Stack>
    </Screen>
  );
};

const regionData = [
  { id: 1, value: "North" },
  { id: 2, value: "North-West" },
  { id: 3, value: "North-East" },
  { id: 4, value: "West" },
  { id: 5, value: "East" },
  { id: 6, value: "South-West" },
  { id: 7, value: "South" },
];

const countryData = [
  { id: 3, value: "Russia" },
  { id: 4, value: "France" },
  { id: 5, value: "China" },
  { id: 6, value: "Brazil" },
  { id: 7, value: "Australia" },
];

export default QuickReport;
