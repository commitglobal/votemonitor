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

interface ResetPasswordEmailProps {
  resetPasswordUrl: string;
  cdnUrl: string;
}

export const ResetPasswordEmail = ({
  resetPasswordUrl = '~$resetPasswordUrl$~',
  cdnUrl = '~$cdnUrl$~'
}: ResetPasswordEmailProps) => (
  <Html>
    <Head />
    <Preview>Reset your password on VoteMonitor Platform</Preview>
    <Body style={main}>
      <Container style={container}>
        <Heading style={heading}>Reset your password on VoteMonitor Platform</Heading>
        <Section style={body}>
          <Text style={paragraph}>
            Hello,
          </Text>
          <Text style={paragraph}>
            You are receiving this email because you requested help with your account credentials.
          </Text>
          <Text style={paragraph}>
            In order to reset your password please follow
            <Link style={link} href={resetPasswordUrl}>👉 this link👈</Link>
            and the instructions provided in the application.
          </Text>
          <Text style={paragraph}>
            If you don&apos;t want to change your password or didn&apos;t
            request this, just ignore and delete this message.
          </Text>
          <Text style={paragraph}>
            To keep your account secure, please don&apos;t forward this email
            to anyone.
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

ResetPasswordEmail.PreviewProps = {
  resetPasswordUrl: "https://example.com/",
  cdnUrl: ""
} as ResetPasswordEmailProps;

export default ResetPasswordEmail;

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