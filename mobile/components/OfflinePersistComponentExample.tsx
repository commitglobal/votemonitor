import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import AsyncStorage from "@react-native-async-storage/async-storage";
import * as API from "../services/test-todo/test-todo.api";
import { Text } from "react-native";

const OfflinePersistComponentExample = () => {
  const queryClient = useQueryClient();

  const { data } = useQuery({
    queryKey: ["todos"],
    queryFn: () => API.getTodos(),
    // staleTime and cacheTime as Infinity, ensures that once the data is fetched and cached,
    // it remains available for the lifetime of the application, regardless of network connectivity.
    staleTime: Infinity,
    gcTime: Infinity, // cacheTime before v5
    // cacheTime: Infinity,
  });

  const updateLocalTodosList = (
    id: string,
    status: "New" | "In progress" | "Done",
    isNotSynched: boolean
  ) => {
    queryClient.setQueryData<API.TodoVM[] | undefined>(
      ["todos"],
      (todos: API.TodoVM[] | undefined) => {
        return todos?.map((todo) => {
          if (todo._id === id) {
            return { ...todo, status, isNotSynched };
          }
          return todo;
        });
      }
    );
  };

  const updateExercise = useMutation({
    mutationKey: ["todos"],
    mutationFn: async (payload: API.UpdateTodoAPI) => API.updateStatus(payload),
    onMutate: async (payload: API.UpdateTodoAPI) => {
      // onMutate is called every time the mutation is about to happen,
      // so as soon as the user pressed the button to change the status of the exercise,
      // queryClient.setQueryData updates the cached data and the local state of it.
      console.log("OnMutate");
      const asyncStorage = await AsyncStorage.getItem(
        "REACT_QUERY_OFFLINE_CACHE"
      );
      console.log("asyncStorage", asyncStorage);
      await queryClient.cancelQueries({ queryKey: ["todos"] });
      updateLocalTodosList(payload.id, payload.status, true); // we mark the mutation as not being synched "isSynched: false"
    },
    onSuccess(data: any) {
      console.log("Success");
      AsyncStorage.getItem("REACT_QUERY_OFFLINE_CACHE").then(console.log);
      updateLocalTodosList(data._id, data.status, false); // here the mutation is synched, all good
    },
    onError(error) {
      console.log(error);
      AsyncStorage.getItem("REACT_QUERY_OFFLINE_CACHE").then(console.log);
    },
  });

  return (
    <>
      {data?.map((todo: API.TodoVM) => (
        <Text
          onPress={() => updateExercise.mutate({ id: todo._id, status: "New" })}
          key={todo._id}
        >
          {todo.title} "{todo.status}" {todo.isNotSynched ? "true" : "false"}
        </Text>
      ))}
    </>
  );
};

export default OfflinePersistComponentExample;
