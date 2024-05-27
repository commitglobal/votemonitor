import {
  Body,
  Container,
  Head,
  Heading,
  Hr,
  Html,
  Img,
  Link,
  Preview,
  Section,
  Text
} from "@react-email/components";
import * as React from "react";
import { Layout } from "./components/Layout";

interface ExistingUserInvitationEmailProps {
  name: string;
  confirmUrl: string;
  cdnUrl: string;
  ngoName: string;
  electionRoundDetails: string;
}

export const ExistingUserInvitationEmail = ({
  name = '~$name$~',
  confirmUrl = '~$confirmUrl$~',
  cdnUrl = '~$cdnUrl$~',
  ngoName = '~$ngoName$~',
  electionRoundDetails = '~$electionRoundDetails$~',
}: ExistingUserInvitationEmailProps) => (
  <Layout name={name}
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
      You may install the app from <Link href="#">Google Play</Link> or <Link href="#">Apple Store</Link>.
    </Text>

  </Layout>

);

ExistingUserInvitationEmail.PreviewProps = {
  cdnUrl: "",
  ngoName: "NGO name",
  electionRoundDetails: "Example election round 2024",
} as ExistingUserInvitationEmailProps;

export default ExistingUserInvitationEmail;

const main = {
  backgroundColor: "#ffffff",
  fontFamily:
    '-apple-system,BlinkMacSystemFont,"Segoe UI",Roboto,Oxygen-Sans,Ubuntu,Cantarell,"Helvetica Neue",sans-serif',
};

const container = {
  margin: "0 auto",
  padding: "20px 25px 48px",
  backgroundPosition: "bottom",
  backgroundRepeat: "no-repeat, no-repeat",
};

const heading = {
  fontSize: "28px",
  fontWeight: "bold",
  marginTop: "48px",
};

const body = {
  margin: "24px 0",
};

const paragraph = {
  fontSize: "16px",
  lineHeight: "26px",
};

const link = {
  color: "#FF6363",
};

const hr = {
  borderColor: "#dddddd",
  marginTop: "48px",
};

const footer = {
  color: "#8898aa",
  fontSize: "12px",
  marginLeft: "4px",
};
