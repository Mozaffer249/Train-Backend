import { ReactNode } from 'react';
import { Navigate } from 'react-router-dom';
import { useMe } from '../../contexts/MeContext';
import { Role } from '../../types/infrastructure';

// Routes wrapped in <RequireRole roles={['Admin','SuperAdmin']}> only render
// when the current user's role set intersects with `roles`. Otherwise we
// redirect — the backend would 401/403 the API call anyway; this just keeps
// the UX clean.

interface Props {
  roles: Role[];
  redirectTo?: string;
  children: ReactNode;
}

const RequireRole = ({ roles, redirectTo = '/dashboard', children }: Props) => {
  const { me, isLoading, hasRole } = useMe();

  if (isLoading) return null;
  if (!me) return <Navigate to="/login" replace />;
  if (!hasRole(...roles)) return <Navigate to={redirectTo} replace />;

  return <>{children}</>;
};

export default RequireRole;
