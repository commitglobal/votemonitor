import { Tailwind } from "@react-email/tailwind";
import * as React from "react";

interface RatingAnswerOptionFragmentProps {
  value: string;
}

export const RatingAnswerOptionFragment = ({
  value = "~$value$~",
}: RatingAnswerOptionFragmentProps) => (
  <Tailwind>
    <td align="center">
      <div className="h-[20px] w-[20px] rounded-[8px] border border-solid border-gray-300 p-[8px] font-semibold">
        {value}
      </div>
    </td>
  </Tailwind>
);

RatingAnswerOptionFragment.PreviewProps = {
  value: "1",
} as RatingAnswerOptionFragmentProps;

export default RatingAnswerOptionFragment;
