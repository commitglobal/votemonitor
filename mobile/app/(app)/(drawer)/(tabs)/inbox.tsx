import { View } from "react-native";
import { Controller, useForm } from "react-hook-form";
import Button from "../../../../components/Button";
import { YStack } from "tamagui";
import FormInput from "../../../../components/FormInputs/FormInput";
// import RatingFormInput from "../../../../components/FormInputs/RatingFormInput";

interface FormData {
  // performanceRating: string;
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
  console.log("errors:", errors);

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

        {/* <Controller
          control={control}
          name="performanceRating"
          render={({ field: { onChange, value } }) => (
            <RatingFormInput
              id="123456"
              type="single"
              label="A4. Please indicate your opinion regarding performance of PEC"
              paragraph="According to the range of 1 = very bad to 5 = very good."
              helper="helper text"
              onValueChange={onChange}
              value={value}
            />
          )}
        /> */}

        <Button onPress={handleSubmit(onSubmit)}>Submit answer</Button>
      </YStack>
    </View>
  );
};

export default Inbox;
