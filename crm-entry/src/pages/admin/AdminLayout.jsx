import { Outlet, NavLink } from "react-router-dom";
import { useAuth } from "../../auth/AuthContext";

const AdminLayout = () => {
  const { logout } = useAuth();

  return (
    <div style={{ display: "flex", minHeight: "100vh" }}>
      {/* Sidebar */}
      <aside
        style={{
          width: 240,
          background: "#1f2937",
          color: "#fff",
          padding: 16,
        }}
      >
        <h3>CRM Admin</h3>

        <nav style={{ marginTop: 24 }}>
          <NavLink to="/admin/users" style={navStyle}>
            Users
          </NavLink>
          <NavLink to="/admin/roles" style={navStyle}>
            Roles
          </NavLink>
          <NavLink to="/admin/permissions" style={navStyle}>
            Permissions
          </NavLink>
        </nav>

        <button
          onClick={logout}
          style={{
            marginTop: 32,
            width: "100%",
            padding: 8,
            background: "#ef4444",
            color: "#fff",
            border: "none",
          }}
        >
          Logout
        </button>
      </aside>

      {/* Content */}
      <main style={{ flex: 1, padding: 24 }}>
        <Outlet />
      </main>
    </div>
  );
};

const navStyle = ({ isActive }) => ({
  display: "block",
  padding: "8px 0",
  color: isActive ? "#60a5fa" : "#fff",
  textDecoration: "none",
});

export default AdminLayout;
