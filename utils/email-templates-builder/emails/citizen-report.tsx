import {
  Body,
  Container,
  Head,
  Heading,
  Hr,
  Html,
  Img,
  Preview,
  Section,
  Tailwind,
  Text,
} from "@react-email/components";
import * as React from "react";
import { InputAnswerFragment } from "./answers-fragments/input-answer-fragment";
import { RatingAnswerFragment } from "./answers-fragments/rating-answer-fragment";
import { RatingAnswerOptionFragment } from "./answers-fragments/rating-answer-option-fragment";
import { RatingAnswerOptionSelectedFragment } from "./answers-fragments/rating-answer-option-selected-fragment";
import { SelectAnswerFragment } from "./answers-fragments/select-answer-fragment";
import { SelectAnswerOptionFragment } from "./answers-fragments/select-answer-option-fragment";
import { SelectAnswerOptionSelectedFragment } from "./answers-fragments/select-answer-option-selected-fragment";

interface CitizenReportEmailProps {
  heading: string;
  preview: string;
  cdnUrl: string;
  answers: React.ReactNode[];
}

export const CitizenReportEmail = ({
  heading = "~$heading$~",
  preview = "~preview~",
  cdnUrl = "~$cdnUrl$~",
  answers = [],
}: CitizenReportEmailProps) => (
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
            <Heading as="h2">{heading}</Heading>

            {answers.length > 0
              ? answers.map((answer) => answer)
              : "~$answers$~"}
          </Section>

          <Hr />

          <Text className="text-sm text-gray-500">
            &copy; {new Date().getFullYear()} Commit Global. All rights
            reserved.
          </Text>
        </Container>
      </Body>
    </Tailwind>
  </Html>
);

CitizenReportEmail.PreviewProps = {
  cdnUrl: "/static",
  heading: "Incident type nr: ############",
  preview: "Thank you for your submission",
  answers: [
    <InputAnswerFragment
      text="1. Text question"
      answer="hello world"
      key={1}
    />,
    <InputAnswerFragment text="2. Number question" answer="322" key={2} />,
    <InputAnswerFragment text="3. Date question" answer="2024-01-01" key={3} />,
    <RatingAnswerFragment
      key={4}
      text="4. Rating question"
      options={[
        <RatingAnswerOptionFragment value="1" key={1} />,
        <RatingAnswerOptionSelectedFragment value="2" key={2} />,
      ]}
    />,
    <SelectAnswerFragment
      key={5}
      text="5. Select question"
      options={[
        <SelectAnswerOptionFragment value={"option 1"} key={1} />,
        <SelectAnswerOptionSelectedFragment value={"option 2"} key={2} />,
      ]}
    />,
  ],
} as CitizenReportEmailProps;

export default CitizenReportEmail;
