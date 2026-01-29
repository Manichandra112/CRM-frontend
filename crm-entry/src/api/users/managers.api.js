import api from "../axios";

export const getManagersByDomain = (domainCode) =>
  api.get("/api/users/managers", {
    params: { domainCode },
  });

export const getMyTeam = () =>
  api.get("/api/users/me/team");
