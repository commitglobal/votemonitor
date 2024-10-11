import { Icon } from "./Icon";
import { Input, XStack, styled } from "tamagui";
import i18n from "../common/config/i18n";
import { useState, useCallback, useEffect } from "react";

interface SearchInputProps {
  onSearch: (value: string) => void;
}

// TODO: Update this to reuse in all the application
const Search = ({ onSearch }: SearchInputProps) => {
  const [searchTerm, setSearchTerm] = useState<string>("");

  const debouncedSearch = useCallback(
    (value: string) => {
      const timer = setTimeout(() => {
        onSearch(value);
      }, 500);
      return () => clearTimeout(timer);
    },
    [onSearch],
  );

  useEffect(() => {
    const cancelDebounce = debouncedSearch(searchTerm);
    return cancelDebounce;
  }, [searchTerm, debouncedSearch]);

  return (
    <XStack backgroundColor="white" borderRadius={8} alignItems="center">
      <Icon icon="search" color="transparent" size={20} marginLeft="$sm" />
      <SearchInput
        flex={1}
        value={searchTerm}
        onChangeText={setSearchTerm}
        maxFontSizeMultiplier={1.2}
      />
    </XStack>
  );
};

const SearchInput = styled(Input, {
  backgroundColor: "white",
  placeholder: i18n.t("search", { ns: "common" }),
  color: "$gray5",
  placeholderTextColor: "$gray5",
  focusStyle: {
    borderColor: "transparent",
  },
  borderWidth: 0,
});

export default Search;
