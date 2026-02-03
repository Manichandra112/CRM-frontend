import api from "../axios";

export const getPermissions = async () => {
  const res = await api.get("/api/admin/permissions");
  return res.data; // ARRAY
};

export const createPermission = async (payload) => {
  const res = await api.post("/api/admin/permissions", payload);
  return res.data;
};

export const updatePermission = async (id, payload) => {
  const res = await api.put(`/api/admin/permissions/${id}`, payload);
  return res.data;
};
