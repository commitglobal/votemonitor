import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import Cookies from "js-cookie";

interface UserState {
  token?: string;
}

const userToken = Cookies.get("jwt") || undefined;

const initialState: UserState = {
  token: userToken,
};

export const userSlice = createSlice({
  initialState,
  name: "userSlice",
  reducers: {
    logout: () => ({
      ...initialState,
      token: undefined,
    }),
    setToken: (state, action: PayloadAction<string>) => {
      state.token = action.payload;
    },
  }
});
export default userSlice.reducer;

export const { logout, setToken } = userSlice.actions;
