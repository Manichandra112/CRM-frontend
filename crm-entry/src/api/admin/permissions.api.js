import api from "../axios";

export const getPermissions = () =>
  api.get("/api/admin/permissions");
