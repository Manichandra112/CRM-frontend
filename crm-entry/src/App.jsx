// import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";

// /* Auth */
// import Login from "./pages/auth/Login";
// import PostLoginRouter from "./routes/PostLoginRouter";
// import ProtectedRoute from "./routes/ProtectedRoute";
// import AdminRoute from "./routes/AdminRoute";
// import Logout from "./pages/auth/Logout";

// /* Admin */
// import AdminLayout from "./pages/admin/AdminLayout";
// import Domains from "./pages/admin/Domains";
// import Users from "./pages/admin/Users";
// import Roles from "./pages/admin/Roles";
// import Permissions from "./pages/admin/Permissions";

// /* CRM */
// import CrmShell from "./pages/crm/CrmShell";

// const App = () => {
//   return (
//     <BrowserRouter>
//       <Routes>

//         {/* AUTH */}
//         <Route path="/login" element={<Login />} />
//         <Route path="/" element={<PostLoginRouter />} />

//         {/* ADMIN */}
//         <Route
//           path="/admin"
//           element={
//             <AdminRoute>
//               <AdminLayout />
//             </AdminRoute>
//           }
//         >
//           {/* Admin always lands on Domains */}
          

//           <Route path="domains" element={<Domains />} />
//           <Route path="users" element={<Users />} />
//           <Route path="roles" element={<Roles />} />
//           <Route path="permissions" element={<Permissions />} />
//         </Route>

//         {/* CRM SHELL (RULE-BASED) */}
//         <Route
//           path="/crm/:domainCode/*"
//           element={
//             <ProtectedRoute>
//               <CrmShell />
//             </ProtectedRoute>
//           }
//         />

//         {/* LOGOUT */}
//         <Route path="/logout" element={<Logout />} />

//         {/* FALLBACK */}
//         <Route path="/access-denied" element={<div>Access Denied</div>} />

//       </Routes>
//     </BrowserRouter>
//   );
// };

// export default App;

import { Routes, Route } from "react-router-dom";

/* Auth */
import Login from "./pages/auth/Login";
import PostLoginRouter from "./routes/PostLoginRouter";
import ProtectedRoute from "./routes/ProtectedRoute";
import AdminRoute from "./routes/AdminRoute";
import Logout from "./pages/auth/Logout";

/* Admin */
import AdminLayout from "./pages/admin/AdminLayout";
import Domains from "./pages/admin/Domains";
import Users from "./pages/admin/Users";
import Roles from "./pages/admin/Roles";
import Permissions from "./pages/admin/Permissions";

/* CRM */
import CrmShell from "./pages/crm/CrmShell";

const App = () => {
  return (
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
        {/* <Route index element={<Domains />} /> */}
        <Route path="domains" element={<Domains />} />
        <Route path="users" element={<Users />} />
        <Route path="roles" element={<Roles />} />
        <Route path="permissions" element={<Permissions />} />
      </Route>

      {/* CRM */}
      <Route
        path="/crm/:domainCode/*"
        element={
          <ProtectedRoute>
            <CrmShell />
          </ProtectedRoute>
        }
      />

      {/* LOGOUT */}
      <Route path="/logout" element={<Logout />} />
      <Route path="/access-denied" element={<div>Access Denied</div>} />
    </Routes>
  );
};

export default App;








