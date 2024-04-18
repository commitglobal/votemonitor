import React from "react";
import { Text } from "react-native";
import { Typography } from "../../../../components/Typography";
import { AlertDialog, Stack, XStack } from "tamagui";
import Select from "../../../../components/Select";
import Button from "../../../../components/Button";
import { Icon } from "../../../../components/Icon";
import Badge from "../../../../components/Badge";
import Card from "../../../../components/Card";
import { Screen } from "../../../../components/Screen";
import LinearProgress from "../../../../components/LinearProgress";
import CardFooter from "../../../../components/CardFooter";
import { Dialog } from "../../../../components/Dialog";
import QuestionCard from "../../../../components/QuestionCard";
import Header from "../../../../components/Header";
import { useNavigation } from "expo-router";
import { DrawerActions } from "@react-navigation/native";
import Input from "../../../../components/Inputs/Input";

const mockQuestions = [
  {
    id: "5043260e-017b-4e48-bb31-6e8bcdd870f0",
    text: "Were all necessary election materials present?",
    status: "not answered",
    numberOfQuestions: 6,
  },
  {
    id: "5043260e-017b-4e48-cb31-jnckencksjn",
    text: "Were the tasks/responsibilities of individual PEC members determined by drawing lots?",
    status: "answered",
    numberOfQuestions: 6,
  },
];

const QuickReport = () => {
  const navigation = useNavigation();
  return (
    <Screen
      preset="auto"
      backgroundColor="white"
      contentContainerStyle={{ gap: 20 }}
      ScrollViewProps={{
        stickyHeaderIndices: [0],
        bounces: false,
        showsVerticalScrollIndicator: false,
      }}
    >
      <Header
        title={"Quick Report"}
        titleColor="white"
        barStyle="light-content"
        leftIcon={<Icon icon="menuAlt2" color="white" />}
        onLeftPress={() => navigation.dispatch(DrawerActions.openDrawer)}
      />
      <Stack padding="$md" gap="$md">
        <Typography>Questions</Typography>
        {mockQuestions.map((question, index) => (
          <QuestionCard
            question={question}
            index={index + 1}
            onPress={() => console.log("question action")}
            key={question.id}
          />
        ))}
      </Stack>
      <Stack padding="$md" gap="$md">
        <Typography preset="subheading">Inputs</Typography>
        <Input type="text" placeholder="type = text" />
        <Input type="password" placeholder="type = password" />
        <Input type="textarea" placeholder="type = textarea" />
        <Input type="numeric" placeholder="type = numeric" />
      </Stack>

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
          <Badge status="not started">not started</Badge>
          <Badge status="completed">not started</Badge>
          <Badge status="in progress">not started</Badge>
          <Badge status="danger">not started</Badge>
        </Stack>

        <Stack padding="$sm" gap="$xs" backgroundColor="white">
          <Typography preset="heading">LinearProgress</Typography>
          <LinearProgress total={5} current={1}></LinearProgress>
          <LinearProgress total={5} current={2}></LinearProgress>
          <LinearProgress total={5} current={3}></LinearProgress>
          <LinearProgress total={5} current={4}></LinearProgress>
          <LinearProgress total={5} current={5}></LinearProgress>
        </Stack>

        <Stack padding="$sm" gap="$xs" backgroundColor="white">
          <Typography preset="heading">Dialog</Typography>
          <Stack marginTop="$md" gap="$sm">
            <Typography preset="subheading">Alert Dialog</Typography>
            <Dialog
              // trigger={<Button preset="red">Delete</Button>}
              header={<Typography preset="heading">Clear answer to Question A1</Typography>}
              content={
                <Typography preset="body1" color="$gray7">
                  Clearing the answer will permanently delete all its information (including notes
                  and media files). Question status will be reverted to Not Answered.
                </Typography>
              }
              footer={
                <XStack gap="$sm">
                  {/* //TODO: maybe we move this Cancel dirrectly into the Dialog? */}
                  {/* // !this 'asChild' is necessary in order to close the modal */}
                  <AlertDialog.Cancel asChild>
                    <Button preset="chromeless" textStyle={{ color: "black" }}>
                      Cancel
                    </Button>
                  </AlertDialog.Cancel>
                  <Button preset="red" flex={1}>
                    Clear answer
                  </Button>
                </XStack>
              }
            ></Dialog>
          </Stack>
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

export default QuickReport;
