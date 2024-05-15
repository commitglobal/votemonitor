import { View, Text } from "react-native";
import { useAuth } from "../../../../hooks/useAuth";
import Button from "../../../../components/Button";
import { Controller, useForm } from "react-hook-form";
// import DateFormInput from "../../../../components/FormInputs/DateFormInput";
import { YStack } from "tamagui";
import RadioFormInput from "../../../../components/FormInputs/RadioFormInput";

const options = [
  { id: "1", value: "yes", label: "Yes" },
  { id: "2", value: "no", label: "No" },
  { id: "3", value: "unknown", label: "Don't know" },
];

const More = () => {
  const { signOut } = useAuth();
  const { control, handleSubmit } = useForm();

  const onSubmit = (data: any) => {
    console.log(data);
  };

  return (
    <View>
      <Text>More</Text>
      <Button onPress={signOut}>Logout</Button>
      <YStack padding="$md">
        {/* <Controller
          control={control}
          name="checkInDate"
          render={({ field: { onChange, value } }) => (
            <DateFormInput
              label="A5. Lorem ipsum dolor sit amet consectetur?"
              paragraph="sncksnd"
              value={value}
              onChange={onChange}
            />
          )}
        ></Controller> */}
        <Controller
          control={control}
          name="allMaterialsPresent"
          render={({ field: { onChange, value } }) => (
            <RadioFormInput
              options={options}
              label="A1. Were all necessary election materials present?"
              paragraph="Lorem ipsum dolor sit amet consectetur. Maecenas donec pharetra elementum mauris est sodales."
              value={value}
              onValueChange={onChange}
            />
          )}
        ></Controller>
        <Button onPress={handleSubmit(onSubmit)}>Submit</Button>
      </YStack>
    </View>
  );
};

export default More;
