import { PersistQueryClientProvider } from "@tanstack/react-query-persist-client";
import {
  Mutation,
  MutationCache,
  QueryClient,
  defaultShouldDehydrateQuery,
} from "@tanstack/react-query";
import { createAsyncStoragePersister } from "@tanstack/query-async-storage-persister";
import AsyncStorage from "@react-native-async-storage/async-storage";
import { useAuth } from "../../hooks/useAuth";
import { pollingStationsKeys } from "../../services/queries.service";
import * as API from "../../services/definitions.api";
import { performanceLog } from "../../helpers/misc";
import { PersistGate } from "../../components/PersistGate";
import SuperJSON from "superjson";
import { AddAttachmentAPIPayload, addAttachment } from "../../services/api/add-attachment.api";
import {
  deleteAttachment,
  DeleteAttachmentAPIPayload,
} from "../../services/api/delete-attachment.api";

const queryClient = new QueryClient({
  mutationCache: new MutationCache({
    // There is also QueryCache
    onSuccess: (data: unknown) => {
      console.log("MutationCache ", data);
    },
    onError: (error: Error) => {
      // Will always fire, is not tied to a mutation
      // TODO: Send the error to Sentry
      console.log("MutationCache error ", error);
    },
  }),
  defaultOptions: {
    mutations: {
      /*
          Set the mutations Garbage Collection time to a very high number to avoid losing pending mutations

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
      gcTime: 5 * 24 * 60 * 60 * 1000, // 5 days
      onError: (err: Error) => {
        console.log(err);
        console.log("QueryClient - mutations: ", JSON.stringify(err));
      },
      // throwOnError: true,
    },
    queries: {
      /*

          Set the queries Garbage Collection time to a very high number to avoid losing pending queries


          TODO: probably will also need this, to allow user to close the app and reopen and still see the forms, data, etc 

      */

      gcTime: 5 * 24 * 60 * 60 * 1000, // 5 days The duration until inactive queries will be removed from the cache. This defaults to 5 minutes. Queries transition to the inactive state as soon as there are no observers registered, so when all components which use that query have unmounted.
      staleTime: 1 * 60 * 60 * 1000, // 1 hour The duration until a query transitions from fresh to stale. As long as the query is fresh, data will always be read from the cache only - no network request will happen! If the query is stale (which per default is: instantly), you will still get data from the cache, but a background refetch can happen under certain conditions.
    },
  },
});

// https://www.whitespectre.com/ideas/how-to-build-offline-first-react-native-apps-with-react-query-and-typescript/
// https://tanstack.com/query/v5/docs/framework/react/plugins/persistQueryClient
// https://tanstack.com/query/v4/docs/framework/react/plugins/createAsyncStoragePersister
const persister = createAsyncStoragePersister({
  storage: AsyncStorage,
  // throttleTime: 3000, // TODO: check what implications are here
  // key: "REACT_QUERY_OFFLINE_CACHE" // t  his is the default key
});

