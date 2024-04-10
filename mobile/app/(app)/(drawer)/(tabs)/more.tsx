import { View, Text, XStack } from "tamagui";
import Card from "../../../../components/Card";
import { Screen } from "../../../../components/Screen";
import { Typography } from "../../../../components/Typography";
import { Icon } from "../../../../components/Icon";

const More = () => {
  return (
    <Screen preset="auto">
      <Text> More</Text>
      <Section
        headerText="Change app language"
        subHeaderText="English (United States)"
        iconName="check"
      ></Section>
    </Screen>
  );
};

interface SectionProps {
  headerText: string;
  subHeaderText?: string;
  iconName: string;
}

const Section = (props: SectionProps) => {
  const { headerText, subHeaderText, iconName } = props;
  const hasSubHeader = subHeaderText ? true : false;

  return (
    <Card>
      <XStack alignItems="center" justifyContent="space-between">
        <XStack alignItems="center" gap="$xxs">
          <Icon size={24} icon={iconName} color="black" />
          <View alignContent="center" gap="$xxxs">
            <Typography preset="body2"> {headerText} </Typography>
            {hasSubHeader && <Typography color="$gray8"> {subHeaderText}</Typography>}
          </View>
        </XStack>

        <Icon size={24} icon="chevronRight" color="$purple7" />
      </XStack>
    </Card>
  );
};

export default More;
