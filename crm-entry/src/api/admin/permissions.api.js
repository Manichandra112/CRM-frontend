import api from "../axios";

export const getPermissions = () =>
  api.get("/api/admin/permissions");

export const createPermission = (payload) =>
  api.post("/api/admin/permissions", payload);

export const updatePermission = (id, payload) =>
  api.put(`/api/admin/permissions/${id}`, payload);