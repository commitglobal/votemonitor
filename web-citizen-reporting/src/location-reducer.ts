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
      switch (action.level) {
        case "level1Filter":
          return {
            ...state,
            level1Filter: action.value,
            level2Filter: undefined,
            level3Filter: undefined,
            level4Filter: undefined,
            level5Filter: undefined,
          };
        case "level2Filter":
          return {
            ...state,
            level2Filter: action.value,
            level3Filter: undefined,
            level4Filter: undefined,
            level5Filter: undefined,
          };
        case "level3Filter":
          return {
            ...state,
            level3Filter: action.value,
            level4Filter: undefined,
            level5Filter: undefined,
          };
        case "level4Filter":
          return {
            ...state,
            level4Filter: action.value,
            level5Filter: undefined,
          };
        case "level5Filter":
          return {
            ...state,
            level5Filter: action.value,
          };
        default:
          return state;
      }
    case "CLEAR_FILTER":
      return { ...state, [action.level]: undefined };
    case "RESET_FILTERS":
      return {};
    default:
      return state;
  }
};
