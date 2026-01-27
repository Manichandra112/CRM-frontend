import { BrowserRouter, Routes, Route } from "react-router-dom";

/* Auth */
import Login from "./pages/auth/Login";
import PostLoginRouter from "./routes/PostLoginRouter";
import ProtectedRoute from "./routes/ProtectedRoute";
import AdminRoute from "./routes/AdminRoute";

/* Admin */
import AdminLayout from "./pages/admin/AdminLayout";
import Users from "./pages/admin/Users";
import Roles from "./pages/admin/Roles";
import Permissions from "./pages/admin/Permissions";
import Logout from "./pages/auth/Logout"
/* Real Social CRM */
import SocialEntry from "./socialCRM/SocialEntry";

const App = () => {
  return (
    <BrowserRouter>
      <Routes>

        {/* AUTH */}
        <Route path="/login" element={<Login />} />
        <Route path="/" element={<PostLoginRouter />} />

        {/* ADMIN */}
        <Route
          path="/admin"
          element={
            <AdminRoute>
              <AdminLayout />
            </AdminRoute>
          }
        >
          <Route index element={<div>Welcome to Admin Panel</div>} />
          <Route path="users" element={<Users />} />
          <Route path="roles" element={<Roles />} />
          <Route path="permissions" element={<Permissions />} />
        </Route>

        {/* REAL SOCIAL CRM (NO GUARDS YET) */}
        <Route path="/social/*" element={<SocialEntry />} />

        {/* FALLBACK */}
        <Route path="/access-denied" element={<div>Access Denied</div>} />
<Route path="/logout" element={<Logout />} />

      </Routes>
    </BrowserRouter>
  );
};

export default App;
