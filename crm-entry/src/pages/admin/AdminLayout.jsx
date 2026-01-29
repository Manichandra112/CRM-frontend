import { Outlet, NavLink } from "react-router-dom";

const linkClass = ({ isActive }) =>
  `block px-3 py-2 rounded text-sm font-medium transition
   ${isActive
     ? "bg-slate-700 text-blue-400"
     : "text-slate-300 hover:bg-slate-700 hover:text-white"}`;

export default function AdminLayout() {
  return (
    <div className="flex min-h-screen bg-slate-100">
      {/* SIDEBAR */}
      <aside className="w-60 bg-slate-800 text-white flex flex-col">
        <div className="px-4 py-4 border-b border-slate-700">
          <h3 className="text-lg font-semibold tracking-wide">
            CRM Admin
          </h3>
        </div>

        <nav className="flex-1 px-3 py-4 space-y-1">
          <NavLink to="/admin/domains" className={linkClass}>
            Domains
          </NavLink>
          <NavLink to="/admin/users" className={linkClass}>
            Users
          </NavLink>
          <NavLink to="/admin/roles" className={linkClass}>
            Roles
          </NavLink>
          <NavLink to="/admin/permissions" className={linkClass}>
            Permissions
          </NavLink>
        </nav>

        <div className="p-4 border-t border-slate-700">
          <button
            onClick={() => (window.location.href = "/logout")}
            className="w-full text-sm bg-red-500 hover:bg-red-600 rounded px-3 py-2"
          >
            Logout
          </button>
        </div>
      </aside>

      {/* MAIN */}
      <main className="flex-1 p-6">
        <Outlet />
      </main>
    </div>
  );
}
