import { BaseToast, ErrorToast } from "react-native-toast-message";

export const toastConfig = {
  success: (props) => (
    <BaseToast
      {...props}
      text2Style={{
        fontSize: 16,
        color: "white",
        lineHeight: 24,
        fontWeight: "500",
      }}
      text2NumberOfLines={5}
      contentContainerStyle={{ padding: 24 }}
      style={{
        borderLeftColor: "hsl(142, 71%, 27%)",
        height: "auto",
        backgroundColor: "whsl(142, 71%, 27%)",
      }}
    />
  ),
  error: (props) => (
    <ErrorToast
      {...props}
      text2Style={{
        fontSize: 16,
        color: "white",
        lineHeight: 24,
        fontWeight: "500",
      }}
      text2NumberOfLines={5}
      contentContainerStyle={{ padding: 24 }}
      style={{
        borderLeftWidth: 0,
        height: "auto",
        backgroundColor: "hsl(0, 74%, 42%)",
      }}
    />
  ),
};
