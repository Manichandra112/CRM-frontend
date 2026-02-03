import api from "../axios";

export const getDomains = async () => {
  const res = await api.get("/api/admin/domains");
  return res.data; // ARRAY
};

export const createDomain = async (payload) => {
  const res = await api.post("/api/admin/domains", payload);
  return res.data;
};

export const updateDomain = async (domainId, payload) => {
  const res = await api.put(`/api/admin/domains/${domainId}`, payload);
  return res.data;
};

export const toggleDomainStatus = async (domainId, isActive) => {
  const res = await api.put(`/api/admin/domains/${domainId}`, {
    isActive,
  });
  return res.data;
};
