import api from "../axios";

export const getManagersByDomain = async (domainCode) => {
  const res = await api.get("/api/users/managers", {
    params: { domainCode },
  });
  return res.data; // ARRAY
};

export const getMyTeam = async () => {
  const res = await api.get("/api/users/me/team");
  return res.data; // ARRAY
};
