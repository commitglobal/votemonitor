import { Text } from "@react-email/components";
import * as React from "react";

interface SelectAnswerOptionSelectedFragmentProps {
  value: string;
}

export const SelectAnswerOptionSelectedFragment = ({
  value = "~$value$~",
}: SelectAnswerOptionSelectedFragmentProps) => (
  <tr>
    <td align="center">
      <div className="h-[20px] w-[20px] rounded-[10px] border border-solid border-gray-300 bg-purple-800 p-[8px] font-semibold  text-white">
        X
      </div>
    </td>
    <td>
      <Text className="ml-[16px]">{value}</Text>
    </td>
  </tr>
);

SelectAnswerOptionSelectedFragment.PreviewProps = {
  value: "checked option",
} as SelectAnswerOptionSelectedFragmentProps;

export default SelectAnswerOptionSelectedFragment;
