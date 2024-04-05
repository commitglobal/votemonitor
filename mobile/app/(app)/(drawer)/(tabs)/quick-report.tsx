import React, { useState } from "react";
import { ScrollView, Text } from "react-native";
import { Typography } from "../../../../components/Typography";
import { RadioGroup, Stack } from "tamagui";
import Button from "../../../../components/Button";
import { Icon } from "../../../../components/Icon";
import Badge from "../../../../components/Badge";

import Card from "../../../../components/Card";
import Input from "../../../../components/Inputs/Input";
import CheckboxInput from "../../../../components/Inputs/CheckboxInput";
import RadioInput from "../../../../components/Inputs/RadioInput";

const QuickReport = () => {
  const [selectedRadioValue, setSelectedRadioValue] = useState("rural");

  return (
    <ScrollView>
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
      </Stack>

      <Stack padding="$sm" gap="$xs" backgroundColor="white">
        <Typography preset="heading">Badge</Typography>
        <Badge> Not started </Badge>
        <Badge preset="success"> Success </Badge>
        <Badge preset="warning"> In progress </Badge>
        <Badge preset="danger"> Red badge</Badge>
      </Stack>

      {/* inputs */}
      <Stack padding="$sm" gap="$xs" marginTop="$md" backgroundColor="white">
        <Typography preset="heading">Inputs</Typography>
        <Typography preset="subheading">Text</Typography>
        <Input />
        <Typography preset="subheading">Checkbox</Typography>
        <CheckboxInput label="hello" id="1" />
        <CheckboxInput label="hello2" id="2" />
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
      </Stack>
    </ScrollView>
  );
};

export default QuickReport;
