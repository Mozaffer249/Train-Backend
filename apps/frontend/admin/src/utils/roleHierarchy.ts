import { ROLES, Role } from '../types/infrastructure';

const PRIVILEGED: Role[] = [ROLES.SuperAdmin, ROLES.Admin];

const ADMIN_ASSIGNABLE: Role[] = [
  ROLES.Staff,
  ROLES.StaffCounter,
  ROLES.StaffBoarding,
  ROLES.Customer,
  ROLES.User,
];

export const isSuperAdmin = (callerRoles: string[]) =>
  callerRoles.includes(ROLES.SuperAdmin);

export const targetHasPrivilegedRole = (targetRoles: string[]) =>
  targetRoles.some((r) => PRIVILEGED.includes(r as Role));

export const canManageUser = (callerRoles: string[], targetRoles: string[]) =>
  isSuperAdmin(callerRoles) || !targetHasPrivilegedRole(targetRoles);

export const getAssignableRoles = (callerRoles: string[]): Role[] =>
  isSuperAdmin(callerRoles) ? Object.values(ROLES) : ADMIN_ASSIGNABLE;
