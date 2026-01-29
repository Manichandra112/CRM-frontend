import { useEffect, useState } from "react";
import { getUserAuditLogs } from "../../../../api/admin/users.api";

export default function UserAuditLogsTab({ userId }) {
  const [logs, setLogs] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [page, setPage] = useState(1);
  const pageSize = 10;

  /* =======================
     LOAD AUDIT LOGS
     ======================= */
  useEffect(() => {
    if (!userId) return;

    const loadLogs = async () => {
      try {
        setLoading(true);
        setError(null);

        const res = await getUserAuditLogs(userId, {
          page,
          pageSize,
        });

        // Support both paged and flat responses
        const data = res.data?.logs || res.data || [];
        setLogs(data);
      } catch (err) {
        console.error("Audit logs fetch error:", err);
        setError("Failed to load audit logs");
      } finally {
        setLoading(false);
      }
    };

    loadLogs();
  }, [userId, page]);

  /* =======================
     UI
     ======================= */
  if (loading) {
    return (
      <div className="text-sm text-slate-500">
        Loading audit logs…
      </div>
    );
  }

  if (error) {
    return (
      <div className="text-sm text-red-600 bg-red-50 border border-red-200 rounded px-3 py-2">
        {error}
      </div>
    );
  }

  if (!logs.length) {
    return (
      <div className="text-sm text-slate-500">
        No audit activity found
      </div>
    );
  }

  return (
    <div className="space-y-4">
      <table className="w-full text-sm border border-slate-200 rounded">
        <thead className="bg-slate-50">
          <tr>
            <th className="text-left px-3 py-2 border-b">Action</th>
            <th className="text-left px-3 py-2 border-b">Performed By</th>
            <th className="text-left px-3 py-2 border-b">Date</th>
          </tr>
        </thead>
        <tbody>
          {logs.map((log, idx) => (
            <tr key={idx} className="border-b last:border-b-0">
              <td className="px-3 py-2 text-slate-700">
                {log.action}
              </td>
              <td className="px-3 py-2 text-slate-600">
                {log.performedBy || "System"}
              </td>
              <td className="px-3 py-2 text-slate-600">
                {log.createdAt
                  ? new Date(log.createdAt).toLocaleString()
                  : "-"}
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {/* PAGINATION */}
      <div className="flex justify-between items-center text-sm">
        <button
          disabled={page === 1}
          onClick={() => setPage((p) => p - 1)}
          className="px-3 py-1 border rounded disabled:opacity-50"
        >
          Previous
        </button>

        <span className="text-slate-600">Page {page}</span>

        <button
          disabled={logs.length < pageSize}
          onClick={() => setPage((p) => p + 1)}
          className="px-3 py-1 border rounded disabled:opacity-50"
        >
          Next
        </button>
      </div>
    </div>
  );
}
