import React from "react";
import { render } from "@testing-library/react-native";
import { Button, View } from "react-native";
import FormOverview from "../../components/FormOverview";
import { TamaguiProvider, createTamagui } from "tamagui";
import { config } from "@tamagui/config/v3";
import { themes } from "../../theme/themes";
import { fonts } from "../../theme/fonts";
import { tokens } from "../../theme/tokens";

export const tamaguiConfig = createTamagui({
  ...config,
  themes,
  fonts,
  tokens,
});

jest.mock("expo-secure-store", () => ({
  getItem: jest.fn().mockResolvedValue("en"),
  setItem: jest.fn().mockResolvedValue(undefined),
  deleteItem: jest.fn().mockResolvedValue(undefined),
}));

jest.mock("expo-localization", () => ({
  getLocales: jest.fn().mockReturnValue([{ languageCode: "en" }]),
}));

jest.mock("i18next", () => ({
  use: jest.fn().mockReturnThis(),
  init: jest.fn().mockReturnThis(),
}));

jest.mock("react-i18next", () => ({
  initReactI18next: {
    type: "3rdParty",
    init: jest.fn(),
  },
  useTranslation: () => ({
    t: (key: string) => key,
    i18n: {
      changeLanguage: jest.fn(),
    },
  }),
}));

jest.mock("tamagui", () => ({
  createTamagui: jest.fn(),
  TamaguiProvider: ({ children }: { children: React.ReactNode }) => children,
  styled: jest.fn(),
  createFont: jest.fn(),
  createTokens: jest.fn(),
  useWindowDimensions: jest.fn().mockReturnValue({ width: 360, height: 640 }),
  // TamaguiCard: ({ children }: { children: React.ReactNode }) => <>{children}</>,
}));

describe("Random Tests", () => {
  it("has 1 child", () => {
    const tree = render(<Button title="Test" />).toJSON();
    expect(tree?.children?.length).toBe(1);
  });

  // it("renders correctly", () => {
  //   const tree = render(
  //     <FormOverview completedAnswers={5} numberOfQuestions={10} onFormActionClick={() => {}} />,
  //   ).toJSON();
  //   expect(tree).toMatchSnapshot();
  // });
});
