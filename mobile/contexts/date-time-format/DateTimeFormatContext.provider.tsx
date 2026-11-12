import { createContext, useContext, useMemo, useState } from "react";
import { SECURE_STORAGE_KEYS } from "../../common/constants";
import {
  DATE_FORMATS,
  DateFormat,
  DEFAULT_DATE_FORMAT,
  formatDate,
  formatTime,
} from "../../common/utils/time";
import { getSecureStoreItem, setSecureStoreItem } from "../../helpers/SecureStoreWrapper";

type DateTimeFormatContextType = {
  // times are shown on a 24-hour clock unless the user picked the 12-hour one in settings
  is24Hour: boolean;
  setIs24Hour: (is24Hour: boolean) => void;
  // dates are shown as YYYY-MM-DD unless the user picked another format in settings
  dateFormat: DateFormat;
  setDateFormat: (dateFormat: DateFormat) => void;
  formatTime: (value: Date) => string;
  formatDate: (value: Date) => string;
};

export const DateTimeFormatContext = createContext<DateTimeFormatContextType>({
  is24Hour: true,
  setIs24Hour: (_is24Hour: boolean) => {},
  dateFormat: DEFAULT_DATE_FORMAT,
  setDateFormat: (_dateFormat: DateFormat) => {},
  formatTime: (value: Date) => formatTime(value, true),
  formatDate: (value: Date) => formatDate(value, DEFAULT_DATE_FORMAT),
});

const getStoredDateFormat = (): DateFormat => {
  const stored = getSecureStoreItem(SECURE_STORAGE_KEYS.DATE_FORMAT);
  return DATE_FORMATS.find((dateFormat) => dateFormat === stored) ?? DEFAULT_DATE_FORMAT;
};

const DateTimeFormatContextProvider = ({ children }: React.PropsWithChildren) => {
  const [is24Hour, setIs24HourState] = useState(
    () => getSecureStoreItem(SECURE_STORAGE_KEYS.CLOCK_FORMAT) !== "12h",
  );
  const [dateFormat, setDateFormatState] = useState(getStoredDateFormat);

  const setIs24Hour = (value: boolean) => {
    setIs24HourState(value);
    setSecureStoreItem(SECURE_STORAGE_KEYS.CLOCK_FORMAT, value ? "24h" : "12h");
  };

  const setDateFormat = (value: DateFormat) => {
    setDateFormatState(value);
    setSecureStoreItem(SECURE_STORAGE_KEYS.DATE_FORMAT, value);
  };

  // a new value only when a format changes, so consumers don't re-render with every root render
  const value = useMemo(
    () => ({
      is24Hour,
      setIs24Hour,
      dateFormat,
      setDateFormat,
      formatTime: (date: Date) => formatTime(date, is24Hour),
      formatDate: (date: Date) => formatDate(date, dateFormat),
    }),
    [is24Hour, dateFormat],
  );

  return <DateTimeFormatContext.Provider value={value}>{children}</DateTimeFormatContext.Provider>;
};

export const useDateTimeFormat = () => {
  return useContext(DateTimeFormatContext);
};

export default DateTimeFormatContextProvider;
