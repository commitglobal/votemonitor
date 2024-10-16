import { Section, Tailwind, Text } from "@react-email/components";
import * as React from "react";
import AttachmentFragment from "./attachment-fragment";

interface AnswerAttachmentsFragmentProps {
  listTitle: string;
  attachments: string[];
}

export const AnswerAttachmentsFragment = ({
  listTitle = "~$listTitle$~",
  attachments = [],
}: AnswerAttachmentsFragmentProps) => (
  <Tailwind>
    <Section>
      <Text className="font-semibold">{listTitle}</Text>
      <Section>
        {attachments.length > 0
          ? attachments.map((attachment, idx) => (
              <AttachmentFragment
                link={attachment}
                linkText={"Attachment " + (idx + 1)}
                key={idx}
              />
            ))
          : "~$attachments$~"}
      </Section>
    </Section>
  </Tailwind>
);

AnswerAttachmentsFragment.PreviewProps = {
  listTitle: "Attachments:",
  attachments: [
    "https://fastly.picsum.photos/id/1/5000/3333.jpg?hmac=Asv2DU3rA_5D1xSe22xZK47WEAN0wjWeFOhzd13ujW4",
    "https://fastly.picsum.photos/id/0/5000/3333.jpg?hmac=_j6ghY5fCfSD6tvtcV74zXivkJSPIfR9B8w34XeQmvU",
  ],
} as AnswerAttachmentsFragmentProps;

export default AnswerAttachmentsFragment;
