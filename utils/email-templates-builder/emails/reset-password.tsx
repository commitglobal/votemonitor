import {
  Body,
  Container,
  Head,
  Heading,
  Html,
  Img,
  Link,
  Preview
} from "@react-email/components";
import * as React from "react";

interface ResetPasswordEmailProps {
  resetPasswordUrl: string;
  cdnUrl: string;
}

export const ResetPasswordEmail = ({
  resetPasswordUrl= '~$resetPasswordUrl$~',
  cdnUrl = '~$cdnUrl$~'
}: ResetPasswordEmailProps) => (
  <Html>
    <Head />
    <Preview>Reset your password</Preview>
    <Body style={main}>
      <Container style={container}>
        <Img src={`${cdnUrl}/static/logo.png`} alt="Cat" width="150" height="150" style={container} />
        <Heading style={h1}>Reset your password</Heading>
        <Link
          href={resetPasswordUrl}
          target="_blank"
          style={{
            ...link,
            display: "block",
            marginBottom: "16px",
          }}
        >
          Click here to reset your password
        </Link>
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
