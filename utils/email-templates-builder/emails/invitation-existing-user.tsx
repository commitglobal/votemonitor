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

interface ExistingUserInvitationEmailProps {
  cdnUrl: string;
  ngoName: string;
  electionRoundDetails: string;
}

export const ExistingUserInvitationEmail = ({
  cdnUrl = '~$cdnUrl$~',
  ngoName = '~$ngoName$~',
  electionRoundDetails = '~$electionRoundDetails$~',
}: ExistingUserInvitationEmailProps) => (
  <Html>
    <Head />
    <Preview>{ngoName} has invited you to be an observer for {electionRoundDetails}.</Preview>
    <Body style={main}>
      <Container style={container}>
        <Heading style={heading}>{ngoName} has invited you to be an observer for {electionRoundDetails}.</Heading>
        <Section style={body}>
          <Text style={paragraph}>
            Hello,
          </Text>
          <Text style={paragraph}>
            Thank you for your service as an observer. We are glad to have you back.
          </Text>
          <Text style={paragraph}>
            {ngoName} has invited you to be an observer for {electionRoundDetails}.
          </Text>
          <Text style={paragraph}>
            Go to the Vote Monitor app and reactivate your account. Please make sure you have the latest version installed on your phone.
          </Text>
          <Text style={paragraph}>
            If you no longer have the app on your device, you may install it from:
            <ul>
              <li key="google"><Link style={link} href="https://google.com/">Google play</Link></li>
              <li key="apple"><Link style={link} href="https://apple.com/">Apple Store</Link></li>
            </ul>
          </Text>
          <Text style={paragraph}>
            Should you require additional support you may call +…………….
          </Text>
        </Section>
        <Text style={paragraph}>
          Thank you,
          <br />- Vote Monitor Team
        </Text>
        <Hr style={hr} />
        <Text style={footer}>Commit Global.</Text>
      </Container>
    </Body>
  </Html>
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