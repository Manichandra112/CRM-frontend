import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { getDomains, createDomain } from "../../api/admin/domains.api";
import api from "../../api/axios";

export default function Domains() {
  const [domains, setDomains] = useState([]);
  const [loading, setLoading] = useState(true);

  const [domainName, setDomainName] = useState("");
  const [domainCode, setDomainCode] = useState("");
  const [creating, setCreating] = useState(false);

  const navigate = useNavigate();

  useEffect(() => {
    loadDomains();
  }, []);

  const loadDomains = async () => {
    try {
      setLoading(true);
      const res = await getDomains();
      setDomains(res.data);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateDomain = async () => {
    if (!domainName.trim() || !domainCode.trim()) {
      alert("Domain name and code are required");
      return;
    }

    setCreating(true);
    try {
      await createDomain({
        domainName: domainName.trim(),
        domainCode: domainCode.trim().toUpperCase(),
      });

      setDomainName("");
      setDomainCode("");
      loadDomains();
    } catch (err) {
      console.error(err);
      alert("Failed to create domain");
    } finally {
      setCreating(false);
    }
  };

 const toggleDomain = async (domain) => {
  const action = domain.active ? "disable" : "enable";
  if (!window.confirm(`Are you sure you want to ${action} this CRM?`)) return;

  await api.put(`/api/admin/domains/${domain.domainId}`, {
    isActive: !domain.active,
  });

  loadDomains();
};


  const openDomain = (domain) => {
    if (!domain.active) return;
    navigate(`/crm/${domain.domainCode.toLowerCase()}`);
  };

  if (loading) {
    return <div className="text-sm text-slate-500">Loading domains…</div>;
  }

  return (
    <div className="space-y-6">
      {/* HEADER */}
      <div>
        <h2 className="text-xl font-semibold text-slate-800">Domains</h2>
        <p className="text-sm text-slate-500">
          Manage business CRMs and control their availability
        </p>
      </div>

      {/* ADD DOMAIN */}
      <div className="bg-white border border-slate-200 rounded-md p-4">
        <h3 className="text-sm font-semibold text-slate-700 mb-3">
          Add New Domain
        </h3>

        <div className="grid grid-cols-1 md:grid-cols-4 gap-3">
          <input
            type="text"
            placeholder="Domain Name (e.g. Finance)"
            value={domainName}
            onChange={(e) => setDomainName(e.target.value)}
            className="border rounded px-3 py-2 text-sm focus:ring-2 focus:ring-blue-500"
          />

          <input
            type="text"
            placeholder="Domain Code (e.g. FINANCE)"
            value={domainCode}
            onChange={(e) => setDomainCode(e.target.value)}
            className="border rounded px-3 py-2 text-sm uppercase focus:ring-2 focus:ring-blue-500"
          />

          <button
            onClick={handleCreateDomain}
            disabled={creating}
            className="bg-blue-600 text-white text-sm rounded px-4 py-2 hover:bg-blue-700 disabled:opacity-50"
          >
            {creating ? "Creating…" : "Add Domain"}
          </button>
        </div>
      </div>

      {/* DOMAINS TABLE */}
      <div className="bg-white border border-slate-200 rounded-md overflow-hidden">
        <table className="w-full text-sm">
          <thead className="bg-slate-50 text-slate-600">
            <tr>
              <th className="px-4 py-3 text-left font-medium">Domain</th>
              <th className="px-4 py-3 text-left font-medium">Status</th>
              <th className="px-4 py-3 text-left font-medium">Created</th>
              <th className="px-4 py-3 text-left font-medium">Updated</th>
              <th className="px-4 py-3 text-right font-medium">Admin Control</th>
            </tr>
          </thead>

          <tbody className="divide-y">
            {domains
              .filter((d) => d.domainCode !== "SYSTEM")
              .map((d) => (
                <tr
                  key={d.domainId}
                  className={`${
                    d.active
                      ? "hover:bg-slate-50 cursor-pointer"
                      : "bg-slate-50 opacity-70"
                  }`}
                  onClick={() => openDomain(d)}
                >
                  {/* DOMAIN */}
                  <td className="px-4 py-3">
                    <div
                      className={`font-medium ${
                        d.active
                          ? "text-blue-600 hover:underline"
                          : "text-slate-500"
                      }`}
                    >
                      {d.domainName}
                    </div>
                    <div className="text-xs text-slate-500">
                      {d.domainCode}
                    </div>
                  </td>

                  {/* STATUS */}
                  <td className="px-4 py-3">
                    <span
                      className={`px-2 py-1 rounded-full text-xs font-semibold ${
                        d.active
                          ? "bg-green-100 text-green-700"
                          : "bg-red-100 text-red-700"
                      }`}
                    >
                      {d.active ? "ACTIVE" : "INACTIVE"}
                    </span>
                  </td>

                  {/* CREATED */}
                  <td className="px-4 py-3 text-slate-600">
                    {new Date(d.createdAt).toLocaleDateString()}
                  </td>

                  {/* UPDATED */}
                  <td className="px-4 py-3 text-slate-600">
                    {d.updatedAt ? (
  new Date(d.updatedAt).toLocaleDateString()
) : (
  <span className="text-xs text-slate-400 italic">
    Never modified
  </span>
)}

                  </td>

                  {/* ADMIN ACTION */}
                  <td
                    className="px-4 py-3 text-right"
                    onClick={(e) => e.stopPropagation()}
                  >
                    <button
                      onClick={() => toggleDomain(d)}
                      className={`text-sm font-medium ${
                        d.active
                          ? "text-red-600 hover:underline"
                          : "text-green-600 hover:underline"
                      }`}
                    >
                      {d.active ? "Disable" : "Enable"}
                    </button>
                  </td>
                </tr>
              ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
