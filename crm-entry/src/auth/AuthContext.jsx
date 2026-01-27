import { createContext, useContext, useEffect, useState } from "react";
import { jwtDecode } from "jwt-decode";

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [accessToken, setAccessToken] = useState(null);
  const [user, setUser] = useState(null);
  const [permissions, setPermissions] = useState([]);
  const [loading, setLoading] = useState(true);

  /**
   * 🔁 Restore session on app refresh
   */
  useEffect(() => {
    const token = localStorage.getItem("accessToken");
    if (token) {
      setSession(token);
    }
    setLoading(false);
  }, []);

  /**
   * 🔐 Set session after login
   */
  const setSession = (token) => {
    localStorage.setItem("accessToken", token);
    setAccessToken(token);

    const decoded = jwtDecode(token);
    setUser(decoded);

    // ✅ Normalize permissions once
    const perms = Array.isArray(decoded?.perm)
      ? decoded.perm
      : decoded?.perm
      ? [decoded.perm]
      : [];

    setPermissions(perms);
  };

  /**
   * ⏱️ Auto-logout on token expiry
   */
  useEffect(() => {
    if (!accessToken) return;

    const decoded = jwtDecode(accessToken);
    const expiryTime = decoded.exp * 1000;
    const timeout = expiryTime - Date.now();

    if (timeout <= 0) {
      logout();
      return;
    }

    const timer = setTimeout(() => {
      logout();
    }, timeout);

    return () => clearTimeout(timer);
  }, [accessToken]);

  /**
   * 🚪 Logout (state only — routing handled elsewhere)
   */
  const logout = () => {
    localStorage.removeItem("accessToken");
    localStorage.removeItem("refreshToken");

    setAccessToken(null);
    setUser(null);
    setPermissions([]);
  };

  /**
   * ✅ Derived flags
   */
  const isAuthenticated = !!accessToken;

  const isAdmin = permissions.some((p) =>
    [
      "CRM_FULL_ACCESS",
      "USER_CREATE",
      "USER_UPDATE",
      "USER_VIEW",
      "USER_LOCK",
      "USER_UNLOCK",
    ].includes(p)
  );

  return (
    <AuthContext.Provider
      value={{
        accessToken,
        user,
        permissions,
        isAuthenticated,
        isAdmin,
        setSession,
        logout,
        loading,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);