const PersistQueryContextProvider = ({ children }: React.PropsWithChildren) => {
  // https://tanstack.com/query/latest/docs/framework/react/plugins/persistQueryClient#useisrestoring
  // const isRestoring = useIsRestoring();
  // console.log("isRestoring persistQueryClient", isRestoring);
  const { isAuthenticated } = useAuth();

  // queryClient.getMutationCache().subscribe((event) => {
  //   console.log("👀", event);
  // });

  queryClient.setMutationDefaults([pollingStationsKeys.mutatePollingStationGeneralData()], {
    mutationFn: (payload: API.PollingStationInformationAPIPayload) => {
      return API.upsertPollingStationGeneralInformation(payload);
    },
  });

  queryClient.setMutationDefaults(pollingStationsKeys.upsertFormSubmission(), {
    mutationFn: (payload: API.FormSubmissionAPIPayload) => {
      return API.upsertFormSubmission(payload);
    },
  });

  queryClient.setMutationDefaults(pollingStationsKeys.addAttachmentMutation(), {
    mutationFn: async (payload: AddAttachmentAPIPayload) => {
      return performanceLog(() => addAttachment(payload));
    },
  });

  queryClient.setMutationDefaults(pollingStationsKeys.deleteAttachment(), {
    mutationFn: async (payload: DeleteAttachmentAPIPayload) => {
      return deleteAttachment(payload);
    },
  });

  queryClient.setMutationDefaults(pollingStationsKeys.addNote(), {
    mutationFn: (payload: API.NotePayload) => {
      return API.addNote(payload);
    },
  });

  queryClient.setMutationDefaults(pollingStationsKeys.updateNote(), {
    mutationFn: (payload: API.UpdateNotePayload) => {
      return API.updateNote(payload);
    },
  });

  queryClient.setMutationDefaults(pollingStationsKeys.deleteNote(), {
    mutationFn: (payload: API.DeleteNotePayload) => {
      return API.deleteNote(payload);
    },
  });

  if (!isAuthenticated) {
    return children;
  }

  const runPendingMutations = async () => {
    console.log(
      "PersistQueryClientProvider onSuccess - Successfully get data from AsyncStorage and put in MutationCache",
    );
    const pausedMutation = queryClient
      .getMutationCache()
      .getAll()
      .filter((mutation) => mutation.state.isPaused);

    console.log("🆕🆕🆕🆕🆕", SuperJSON.stringify(pausedMutation));

    const mergedMutations = pausedMutation.reduce(
      (acc: Record<string, Mutation<unknown, Error, void, unknown>>, mutation) => {
        const scopeId = mutation.options.scope?.id;

        if (!scopeId) {
          // Use mutationId as key if scope was not defined (nothing will merge here)
          acc[mutation.mutationId] = mutation;
          return acc;
        }

        if (scopeId && !acc[scopeId]) {
          acc[scopeId] = mutation;
          return acc;
        }

        if (mutation.state.submittedAt > acc[scopeId].state.submittedAt) {
          acc[scopeId] = mutation;
        }

        return acc;
      },
      {},
    );

    queryClient.getMutationCache().clear();

    Object.values(mergedMutations).forEach((mutation) => {
      queryClient.getMutationCache().add(mutation);
    });

    // const newPausedMutations = queryClient
    //   .getMutationCache()
    //   .getAll()
    //   .filter((mutation) => mutation.state.isPaused);

    // console.log("📍📍📍📍📍📍", SuperJSON.stringify(newPausedMutations));

    if (pausedMutation?.length) {
      await queryClient.resumePausedMutations(); // Looks in the inmemory cache
      queryClient.invalidateQueries(); // Avoid using await, not to wait for queries to refetch (maybe not the case here as there are no active queries)
      console.log("✅ Resume Paused Mutation & Invalidate Quries");
    }

    // await new Promise((resolve, _reject) => {
    //   setTimeout(() => {
    //     resolve(undefined);
    //   }, 10000);
    // });
  };

  return (
    <PersistQueryClientProvider
      onSuccess={async () => {
        await runPendingMutations();
      }}
      persistOptions={{
        persister,
        maxAge: 5 * 24 * 60 * 60 * 1000,
        dehydrateOptions: {
          shouldDehydrateQuery: (query) => {
            // SELECTIVELY PERSIST QUERY KEYS https://github.com/TanStack/query/discussions/3568
            // if (query.queryKey.includes("polling-stations-nomenclator")) return false;
            // if (query.queryKey.includes(null)) return false;
            // if (query.queryKey.includes(undefined)) return false;

            // if (query.meta?.dontPersist) return false;

            return defaultShouldDehydrateQuery(query);
          },
        },
      }}
      client={queryClient}
    >
      <PersistGate>{children}</PersistGate>
    </PersistQueryClientProvider>
  );
};

export default PersistQueryContextProvider;
