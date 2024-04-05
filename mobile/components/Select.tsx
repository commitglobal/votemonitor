import React, { useMemo, useState } from "react";
import { Adapt, Select as TamaguiSelect, Sheet } from "tamagui";
import { Icon } from "./Icon";

const Select = ({
  placeholder = "Select",
  options,
}: {
  placeholder?: string;
  options: { id: string | number; value: string }[];
}) => {
  const [val, setVal] = useState("");

  return (
    <TamaguiSelect
      value={val}
      onValueChange={setVal}
      disablePreventBodyScroll
      native
    >
      <TamaguiSelect.Trigger
        backgroundColor="transparent"
        paddingHorizontal="$md"
        iconAfter={
          <Icon
            icon="chevronRight"
            size={20}
            transform="rotate(90deg)"
            color="$gray7"
          />
        }
      >
        <TamaguiSelect.Value
          width={"90%"}
          color="$gray5"
          placeholder={placeholder}
          fontWeight="500"
        ></TamaguiSelect.Value>
      </TamaguiSelect.Trigger>

      <Adapt platform="touch">
        <Sheet native modal snapPoints={[80, 50]}>
          <Sheet.Frame>
            <Sheet.ScrollView padding="$sm">
              <Adapt.Contents />
            </Sheet.ScrollView>
          </Sheet.Frame>
          <Sheet.Overlay />
        </Sheet>
      </Adapt>

      <TamaguiSelect.Content>
        <TamaguiSelect.Viewport>
          <TamaguiSelect.Group>
            {useMemo(
              () =>
                options.map((entry, i) => {
                  return (
                    <TamaguiSelect.Item
                      index={i}
                      key={entry.id}
                      value={entry.value}
                      gap="$3"
                    >
                      <TamaguiSelect.ItemText width={"90%"} numberOfLines={1}>
                        {entry.value}
                      </TamaguiSelect.ItemText>
                      <TamaguiSelect.ItemIndicator>
                        <Icon icon="chevronLeft" />
                      </TamaguiSelect.ItemIndicator>
                    </TamaguiSelect.Item>
                  );
                }),
              [options],
            )}
          </TamaguiSelect.Group>
        </TamaguiSelect.Viewport>
      </TamaguiSelect.Content>
    </TamaguiSelect>
  );
};

export default Select;
