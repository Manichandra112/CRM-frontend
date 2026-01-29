import api from "../axios";


export const getDomains = () =>
  api.get("/api/admin/domains");

export const createDomain = (data) =>
  api.post("/api/admin/domains", data);

export const updateDomain = (domainId, payload) =>
  api.put(`/api/admin/domains/${domainId}`, payload);



