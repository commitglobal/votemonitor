import {
  StarIcon,
  GroupIcon,
  TextIcon,
  ListBulletIcon
} from "@radix-ui/react-icons";
import { v4 as uuidv4 } from 'uuid';
import { TFormQuestionType } from "@/redux/api/types";

export type TFormQuestion = {
  id: string;
  label: string;
  description: string;
  icon: any;
  preset: any;
};

export const questionTypes: TFormQuestion[] = [
  {
    id: TFormQuestionType.OpenText,
    label: "Free text",
    description: "A single line of text",
    icon: TextIcon,
    preset: {
      code: "",
      headline: "Who let the dogs out?",
      subheader: "Who? Who? Who?",
      placeholder: "Type your answer here...",
      longAnswer: true,
    },
  },
  {
    id: TFormQuestionType.MultipleChoiceSingle,
    label: "Single-Select",
    description: "A single choice from a list of options (radio buttons)",
    icon: GroupIcon,
    preset: {
      code: "",
      headline: "What do you do?",
      subheader: "Can't do both.",
      choices: [
        { id: uuidv4(), label: "Eat the cake 🍰" },
        { id: uuidv4(), label: "Have the cake 🎂" },
      ],
    },
  },
  {
    id: TFormQuestionType.MultipleChoiceMulti,
    label: "Multi-Select",
    description: "Number of choices from a list of options (checkboxes)",
    icon: ListBulletIcon,
    preset: {
      code: "",
      headline: "What's important on vacay?",
      choices: [
        { id: uuidv4(), label: "Sun ☀️" },
        { id: uuidv4(), label: "Ocean 🌊" },
        { id: uuidv4(), label: "Palms 🌴" },
      ],
    },
  },
  {
    id: TFormQuestionType.Rating,
    label: "Rating",
    description: "Ask your users to rate something",
    icon: StarIcon,
    preset: {
      code: "",
      headline: "How would you rate ...",
      subheader: "Don't worry, be honest.",
      range: 5,
      lowerLabel: "Not good",
      upperLabel: "Very good",
    },
  }
];

export const universalQuestionPresets = {
  required: true,
};

export const getQuestionDefaults = (id: string) => {
  const questionType = questionTypes.find((questionType) => questionType.id === id);
  return questionType?.preset;
};

export const getTFormQuestionTypeName = (id: string) => {
  const questionType = questionTypes.find((questionType) => questionType.id === id);
  return questionType?.label;
};
