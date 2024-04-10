import * as React from "react";
import { View, Text } from "react-native";
import CheckboxInput from "../../../../components/Inputs/CheckboxInput";
import { YStack, CheckedState } from "tamagui";
import { useForm, Controller } from "react-hook-form";
import Button from "../../../../components/Button";

interface FormData {
  missingMaterials: string[];
}

const checkboxOptions = [
  { label: "PEC SEal", value: "pecSeal" },
  { label: "Voters list", value: "votersList" },
  { label: "Envelopes", value: "envelopes" },
  { label: "Protocols", value: "protocols" },
];

const Guides = () => {
  const { control, handleSubmit, setValue, getValues } = useForm<FormData>({
    defaultValues: {
      missingMaterials: ["votersList"],
    },
  });

  // TODO: change data type here with that we need
  const onSubmit = (data: any) => console.log(data);

  const handleCheckboxChange = (value: string, checked: CheckedState) => {
    // Get the current state of missingMaterials from the form
    const currentMissingMaterials = getValues("missingMaterials");

    // Update the state based on whether the checkbox was checked or unchecked
    const updatedMissingMaterials = checked
      ? [...currentMissingMaterials, value] // Add to the array if checked
      : currentMissingMaterials.filter((v) => v !== value); // Remove from the array if unchecked

    // Update the missingMaterials state
    setValue("missingMaterials", updatedMissingMaterials, { shouldValidate: true });
  };

  return (
    <View>
      <Text>Guides hello</Text>
      <YStack gap="$sm">
        {checkboxOptions.map((option, index) => (
          <Controller
            key={option.value}
            name="missingMaterials"
            control={control}
            rules={{ required: true }}
            render={({ field: { onChange, value } }) => (
              <YStack>
                <CheckboxInput
                  id={index.toString()}
                  label={option.label}
                  checked={getValues("missingMaterials").includes(option.value)}
                  onCheckedChange={(checked) => handleCheckboxChange(option.value, checked)}
                />
              </YStack>
            )}
          />
        ))}
      </YStack>
      <Button onPress={handleSubmit(onSubmit)}>Submit</Button>
    </View>
  );
};

export default Guides;
