import { WeaponData } from "../../api/generated/abrApiClient";

export const weaponSortFn = (a: WeaponData, b: WeaponData) => {
  return (b.damage ?? 0) - (a.damage ?? 0)
};
