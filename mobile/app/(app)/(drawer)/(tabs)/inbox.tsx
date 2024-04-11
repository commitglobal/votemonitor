import { View, Text } from "react-native";
import { Controller, useForm } from "react-hook-form";
import Button from "../../../../components/Button";
import FormInput from "../../../../components/FormInputs/FormInput";
import { YStack } from "tamagui";

interface FormData {
  nrOfMembers: string;
}

const Inbox = () => {
  const {
    control,
    handleSubmit,
    formState: { errors },
  } = useForm<FormData>();
  const onSubmit = (data: any) => {
    console.log(data);
  };

  return (
    <View style={{ backgroundColor: "white" }}>
      <YStack padding="$md" gap="$lg">
        <Controller
          control={control}
          name="nrOfMembers"
          rules={{ maxLength: 10 }}
          render={({ field: { onChange, value } }) => (
            <FormInput
              type="text"
              label="A2. How many PEC members have been appointed?"
              paragraph="Lorem ipsum dolor sit amet consectetur. Maecenas donec pharetra elementum mauris est sodales."
              helper="10 characters"
              onChangeText={onChange}
              value={value}
            />
          )}
        />
        <Button onPress={handleSubmit(onSubmit)}>Submit answer</Button>
      </YStack>
    </View>
  );
};

export default Inbox;
