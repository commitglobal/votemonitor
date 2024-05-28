import { Link, Text } from "@react-email/components";
import * as React from "react";
import { Layout } from "./components/Layout";

interface ResetPasswordEmailProps {
  name: string;
  resetPasswordUrl: string;
  cdnUrl: string;
}

export const ResetPasswordEmail = ({
  name = '~$name$~',
  resetPasswordUrl = '~$resetPasswordUrl$~',
  cdnUrl = '~$cdnUrl$~'
}: ResetPasswordEmailProps) => (
  <Layout
    name={name}
    cdnUrl={cdnUrl}
    preview={`Reset your password on VoteMonitor Platform.`}
  >
    <Text className="text-base">
      You are receiving this email because you requested help with your account credentials. In order to reset your password please follow <Link href={resetPasswordUrl}>this link</Link> and the instructions provided in the application.
    </Text>
  </Layout>
);

ResetPasswordEmail.PreviewProps = {
  resetPasswordUrl: "https://example.com/",
  cdnUrl: "/static",
  name: "John Doe"
} as ResetPasswordEmailProps;

export default ResetPasswordEmail;
