import {
  Body,
  Container,
  Head,
  Heading,
  Html,
  Img,
  Link,
  Preview,
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
    <Preview>Confirm your account on VoteMonitor platform</Preview>
    <Body style={main}>
      <Container style={container}>
        <Img src={`${cdnUrl}/static/logo.png`} alt="logo" width="150" height="150" style={container} />
        <Heading style={h1}>Confirm your account on VoteMonitor platform</Heading>
        <Link
          href={confirmUrl}
          target="_blank"
          style={{
            ...link,
            display: "block",
            marginBottom: "16px",
          }}
        >
          Click here to confirm your email address
        </Link>
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
};

const container = {
  paddingLeft: "12px",
  paddingRight: "12px",
  margin: "0 auto",
};

const h1 = {
  color: "#333",
  fontFamily:
    "-apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen', 'Ubuntu', 'Cantarell', 'Fira Sans', 'Droid Sans', 'Helvetica Neue', sans-serif",
  fontSize: "24px",
  fontWeight: "bold",
  margin: "40px 0",
  padding: "0",
};

const link = {
  color: "#2754C5",
  fontFamily:
    "-apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen', 'Ubuntu', 'Cantarell', 'Fira Sans', 'Droid Sans', 'Helvetica Neue', sans-serif",
  fontSize: "14px",
  textDecoration: "underline",
};