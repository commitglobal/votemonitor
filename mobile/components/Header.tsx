import { Header } from "@react-navigation/elements";
import { StyleSheet } from "react-native";

const AppHeader = () => <Header headerStyle={styles.header} title="My app" />;

const styles = StyleSheet.create({
  header: {
    backgroundColor: "purple",
  },
});

export default AppHeader;
