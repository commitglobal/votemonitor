import { Tailwind } from "@react-email/tailwind";
import * as React from "react";

interface RatingAnswerOptionSelectedFragmentProps {
  value: string;
}

export const RatingAnswerOptionSelectedFragment = ({
  value = "~$value$~",
}: RatingAnswerOptionSelectedFragmentProps) => (
  <Tailwind>
    <td align="center">
      <div className="h-[20px] w-[20px] rounded-[10px] border border-solid border-gray-300 bg-purple-800 p-[8px] font-semibold  text-white">
        {value}
      </div>
    </td>
  </Tailwind>
);

RatingAnswerOptionSelectedFragment.PreviewProps = {
  value: "1",
} as RatingAnswerOptionSelectedFragmentProps;

export default RatingAnswerOptionSelectedFragment;
