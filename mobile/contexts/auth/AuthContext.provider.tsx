import { useState } from "react";
import { AuthContext } from "./auth-context";

const AuthContextProvider = ({ children }: React.PropsWithChildren) => {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);

  const signIn = () => {
    setIsAuthenticated(true);
  };

  const signOut = () => {
    setIsAuthenticated(false);
  };

  return (
    <AuthContext.Provider
      value={{
        signIn,
        signOut,
        isAuthenticated,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export default AuthContextProvider;
