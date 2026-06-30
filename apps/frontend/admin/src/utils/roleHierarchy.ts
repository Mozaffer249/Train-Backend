import { ROLES, Role } from '../types/infrastructure';

const PRIVILEGED: Role[] = [ROLES.SuperAdmin, ROLES.Admin];

const ADMIN_ASSIGNABLE: Role[] = [
  ROLES.Staff,
  ROLES.StaffCounter,
  ROLES.StaffBoarding,
  ROLES.Customer,
  ROLES.User,
];

export const OPERATIONAL_ROLES: Role[] = [...ADMIN_ASSIGNABLE];

export const PRIVILEGED_ROLES: Role[] = [...PRIVILEGED];

export const isSuperAdmin = (callerRoles: string[]) =>
  callerRoles.includes(ROLES.SuperAdmin);

export const targetHasPrivilegedRole = (targetRoles: string[]) =>
  targetRoles.some((r) => PRIVILEGED.includes(r as Role));

export const canManageUser = (callerRoles: string[], targetRoles: string[]) =>
  isSuperAdmin(callerRoles) || !targetHasPrivilegedRole(targetRoles);

/** Roles assignable on the regular Users page (staff/customers only). */
export const getOperationalAssignableRoles = (): Role[] => OPERATIONAL_ROLES;

/** Roles assignable when managing admins (Admin/SuperAdmin). */
export const getAdminAssignableRoles = (): Role[] => PRIVILEGED_ROLES;

/** Roles for demoting an admin to operational staff/customer. */
export const getDemotionRoles = (): Role[] => OPERATIONAL_ROLES;

export const getAssignableRoles = (_callerRoles: string[]): Role[] =>
  OPERATIONAL_ROLES;
