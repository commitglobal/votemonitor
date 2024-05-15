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
  Text,
} from "@react-email/components";
import * as React from "react";

interface NotionMagicLinkEmailProps {
  cdnUrl: string;
  confirmUrl: string;
}

export const ConfirmEmail = ({
  confirmUrl = '~$confirmUrl$~',
  cdnUrl = '~$cdnUrl$~'
}: NotionMagicLinkEmailProps) => (
  <Html>
    <Head />
    <Preview>Confirm your email on VoteMonitor platform.</Preview>
    <Body style={main}>
      <Container style={container}>
        <Heading style={heading}>Confirm your email on VoteMonitor platform.</Heading>
        <Section style={body}>
          <Text style={paragraph}>
            Hello!
          </Text>
          <Text style={paragraph}>
            This is an automatic email that was sent to confirm your account.
          </Text>
          <Text style={paragraph}>
            Please follow
            <Link style={link} href={confirmUrl}>👉 this link👈</Link>
            to complete your registration in order to use the application.
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

ConfirmEmail.PreviewProps = {
  confirmUrl: "https://example.com/",
  cdnUrl: "",
} as NotionMagicLinkEmailProps;

export default ConfirmEmail;

const main = {
  backgroundColor: "#ffffff",
  fontFamily:
    '-apple-system,BlinkMacSystemFont,"Segoe UI",Roboto,Oxygen-Sans,Ubuntu,Cantarell,"Helvetica Neue",sans-serif',
};

const container = {
  margin: "0 auto",
  padding: "20px 25px 48px",
  backgroundImage: 'url("/assets/raycast-bg.png")',
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