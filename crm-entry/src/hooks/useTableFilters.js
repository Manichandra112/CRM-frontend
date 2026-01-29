import { useMemo } from "react";

/**
 * Generic table filtering hook
 * Domain is inferred from role prefix:
 * HR → HR_*
 * SALES → SALES_*
 * SOCIAL → SOCIAL_*
 *
 * @param {Array} users - raw users list
 * @param {Object} filters - { search, domainCode, roleCode, status }
 */
export default function useTableFilters(users, filters) {
  const { search, domainCode, roleCode, status } = filters;

  const filteredUsers = useMemo(() => {
    return users.filter((u) => {
      /* 🔍 SEARCH */
      const matchesSearch =
        !search ||
        u.username?.toLowerCase().includes(search.toLowerCase()) ||
        u.email?.toLowerCase().includes(search.toLowerCase());

      /* 🏢 DOMAIN (DERIVED FROM ROLE PREFIX) */
      const matchesDomain =
        !domainCode ||
        u.roles?.some((r) =>
          r.startsWith(domainCode + "_")
        );

      /* 👤 ROLE (ROLE CODE MATCH) */
      const matchesRole =
        !roleCode ||
        u.roles?.includes(roleCode);

      /* 🔒 STATUS */
      const matchesStatus =
        !status ||
        u.accountStatus === status;

      return (
        matchesSearch &&
        matchesDomain &&
        matchesRole &&
        matchesStatus
      );
    });
  }, [users, search, domainCode, roleCode, status]);

  return filteredUsers;
}
