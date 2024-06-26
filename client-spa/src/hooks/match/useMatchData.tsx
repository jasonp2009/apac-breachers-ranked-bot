import { useQuery } from "react-query";
import { useAbrApiClient } from "../../api/useAbrApiClient";

export function useMatchData(matchId: string) {
  const abrApi = useAbrApiClient();
  return useQuery({
    queryKey: [`matchData:${matchId}`],
    queryFn: () =>
      abrApi.matchData(matchId)
  })
}
