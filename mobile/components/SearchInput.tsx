import { Icon } from "./Icon";
import { Input, XStack, styled, Button } from "tamagui";
import { useState, useCallback, useEffect, useMemo } from "react";
import { useTranslation } from "react-i18next";

interface SearchInputProps {
  onSearch: (value: string) => void;
  placeholder?: string;
}

// TODO: Update this to reuse in all the application
const Search = ({ onSearch, placeholder }: SearchInputProps) => {
  const [searchTerm, setSearchTerm] = useState<string>("");
  const { t } = useTranslation();

  const inputPlaceholder = useMemo(
    () => placeholder ?? t("search", { ns: "common" }),
    [t, placeholder],
  );

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

  const handleClearSearch = () => {
    setSearchTerm("");
    onSearch("");
  };

  return (
    <XStack backgroundColor="white" borderRadius={8} alignItems="center">
      <Icon icon="search" color="transparent" size={20} marginLeft="$sm" />
      <SearchInput
        flex={1}
        value={searchTerm}
        onChangeText={setSearchTerm}
        maxFontSizeMultiplier={1.2}
        placeholder={inputPlaceholder}
      />
      {searchTerm !== "" && (
        <Button
          size="$2"
          circular
          icon={<Icon icon="x" size={16} color="$gray5" />}
          onPress={handleClearSearch}
          backgroundColor="transparent"
          marginRight="$sm"
        />
      )}
    </XStack>
  );
};

const SearchInput = styled(Input, {
  backgroundColor: "white",
  color: "$gray5",
  placeholderTextColor: "$gray5",
  focusStyle: {
    borderColor: "transparent",
  },
  borderWidth: 0,
});

export default Search;
