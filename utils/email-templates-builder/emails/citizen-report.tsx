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
  preview = "~$preview$~",
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
      notes={[
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
      ]}
      attachments={[
        "https://fastly.picsum.photos/id/1/5000/3333.jpg?hmac=Asv2DU3rA_5D1xSe22xZK47WEAN0wjWeFOhzd13ujW4",
        "https://fastly.picsum.photos/id/0/5000/3333.jpg?hmac=_j6ghY5fCfSD6tvtcV74zXivkJSPIfR9B8w34XeQmvU",
      ]}
      key={1}
    />,
    <InputAnswerFragment
      text="2. Number question"
      answer="322"
      notes={["Nothing"]}
      attachments={[
        "https://fastly.picsum.photos/id/1/5000/3333.jpg?hmac=Asv2DU3rA_5D1xSe22xZK47WEAN0wjWeFOhzd13ujW4",
        "https://fastly.picsum.photos/id/0/5000/3333.jpg?hmac=_j6ghY5fCfSD6tvtcV74zXivkJSPIfR9B8w34XeQmvU",
      ]}
      key={2}
    />,
    <InputAnswerFragment
      text="3. Date question"
      answer="2024-01-01"
      notes={[
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
      ]}
      key={3}
      attachments={[
        "https://fastly.picsum.photos/id/1/5000/3333.jpg?hmac=Asv2DU3rA_5D1xSe22xZK47WEAN0wjWeFOhzd13ujW4",
        "https://fastly.picsum.photos/id/0/5000/3333.jpg?hmac=_j6ghY5fCfSD6tvtcV74zXivkJSPIfR9B8w34XeQmvU",
      ]}
    />,
    <RatingAnswerFragment
      key={4}
      text="4. Rating question"
      options={[
        <RatingAnswerOptionFragment value="1" key={1} />,
        <RatingAnswerOptionSelectedFragment value="2" key={2} />,
        <RatingAnswerOptionFragment value="3" key={3} />,
        <RatingAnswerOptionFragment value="4" key={4} />,
        <RatingAnswerOptionFragment value="5" key={5} />,
        <RatingAnswerOptionFragment value="6" key={6} />,
        <RatingAnswerOptionFragment value="7" key={7} />,
        <RatingAnswerOptionFragment value="8" key={8} />,
        <RatingAnswerOptionFragment value="9" key={9} />,
        <RatingAnswerOptionFragment value="10" key={10} />,
      ]}
      notes={[
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
      ]}
      attachments={[
        "https://fastly.picsum.photos/id/1/5000/3333.jpg?hmac=Asv2DU3rA_5D1xSe22xZK47WEAN0wjWeFOhzd13ujW4",
        "https://fastly.picsum.photos/id/0/5000/3333.jpg?hmac=_j6ghY5fCfSD6tvtcV74zXivkJSPIfR9B8w34XeQmvU",
      ]}
    />,
    <SelectAnswerFragment
      key={5}
      text="5. Select question"
      options={[
        <SelectAnswerOptionFragment value={"option 1"} key={1} />,
        <SelectAnswerOptionSelectedFragment value={"option 2"} key={2} />,
        <SelectAnswerOptionFragment value={"option 3"} key={3} />,
        <SelectAnswerOptionFragment value={"option 4"} key={4} />,

      ]}
      notes={[
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
      ]}
      attachments={[
        "https://fastly.picsum.photos/id/1/5000/3333.jpg?hmac=Asv2DU3rA_5D1xSe22xZK47WEAN0wjWeFOhzd13ujW4",
        "https://fastly.picsum.photos/id/0/5000/3333.jpg?hmac=_j6ghY5fCfSD6tvtcV74zXivkJSPIfR9B8w34XeQmvU",
      ]}
    />,
  ],
} as CitizenReportEmailProps;

export default CitizenReportEmail;
