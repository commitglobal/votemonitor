export type LocationState = {
  level1Filter?: number;
  level2Filter?: number;
  level3Filter?: number;
  level4Filter?: number;
  level5Filter?: number;
};

export type LocationAction =
  | { type: "SET_FILTER"; level: keyof LocationState; value: number }
  | { type: "CLEAR_FILTER"; level: keyof LocationState }
  | { type: "RESET_FILTERS" };

export const locationReducer = (
  state: LocationState,
  action: LocationAction
): LocationState => {
  switch (action.type) {
    case "SET_FILTER":
      return { ...state, [action.level]: action.value };
    case "CLEAR_FILTER":
      return { ...state, [action.level]: undefined };
    case "RESET_FILTERS":
      return {};
    default:
      return state;
  }
};
