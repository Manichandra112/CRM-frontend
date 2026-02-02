// import { createContext, useContext, useEffect, useState } from "react";
// import { jwtDecode } from "jwt-decode";

// const AuthContext = createContext(null);

// export const AuthProvider = ({ children }) => {
//   const [accessToken, setAccessToken] = useState(null);
//   const [user, setUser] = useState(null);
//   const [permissions, setPermissions] = useState([]);
//   const [loading, setLoading] = useState(true);

//   // 🔐 NEW: password reset flags
//   const [pwdResetRequired, setPwdResetRequired] = useState(false);
//   const [pwdResetCompleted, setPwdResetCompleted] = useState(false);

//   /**
//    * 🔁 Restore session on app refresh
//    */
// useEffect(() => {
//   const token = localStorage.getItem("accessToken");

//   if (token && token !== "undefined" && token !== "null") {
//     setSession(token);
//   } else {
//     localStorage.removeItem("accessToken");
//   }

//   setLoading(false);
// }, []);
//   /**
//    * 🔐 Set session after login / refresh
//    */
// const setSession = (token) => {
//   try {
//     if (!token || typeof token !== "string" || token.split(".").length !== 3) {
//       throw new Error("Invalid JWT format");
//     }

//     localStorage.setItem("accessToken", token);
//     setAccessToken(token);

//     const decoded = jwtDecode(token);
//     setUser(decoded);

//     // 🔐 Password reset flags
//     setPwdResetRequired(decoded.pwd_reset_required === "true");
//     setPwdResetCompleted(decoded.pwd_reset_completed === "true");

//     // ✅ Normalize permissions
//     const perms = Array.isArray(decoded?.perm)
//       ? decoded.perm
//       : decoded?.perm
//       ? [decoded.perm]
//       : [];

//     setPermissions(perms);
//   } catch (err) {
//     console.error("JWT decode failed, clearing session", err);
//     logout();
//   }
// };


//   /**
//    * ⏱️ Auto-logout on token expiry
//    */
//   useEffect(() => {
//     if (!accessToken) return;

//     const decoded = jwtDecode(accessToken);
//     const expiryTime = decoded.exp * 1000;
//     const timeout = expiryTime - Date.now();

//     if (timeout <= 0) {
//       logout();
//       return;
//     }

//     const timer = setTimeout(() => {
//       logout();
//     }, timeout);

//     return () => clearTimeout(timer);
//   }, [accessToken]);

//   /**
//    * 🚪 Logout (state only — routing handled elsewhere)
//    */
//   const logout = () => {
//     localStorage.removeItem("accessToken");
//     localStorage.removeItem("refreshToken");

//     setAccessToken(null);
//     setUser(null);
//     setPermissions([]);
//     setPwdResetRequired(false);
//     setPwdResetCompleted(false);
//   };

//   /**
//    * ✅ Derived flags
//    */
//   const isAuthenticated = !!accessToken;

//   const isAdmin = permissions.some((p) =>
//     [
//       "CRM_FULL_ACCESS",
//       "USER_CREATE",
//       "USER_UPDATE",
//       "USER_VIEW",
//       "USER_LOCK",
//       "USER_UNLOCK",
//     ].includes(p)
//   );

//   return (
//     <AuthContext.Provider
//       value={{
//         accessToken,
//         user,
//         permissions,

//         // 🔐 password reset flags
//         pwdResetRequired,
//         pwdResetCompleted,

//         isAuthenticated,
//         isAdmin,

//         setSession,
//         logout,
//         loading,
//       }}
//     >
//       {children}
//     </AuthContext.Provider>
//   );
// };

// export const useAuth = () => useContext(AuthContext);

import { createContext, useContext, useEffect, useState } from "react";
import { jwtDecode } from "jwt-decode";

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [accessToken, setAccessToken] = useState(null);
  const [user, setUser] = useState(null);
  const [permissions, setPermissions] = useState([]);
  const [loading, setLoading] = useState(true);

  const [pwdResetRequired, setPwdResetRequired] = useState(false);
  const [pwdResetCompleted, setPwdResetCompleted] = useState(false);

  const [authChecking, setAuthChecking] = useState(false);

  // 🔁 Restore session on refresh
  useEffect(() => {
    const token = localStorage.getItem("accessToken");

    if (token && token !== "null" && token !== "undefined") {
      try {
        setSession(token);
      } catch {
        logout();
      }
    }

    setLoading(false);
  }, []);

  const setSession = (token) => {
    if (!token || token.split(".").length !== 3) {
      throw new Error("Invalid JWT");
    }

    localStorage.setItem("accessToken", token);
    setAccessToken(token);

    const decoded = jwtDecode(token);
    setUser(decoded);

    setPwdResetRequired(decoded.pwd_reset_required === "true");
    setPwdResetCompleted(decoded.pwd_reset_completed === "true");

    const perms = Array.isArray(decoded?.perm)
      ? decoded.perm
      : decoded?.perm
      ? [decoded.perm]
      : [];

    setPermissions(perms);
  };

  // ⏱️ Auto logout on expiry
  useEffect(() => {
    if (!accessToken) return;

    const { exp } = jwtDecode(accessToken);
    const timeout = exp * 1000 - Date.now();

    if (timeout <= 0) {
      logout();
      return;
    }

    const timer = setTimeout(logout, timeout);
    return () => clearTimeout(timer);
  }, [accessToken]);

  const logout = () => {
    localStorage.clear();
    setAccessToken(null);
    setUser(null);
    setPermissions([]);
    setPwdResetRequired(false);
    setPwdResetCompleted(false);
    setAuthChecking(false);
  };

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
        pwdResetRequired,
        pwdResetCompleted,
        authChecking,
        setAuthChecking,
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
