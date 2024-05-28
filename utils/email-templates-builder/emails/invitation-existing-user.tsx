import { Link, Text } from "@react-email/components";
import * as React from "react";
import { Layout } from "./components/Layout";

interface ExistingUserInvitationEmailProps {
  name: string;
  cdnUrl: string;
  ngoName: string;
  electionRoundDetails: string;
}

export const ExistingUserInvitationEmail = ({
  name = '~$name$~',
  cdnUrl = '~$cdnUrl$~',
  ngoName = '~$ngoName$~',
  electionRoundDetails = '~$electionRoundDetails$~'
}: ExistingUserInvitationEmailProps) => (
  <Layout
    name={name}
    cdnUrl={cdnUrl}
    preview={`${ngoName} has invited you to be an observer for ${electionRoundDetails}.`}
  >
    <Text className="text-base">
      Thank you for your service as an observer. We are glad to have you back.
    </Text>

    <Text className="text-base">
      {ngoName} has invited you to be an observer for {electionRoundDetails}.
    </Text>

    <Text className="text-base">
      Go to the Vote Monitor app and reactivate your account. Please make sure you have the latest version installed on your phone.
    </Text>

    <Text className="text-base">
      If you no longer have the app on your device, you may install it from <Link href='https://play.google.com/store/apps/details?id=org.commitglobal.votemonitor.app'>Google Play</Link> or <Link href='https://apps.apple.com/ro/app/vote-monitor/id6478601394'>Apple Store</Link>.
    </Text>
  </Layout>
);

ExistingUserInvitationEmail.PreviewProps = {
  cdnUrl: "/static",
  name: "John Doe",
  ngoName: "NGO name",
  electionRoundDetails: "Example election round 2024",
} as ExistingUserInvitationEmailProps;

export default ExistingUserInvitationEmail;
