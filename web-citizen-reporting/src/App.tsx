const testData = {
  electionRoundId: "39e06a94-a201-4385-aeba-9a804452d89f",
  version: "970ec83f-bfaf-49db-a927-0e1266f54d0f",
  forms: [
    {
      description: {
        EN: "aaa",
      },
      id: "b315c55b-df03-43bc-aca3-70f6d2cb0a1a",
      formType: "CitizenReporting",
      code: "CR1",
      name: {
        EN: "Citizen reporting test",
      },
      status: "Published",
      defaultLanguage: "EN",
      languages: ["EN"],
      numberOfQuestions: 2,
      icon: null,
      languagesTranslationStatus: {
        EN: "Translated",
      },
      questions: [
        {
          inputPlaceholder: {
            EN: "",
          },
          $questionType: "textQuestion",
          id: "43c0e5b0-1e1e-4031-90e0-31dd7d87be59",
          code: "1",
          text: {
            EN: "How are you feeling today?",
          },
          helptext: {
            EN: "",
          },
          displayLogic: null,
        },
        {
          lowerLabel: {
            EN: "",
          },
          upperLabel: {
            EN: "",
          },
          scale: "OneTo5",
          $questionType: "ratingQuestion",
          id: "4fa8b2db-c7d3-48d3-8b34-a29b54deeea8",
          code: "2",
          text: {
            EN: "How good was your morning coffee?",
          },
          helptext: {
            EN: "",
          },
          displayLogic: null,
        },
      ],
    },
  ],
};

function App() {
  return <></>;
}

export default App;
