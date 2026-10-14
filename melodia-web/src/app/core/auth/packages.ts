import { PackageName } from './auth.models';

export const PACKAGE_LEVEL: Record<PackageName, number> = {
  Basic: 1,
  Gold: 2,
  Premium: 3,
  Elit: 4,
};

export function canAccess(userPackage: PackageName, required: PackageName): boolean {
  return PACKAGE_LEVEL[userPackage] >= PACKAGE_LEVEL[required];
}
