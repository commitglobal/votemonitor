import React, { useState } from "react";
import { Text } from "react-native";
import { Typography } from "../../../../components/Typography";
import { RadioGroup, Stack } from "tamagui";
import Select from "../../../../components/Select";
import Button from "../../../../components/Button";
import { Icon } from "../../../../components/Icon";
import Badge from "../../../../components/Badge";

import Card from "../../../../components/Card";
import Input from "../../../../components/Inputs/Input";
import CheckboxInput from "../../../../components/Inputs/CheckboxInput";
import RadioInput from "../../../../components/Inputs/RadioInput";
import { Screen } from "../../../../components/Screen";
import LinearProgress from "../../../../components/LinearProgress";
import CardFooter from "../../../../components/CardFooter";
import { RatingInput } from "../../../../components/Inputs/RatingInput";
import { DateInput } from "../../../../components/Inputs/DateInput";
import CircularProgress from "../../../../components/CircularProgress";

const QuickReport = () => {
  const [selectedRadioValue, setSelectedRadioValue] = useState("rural");
  const [progress, setProgress] = useState(0);

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
      <Card padding="$md">
        <Typography>Card component</Typography>
        <CardFooter text="Card footer" marginTop="$sm" />
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
          <Badge> Not started </Badge>
          <Badge preset="success"> Success </Badge>
          <Badge preset="warning"> In progress </Badge>
          <Badge preset="danger"> Red badge</Badge>
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

      {/* inputs */}
      <Stack padding="$sm" gap="$xs" marginTop="$md" backgroundColor="white">
        <Typography preset="heading">Inputs</Typography>
        <Typography preset="subheading">Text/Numeric</Typography>
        <Input type="text" />
        <Input type="numeric" />

        <Typography preset="subheading">Checkbox</Typography>
        <CheckboxInput id="1" label="hello" />
        <CheckboxInput id="2" label="hello2" defaultChecked />

        <Typography preset="subheading">Radio buttons</Typography>
        <RadioGroup
          gap="$sm"
          defaultValue={selectedRadioValue}
          onValueChange={(value) => setSelectedRadioValue(value)}
        >
          <RadioInput id="10" value="rural" label="Rural" selectedValue={selectedRadioValue} />
          <RadioInput id="20" value="urban" label="Urban" selectedValue={selectedRadioValue} />
          <RadioInput
            id="30"
            value="not-known"
            label="Not known"
            selectedValue={selectedRadioValue}
          />
        </RadioGroup>

        <Typography preset="subheading">Rating</Typography>
        <RatingInput
          id="5"
          type="single"
          defaultValue="2"
          // onValueChange={(value) => console.log(value)}
        />

        <Typography preset="subheading">Date</Typography>
        <DateInput minimumDate={new Date(2024, 6, 20)} />
      </Stack>

      <Stack padding="$sm" gap="$xs" marginTop="$md" backgroundColor="white">
        <Button
          preset="outlined"
          style={{ marginBottom: 20 }}
          onPress={() => {
            setProgress((prevProgress) => {
              const newProgress = prevProgress + 10;
              return newProgress <= 360 ? newProgress : 360;
            });
          }}
        >
          Progress 10%
        </Button>
        <Button
          preset="outlined"
          style={{ marginBottom: 20 }}
          onPress={() => {
            setProgress((prevProgress) => {
              const newProgress = prevProgress + 45;
              return newProgress <= 360 ? newProgress : 360;
            });
          }}
        >
          Progress 45%
        </Button>
        <CircularProgress progress={progress}></CircularProgress>
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
  { id: 8, value: "South-East" },
  { id: 9, value: "Central" },
  { id: 10, value: "Mid-West" },
  { id: 11, value: "Mid-East" },
  { id: 12, value: "Far North" },
  { id: 13, value: "Far South" },
  { id: 14, value: "Far West" },
  { id: 15, value: "Far East" },
  { id: 16, value: "Northern Territory" },
  { id: 17, value: "Pacific Northwest" },
  { id: 18, value: "South Central" },
];

const countryData = [
  { id: 3, value: "Russia" },
  { id: 4, value: "France" },
  { id: 5, value: "China" },
  { id: 6, value: "Brazil" },
  { id: 7, value: "Australia" },
];

export default QuickReport;
