import { Heading, Link, Text } from "@react-email/components";
import * as React from "react";
import { Layout } from "./components/Layout";

interface ConfirmEmailProps {
  name: string;
  cdnUrl: string;
  confirmUrl: string;
}



export const ConfirmEmail = ({
  name = '~$name$~',
  confirmUrl = '~$confirmUrl$~',
  cdnUrl = '~$cdnUrl$~'
}: ConfirmEmailProps) => (
  <Layout
    name={name}
    cdnUrl={cdnUrl}
    preview={`Confirm your email on VoteMonitor platform`}
  >
    <Text className="text-base">
      This is an automatic email that was sent to confirm your account. Please follow <Link href={confirmUrl}>this link</Link> to complete your registration and to set your password in order to use the application.
    </Text>
  </Layout>
);

ConfirmEmail.PreviewProps = {
  confirmUrl: "https://example.com/",
  name: "John Doe",
  cdnUrl: "/static",
} as ConfirmEmailProps;

export default ConfirmEmail;
