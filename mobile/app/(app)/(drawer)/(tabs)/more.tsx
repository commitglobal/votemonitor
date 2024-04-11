import { View, Text } from "react-native";
import { useAuth } from "../../../../hooks/useAuth";
import Button from "../../../../components/Button";
import { Controller, useForm } from "react-hook-form";
import DateFormInput from "../../../../components/FormInputs/DateFormInput";
import { YStack } from "tamagui";

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
        <Controller
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
        ></Controller>
        <Button onPress={handleSubmit(onSubmit)}>Submit</Button>
      </YStack>
    </View>
  );
};

export default More;
