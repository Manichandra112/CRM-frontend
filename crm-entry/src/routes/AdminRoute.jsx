// import { Navigate } from "react-router-dom";
// import { useAuth } from "../auth/AuthContext";

// const AdminRoute = ({ children }) => {
//   const {
//     isAuthenticated,
//     isAdmin,
//     pwdResetRequired,
//     loading,
//     authChecking,
//   } = useAuth();

//   // 🚦 FREEZE routing during auth
//   if (loading || authChecking) return null;

//   if (!isAuthenticated) {
//     return <Navigate to="/login" replace />;
//   }

//   if (pwdResetRequired) {
//     return <Navigate to="/reset-password" replace />;
//   }

//   if (!isAdmin) {
//     return <Navigate to="/access-denied" replace />;
//   }

//   return children;
// };

// export default AdminRoute;

import { Navigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

const AdminRoute = ({ children }) => {
  const { isAuthenticated, isAdmin, loading } = useAuth();

  if (loading) return null;

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (!isAdmin) {
    return <Navigate to="/access-denied" replace />;
  }

  return children;
};

export default AdminRoute;
