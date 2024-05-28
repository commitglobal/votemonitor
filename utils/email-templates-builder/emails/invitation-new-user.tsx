import { Link, Text } from "@react-email/components";
import * as React from "react";
import { Layout } from "./components/Layout";

interface NewUserInvitationEmailProps {
  name: string;
  confirmUrl: string;
  cdnUrl: string;
  ngoName: string;
  electionRoundDetails: string;
}

export const NewUserInvitationEmail = ({
  name = '~$name$~',
  confirmUrl = '~$confirmUrl$~',
  cdnUrl = '~$cdnUrl$~',
  ngoName = '~$ngoName$~',
  electionRoundDetails = '~$electionRoundDetails$~'
}: NewUserInvitationEmailProps) => (
  <Layout
    name={name}
    cdnUrl={cdnUrl}
    preview={`${ngoName} has invited you to be an observer for ${electionRoundDetails}.`}
  >
    <Text className="text-base">
      Thank you for your decision to be an independent observer! We are very glad and grateful to have you here.
    </Text>

    <Text className="text-base">
      {ngoName} has invited you to be an observer for {electionRoundDetails}.
    </Text>

    <Text className="text-base">
      Please follow <Link href={confirmUrl}>this link</Link> to complete your registration and to set your password in order to use the application.
    </Text>

    <Text className="text-base">
      You may install the app from <Link href='https://play.google.com/store/apps/details?id=org.commitglobal.votemonitor.app'>Google Play</Link> or <Link href='https://apps.apple.com/ro/app/vote-monitor/id6478601394'>Apple Store</Link>.
    </Text>
  </Layout>
);

NewUserInvitationEmail.PreviewProps = {
  cdnUrl: "/static",
  name: "John Doe",
  ngoName: "NGO name",
  electionRoundDetails: "Example election round 2024",
} as NewUserInvitationEmailProps;

export default NewUserInvitationEmail;
