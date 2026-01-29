// import { useEffect, useState } from "react";
// import { getAdminRoles } from "../../api/admin/roles.api";
// import RolePermissionsEditor from "../../components/admin/roles/RolePermissionsEditor";
// import CreateRoleModal from "../../components/admin/roles/CreateRoleModal";


// export default function RoleManagement() {
//   const [roles, setRoles] = useState([]);
//   const [selectedRole, setSelectedRole] = useState(null);
//   const [loading, setLoading] = useState(true);

//   useEffect(() => {
//     const loadRoles = async () => {
//       try {
//         const res = await getAdminRoles();
//         setRoles(res.data || []);
//         setSelectedRole(res.data?.[0] || null);
//       } catch (e) {
//         console.error("Failed to load roles", e);
//       } finally {
//         setLoading(false);
//       }
//     };

//     loadRoles();
//   }, []);

//   if (loading) {
//     return (
//       <div className="p-6 text-sm text-slate-500">
//         Loading roles…
//       </div>
//     );
//   }

//   return (
//     <div className="p-6">
//       <div className="mb-4">
//         <h2 className="text-xl font-semibold text-slate-800">
//           Role Management
//         </h2>
//         <p className="text-sm text-slate-500">
//           Define roles and assign permissions
//         </p>
//       </div>

//       <div className="grid grid-cols-12 gap-6">
//         {/* LEFT */}
//         <div className="col-span-4 bg-white border rounded-md">
//           <div className="px-4 py-3 border-b font-medium text-sm">
//             Roles
//           </div>

//           <div className="divide-y">
//             {roles.map((role) => {
//               const active =
//                 selectedRole?.roleCode === role.roleCode;

//               return (
//                 <div
//                   key={role.roleCode}
//                   onClick={() => setSelectedRole(role)}
//                   className={`px-4 py-3 cursor-pointer ${
//                     active
//                       ? "bg-blue-50 text-blue-700 font-medium"
//                       : "hover:bg-slate-50"
//                   }`}
//                 >
//                   <div>{role.roleName}</div>
//                   <div className="text-xs text-slate-400">
//                     {role.roleCode}
//                   </div>
//                 </div>
//               );
//             })}
//           </div>
//         </div>

//         {/* RIGHT */}
//         <div className="col-span-8 bg-white border rounded-md p-4">
//           {selectedRole ? (
//             <RolePermissionsEditor role={selectedRole} />
//           ) : (
//             <div className="text-sm text-slate-500">
//               Select a role to manage permissions
//             </div>
//           )}
//         </div>
//       </div>

//       <div className="flex items-center justify-between px-4 py-3 border-b">
//   <span className="text-sm font-medium text-slate-700">
//     Roles
//   </span>
//   <button
//     onClick={() => setShowCreate(true)}
//     className="text-xs text-blue-600 hover:underline"
//   >
//     + Add Role
//   </button>
// </div>

//     </div>
//   );
// }


import { useEffect, useState } from "react";
import { getAdminRoles } from "../../api/admin/roles.api";

import RolePermissionsEditor from "../../components/admin/roles/RolePermissionsEditor";
import CreateRoleModal from "../../components/admin/roles/CreateRoleModal";

export default function RoleManagement() {
  const [roles, setRoles] = useState([]);
  const [selectedRole, setSelectedRole] = useState(null);
  const [loading, setLoading] = useState(true);
  const [showCreate, setShowCreate] = useState(false);

  /* =======================
     LOAD ROLES
     ======================= */
  const loadRoles = async () => {
    try {
      setLoading(true);
      const res = await getAdminRoles();
      const list = res.data || [];
      setRoles(list);
      setSelectedRole(list[0] || null);
    } catch (e) {
      console.error("Failed to load roles", e);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadRoles();
  }, []);

  if (loading) {
    return (
      <div className="p-6 text-sm text-slate-500">
        Loading roles…
      </div>
    );
  }

  return (
    <div className="p-6 space-y-4">
      {/* PAGE HEADER */}
      <div>
        <h2 className="text-xl font-semibold text-slate-800">
          Role Management
        </h2>
        <p className="text-sm text-slate-500">
          Define roles and assign permissions
        </p>
      </div>

      {/* MAIN LAYOUT */}
      <div className="grid grid-cols-12 gap-6">
        {/* LEFT PANEL — ROLES LIST */}
        <div className="col-span-4 bg-white border rounded-md overflow-hidden">
          {/* HEADER */}
          <div className="flex items-center justify-between px-4 py-3 border-b">
            <span className="text-sm font-medium text-slate-700">
              Roles
            </span>
            <button
              onClick={() => setShowCreate(true)}
              className="text-xs text-blue-600 hover:underline"
            >
              + Add Role
            </button>
          </div>

          {/* LIST */}
          <div className="divide-y">
            {roles.map((role) => {
              const active =
                selectedRole?.roleCode === role.roleCode;

              return (
                <div
                  key={role.roleCode}
                  onClick={() => setSelectedRole(role)}
                  className={`px-4 py-3 cursor-pointer ${
                    active
                      ? "bg-blue-50 text-blue-700 font-medium"
                      : "hover:bg-slate-50"
                  }`}
                >
                  <div>{role.roleName}</div>
                  <div className="text-xs text-slate-400">
                    {role.roleCode}
                  </div>
                </div>
              );
            })}

            {roles.length === 0 && (
              <div className="px-4 py-6 text-sm text-slate-500 text-center">
                No roles found
              </div>
            )}
          </div>
        </div>

        {/* RIGHT PANEL — ROLE DETAILS */}
        <div className="col-span-8 bg-white border rounded-md p-4">
          {selectedRole ? (
            <RolePermissionsEditor role={selectedRole} />
          ) : (
            <div className="text-sm text-slate-500">
              Select a role to manage permissions
            </div>
          )}
        </div>
      </div>

      {/* CREATE ROLE MODAL */}
      {showCreate && (
        <CreateRoleModal
          onClose={() => setShowCreate(false)}
          onCreated={() => {
            setShowCreate(false);
            loadRoles(); // refresh roles list
          }}
        />
      )}
    </div>
  );
}
