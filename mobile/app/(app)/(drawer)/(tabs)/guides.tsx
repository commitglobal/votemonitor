import * as React from "react";
import { View, Text } from "react-native";
import CheckboxInput from "../../../../components/Inputs/CheckboxInput";
import { YStack, CheckedState } from "tamagui";
import { useForm, Controller } from "react-hook-form";
import Button from "../../../../components/Button";
import FormElement from "../../../../components/FormInputs/FormElement";

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

  const onSubmit = (data: FormData) => console.log(data);

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
      <YStack gap="$sm" padding="$md">
        <FormElement label="A1.1. Mark all the materials that are not present:">
          {/* //! we need a controller for every checbox input, so does it make sense to have a separate CheckboxFormInput where we add the controller inside? */}
          {checkboxOptions.map((option, index) => (
            <YStack>
              <Controller
                key={option.value}
                name="missingMaterials"
                control={control}
                render={() => (
                  <YStack>
                    <CheckboxInput
                      marginBottom="$md"
                      id={option.value}
                      label={option.label}
                      checked={getValues("missingMaterials").includes(option.value)}
                      onCheckedChange={(checked) => handleCheckboxChange(option.value, checked)}
                    />
                  </YStack>
                )}
              />
            </YStack>
          ))}
        </FormElement>
      </YStack>
      <Button onPress={handleSubmit(onSubmit)}>Submit</Button>
    </View>
  );
};

export default Guides;
