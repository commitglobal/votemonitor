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
  Tailwind,
} from "@react-email/components";
import * as React from "react";


interface LayoutProps {
  name: string;
  preview: string;
  cdnUrl: string;
  children: React.ReactNode;
}

export const Layout = ({ name, preview, cdnUrl, children }: LayoutProps) => (
  <Html>
    <Tailwind>
      <Head />
      <Preview>{preview}</Preview>
      <Body className="px-2 mx-auto my-auto font-sans bg-white">
        <Container className="my-[20px] sm:my-[40px] mx-auto p-[10px] sm:p-[20px] max-w-[640px]">
          <Section className="flex px-6 py-8 bg-purple-800 sm:pb-12 sm:px-10 sm:pt-14">
            <Img className="w-48 sm:w-64" src={`${cdnUrl}/logo.png`} />
          </Section>

          <Section className="px-6 py-8 sm:px-12 sm:pt-14">
            <Heading as="h2">Hello, {name}</Heading>

            {children}

            <Text className="text-base">
              Thank you.
            </Text>

            <Text className="mt-10 text-gray-500">If you have any questions, reach out to us at <Link href="mailto:info@commitglobal.org">info@commitglobal.org</Link>.</Text>
          </Section>

          <Hr />

          <Text className="text-sm text-gray-500">
            &copy; {(new Date().getFullYear())} Commit Global. All rights reserved.
          </Text>
        </Container>
      </Body>
    </Tailwind>
  </Html>
);
