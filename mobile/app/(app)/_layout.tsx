import { Redirect, Stack } from "expo-router";
import { useAuth } from "../../hooks/useAuth";

const AppLayout = () => {
  const { isAuthenticated } = useAuth();
  // const theme = useTheme();

  if (!isAuthenticated) {
    // On web, static rendering will stop here as the user is not authenticated
    // in the headless Node process that the pages are rendered in.
    return <Redirect href="/login" />;
  }

  return (
    <Stack>
      <Stack.Screen name="(drawer)" options={{ headerShown: false }} />
      <Stack.Screen name="polling-station-wizzard" />
      <Stack.Screen name="form-questionnaire" />
      <Stack.Screen name="polling-station-questionnaire" />
    </Stack>
  );
};

// function CustomDrawerContent(props) {
//   const theme = useTheme();
//   return (
//     <DrawerContentScrollView {...props}>
//       {/* <DrawerItemList {...props} /> */}
//       {votingSessions.map((votingSession) => (
//         <DrawerItem
//           label={votingSession.name}
//           inactiveTintColor={theme.yellow6.val}
//           onPress={() => console.log("")}
//         />
//       ))}
//     </DrawerContentScrollView>
//   );
// }

export default AppLayout;

// const votingSessions = [
//   { name: "session 1" },
//   { name: "session2" },
//   { name: "session 3" },
// ];
