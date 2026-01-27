import { useNavigate } from "react-router-dom";
import { useAuth } from "@/auth/AuthContext";

export default function CrmSwitcher() {
  const navigate = useNavigate();
  const auth = useAuth();

  const domains = auth.role === "ADMIN"
    ? ["social", "sales", "hr"]
    : auth.domains; // later from backend

  return (
    <div>
      {domains.map(d => (
        <button key={d} onClick={() => navigate(`/crm/${d}`)}>
          {d.toUpperCase()}
        </button>
      ))}
    </div>
  );
}
