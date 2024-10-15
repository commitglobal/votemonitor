import { Text } from "@react-email/components";
import * as React from "react";

interface SelectAnswerOptionFragmentProps {
  value: string;
}

export const SelectAnswerOptionFragment = ({
  value = "~$text$~",
}: SelectAnswerOptionFragmentProps) => (
  <tr>
    <td align="left">
      <div className="h-[20px] w-[20px] rounded-[8px] border border-solid border-gray-300 p-[8px] font-semibold"></div>
    </td>
    <td>
      <Text className="ml-[16px]">{value}</Text>
    </td>
  </tr>
);

SelectAnswerOptionFragment.PreviewProps = {
  value: "1",
} as SelectAnswerOptionFragmentProps;

export default SelectAnswerOptionFragment;
