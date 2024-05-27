import { Link, Text } from "@react-email/components";
import * as React from "react";
import { Layout } from "./components/Layout";

interface ExistingUserInvitationEmailProps {
  name: string;
  phoneNumber: string;
  cdnUrl: string;
  ngoName: string;
  electionRoundDetails: string;
  googlePlayUrl: string;
  appleStoreUrl: string;
}

export const ExistingUserInvitationEmail = ({
  name = '~$name$~',
  phoneNumber = '~$phoneNumber$~',
  cdnUrl = '~$cdnUrl$~',
  ngoName = '~$ngoName$~',
  electionRoundDetails = '~$electionRoundDetails$~',
  googlePlayUrl = '~$googlePlayUrl$~',
  appleStoreUrl = '~$appleStoreUrl$~',
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
      If you no longer have the app on your device, you may install it from <Link href={googlePlayUrl}>Google Play</Link> or <Link href={appleStoreUrl}>Apple Store</Link>.
    </Text>

    <Text className="text-base">
      Should you require additional support you may call {phoneNumber}
    </Text>
  </Layout>
);

ExistingUserInvitationEmail.PreviewProps = {
  cdnUrl: "/static",
  name: "John Doe",
  ngoName: "NGO name",
  electionRoundDetails: "Example election round 2024",
  googlePlayUrl: "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
  appleStoreUrl: "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
  phoneNumber: "+1234567890"
} as ExistingUserInvitationEmailProps;

export default ExistingUserInvitationEmail;
