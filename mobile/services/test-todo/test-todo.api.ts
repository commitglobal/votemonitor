import API from "../api";

export type TodoAPI = {
  _id: string;
  title: string;
  priority: "Low" | "Medium" | "High" | "Very High";
  tags: string[];
  status: "New" | "In progress" | "Done";
  createdAt: string;
  updatedAt: string;
};

export type TodoVM = TodoAPI & {
  isNotSynched: boolean;
};

export type UpdateTodoAPI = {
  id: string;
  status: "New" | "In progress" | "Done";
};

export const getTodos = (): Promise<TodoVM[]> => {
  return API.get(`todos`).then((res) => res.data);
};

export const updateStatus = (payload: UpdateTodoAPI): Promise<TodoAPI> => {
  return API.patch(`todos/${payload.id}`, {
    status: payload.status,
  }).then((res) => res.data);
};
