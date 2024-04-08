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

interface NewUserInvitationEmailProps {
  acceptUrl: string;
  cdnUrl: string;
}

export const NewUserInvitationEmail = ({
  acceptUrl = '~$acceptUrl$~',
  cdnUrl = '~$cdnUrl$~'
}: NewUserInvitationEmailProps) => (
  <Html>
    <Head />
    <Preview>You have been invited to VoteMonitor</Preview>
    <Body style={main}>
      <Container style={container}>
        <Img src={`${cdnUrl}/static/logo.png`} alt="logo" width="150" height="150" style={container}/>
        <Heading style={h1}>You have been invited to VoteMonitor</Heading>
        <Link
          href={acceptUrl}
          target="_blank"
          style={{
            ...link,
            display: "block",
            marginBottom: "16px",
          }}
        >
          Click here accept invitation
        </Link>
      </Container>
    </Body>
  </Html>
);

NewUserInvitationEmail.PreviewProps = {
  acceptUrl: "https://example.com/",
  cdnUrl: "",
} as NewUserInvitationEmailProps;

export default NewUserInvitationEmail;

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
