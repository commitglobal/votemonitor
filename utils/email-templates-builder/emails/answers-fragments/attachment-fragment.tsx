import { Link, Row } from "@react-email/components";
import * as React from "react";

interface AttachmentFragmentProps {
  link: string;
  linkText: string;
}

export const AttachmentFragment = ({
  link = "~$link$~",
  linkText = "~$linkText$~",
}: AttachmentFragmentProps) => (
  <Row>
    <Link href={link}>{linkText}</Link>
  </Row>
);

AttachmentFragment.PreviewProps = {
  link: "https://fastly.picsum.photos/id/0/5000/3333.jpg?hmac=_j6ghY5fCfSD6tvtcV74zXivkJSPIfR9B8w34XeQmvU",
  linkText: "Attachment 1",
} as AttachmentFragmentProps;

export default AttachmentFragment;
