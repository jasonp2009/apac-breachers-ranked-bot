import { useParams } from "react-router-dom";
import { MatchDetails } from "../../components";

export function MatchDetailsPage() {
  const params = useParams();
  return (
    <MatchDetails matchId={params.id} />
  )
}
