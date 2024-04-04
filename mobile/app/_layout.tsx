import { Slot } from "expo-router";
import AuthContextProvider from "../contexts/auth/AuthContext.provider";
import { createAsyncStoragePersister } from "@tanstack/query-async-storage-persister";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { PersistQueryClientProvider } from "@tanstack/react-query-persist-client";
import { useEffect, useState } from "react";
import NetInfo from "@react-native-community/netinfo";
import OfflineBanner from "../components/OfflineBanner";
import {
  QueryClient,
  onlineManager,
  useIsRestoring,
} from "@tanstack/react-query";
import { TamaguiProvider } from "@tamagui/core";
import { tamaguiConfig } from "../tamagui.config";
import { useFonts } from "expo-font";
import Reactotron from "reactotron-react-native";

if (__DEV__) {
  Reactotron.setAsyncStorageHandler(AsyncStorage)
    .configure({
      host: "192.168.68.58", // PUT YOUR OWN LOCAL IP (logged by Expo)
    }) // controls connection & communication settings
    .useReactNative({
      networking: {},
    }) // add all built-in react native plugins
    .connect(); // let's connect!
}

const queryClient = new QueryClient({
  defaultOptions: {
    mutations: {
      /*

          Set the mutations Garbage Collection time to a very high number to avoid losing pending mutations

          TODO: Do we need to cache queries? Probably NOT!
          TODO: To check with AsyncStorage how it works and when the GC kicks in

          If it is not set when creating the QueryClient instance,
          it will default to 300000 (5 minutes) for hydration, and the stored cache will be discarded after 5 minutes of inactivity.
          This is the default garbage collection behavior.

          It should be set as the same value or higher than persistQueryClient's maxAge option.
          E.g. if maxAge is 24 hours (the default) then gcTime should be 24 hours or higher.
          If lower than maxAge, garbage collection will kick in and discard the stored cache earlier than expected.

        
          TODO DANGER: Mutations are discarded if the API returns error. 
            - How to keep it?
            - There is a failureCount, failureReason, when are those populated?
          https://tanstack.com/query/v5/docs/framework/react/plugins/persistQueryClient#removal


          TODO TESTING: Can go online/offline to simulate this by using onlineManager.setOnline(true/false)

      */
      gcTime: 1000 * 60 * 60 * 24, // 24h - maybe need infity here
    },
    queries: {
      /*

          Set the queries Garbage Collection time to a very high number to avoid losing pending queries


          TODO: probably will also need this, to allow user to close the app and reopen and still see the forms, data, etc 

      */
      gcTime: 1000 * 60 * 60 * 24,
    },
  },
});

// https://www.whitespectre.com/ideas/how-to-build-offline-first-react-native-apps-with-react-query-and-typescript/
// https://tanstack.com/query/v5/docs/framework/react/plugins/persistQueryClient
// https://tanstack.com/query/v4/docs/framework/react/plugins/createAsyncStoragePersister
const persister = createAsyncStoragePersister({
  storage: AsyncStorage,
  throttleTime: 3000, // TODO: check what implications are here
  // key: "REACT_QUERY_OFFLINE_CACHE" // this is the default key
});

export default function Root() {
  const [isOnline, setIsOnline] = useState(true);

  const [loaded] = useFonts({
    Roboto: require("../assets/fonts/Roboto-Medium.ttf"),
    RobotoRegular: require("../assets/fonts/Roboto-Regular.ttf"),
    RobotoBold: require("../assets/fonts/Roboto-Bold.ttf"),
    DMSans: require("../assets/fonts/DMSans-Medium.ttf"),
    DMSansRegular: require("../assets/fonts/DMSans-Regular.ttf"),
    DMSansBold: require("../assets/fonts/DMSans-Bold.ttf"),
  });

  useEffect(() => {
    if (loaded) {
      // can hide splash screen here
    }
  }, [loaded]);

  useEffect(() => {
    return NetInfo.addEventListener((state) => {
      const status = !!state.isConnected;
      setIsOnline(status);
      onlineManager.setOnline(status);
    });
  }, []);

  // https://tanstack.com/query/latest/docs/framework/react/plugins/persistQueryClient#useisrestoring
  const isRestoring = useIsRestoring();
  console.log("isRestoring persistQueryClient", isRestoring);

  if (!loaded) {
    return null;
  }

  return (
    <PersistQueryClientProvider
      onSuccess={async () => {
        queryClient
          .resumePausedMutations()
          .then(() => queryClient.invalidateQueries());
      }}
      persistOptions={{ persister }}
      client={queryClient}
    >
      <TamaguiProvider config={tamaguiConfig}>
        <AuthContextProvider>
          {!isOnline && <OfflineBanner />}
          <Slot />
        </AuthContextProvider>
      </TamaguiProvider>
    </PersistQueryClientProvider>
  );
}
