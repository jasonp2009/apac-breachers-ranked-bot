import { GadgetData } from "../../api/generated/abrApiClient";

export const gadgetSortFn = (a: GadgetData, b: GadgetData) => {
  return (b.damage ?? b.healed ?? 0) - (a.damage ?? a.healed ?? 0)
};
