// import api from "../axios";


// export const getAllPermissions = () => {
//   return api.get("/api/admin/permissions");
// };



// export const getRolePermissionsByRoleCode = (roleCode) => {
//   return api.get(
//     `/api/admin/role-permissions/by-role/code/${roleCode}`
//   );
// };

// export const assignPermissionToRole = ({
//   roleCode,
//   permissionCode,
// }) => {
//   return api.post("/api/admin/role-permissions", {
//     roleCode,
//     permissionCode,
//   });
// };

// export const removePermissionFromRole = ({
//   roleCode,
//   permissionCode,
// }) => {
//   return api.delete("/api/admin/role-permissions", {
//     data: {
//       roleCode,
//       permissionCode,
//     },
//   });
// };


import api from "../axios";

export const getAllPermissions = async () => {
  const res = await api.get("/api/admin/permissions");
  return res.data; // ARRAY
};

export const getRolePermissionsByRoleCode = async (roleCode) => {
  const res = await api.get(
    `/api/admin/role-permissions/by-role/code/${roleCode}`
  );
  return res.data; // ARRAY
};

export const assignPermissionToRole = async ({
  roleCode,
  permissionCode,
}) => {
  const res = await api.post("/api/admin/role-permissions", {
    roleCode,
    permissionCode,
  });
  return res.data;
};

export const removePermissionFromRole = async ({
  roleCode,
  permissionCode,
}) => {
  const res = await api.delete("/api/admin/role-permissions", {
    data: { roleCode, permissionCode },
  });
  return res.data;
};
