import { createContext, useContext } from "react";
import i18n from "../../common/config/i18n";

export type Language = "en" | "ro";

type LanguageContextType = {
  changeLanguage: (language: Language) => void;
};

export const LanguageContext = createContext<LanguageContextType>({
  changeLanguage: (_language: Language) => {},
});

const LanguageContextProvider = ({ children }: React.PropsWithChildren) => {
  const changeLanguage = (lng: Language) => {
    if (i18n.language !== lng) {
      i18n.changeLanguage(lng);
    }
  };

  return (
    <LanguageContext.Provider
      value={{
        changeLanguage,
      }}
    >
      {children}
    </LanguageContext.Provider>
  );
};

export const useLanguage = () => {
  return useContext(LanguageContext);
};

export default LanguageContextProvider;
