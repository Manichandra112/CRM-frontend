import api from "../axios";


export const getAllPermissions = () => {
  return api.get("/api/admin/permissions");
};



export const getRolePermissionsByRoleCode = (roleCode) => {
  return api.get(
    `/api/admin/role-permissions/by-role/code/${roleCode}`
  );
};

export const assignPermissionToRole = ({
  roleCode,
  permissionCode,
}) => {
  return api.post("/api/admin/role-permissions", {
    roleCode,
    permissionCode,
  });
};

export const removePermissionFromRole = ({
  roleCode,
  permissionCode,
}) => {
  return api.delete("/api/admin/role-permissions", {
    data: {
      roleCode,
      permissionCode,
    },
  });
};
