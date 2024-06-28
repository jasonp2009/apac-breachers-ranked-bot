import { useParams } from "react-router-dom";
import { MatchDetails } from "../../components";
import Box from "@mui/material/Box";

export function MatchDetailsPage() {
  const params = useParams();
  return (
    <Box maxWidth={600}>
      <MatchDetails
        matchId={params.id} />
    </Box>
  )
}
