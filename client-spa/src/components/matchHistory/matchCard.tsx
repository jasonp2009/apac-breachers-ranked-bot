import {
  Card,
  CardContent,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow
} from "@mui/material";
import { MatchDto, MatchPlayer, MatchPlayerDto } from "../../api/generated/abrApiClient";
import Typography from "@mui/material/Typography";

class MatchPlayerRow {
  homePlayer: MatchPlayerDto | null | undefined;
  awayPlayer: MatchPlayerDto | null | undefined;
}

export function MatchCard(props: {
  sx: any,
  match: MatchDto
}) {
  const { match } = props;
  let playerRows: MatchPlayerRow[] = [];
  for(let i = 0; i < Math.max(match.homePlayers?.length ?? 0, match.awayPlayers?.length ?? 0); i++) {
    const item = new MatchPlayerRow();
    if (match.homePlayers && i < (match.homePlayers?.length ?? 0)) {
      item.homePlayer = match.homePlayers[i];
    }
    if (match.awayPlayers && i < (match.awayPlayers?.length ?? 0)) {
      item.awayPlayer = match.awayPlayers[i];
    }
    playerRows.push(item);
  }
  return (
    <Card sx={props.sx}>
      <CardContent>
        <Typography sx={{ textAlign: 'center' }} variant="h5" component="div">
          Match #{match.matchNumber}
        </Typography>
        <Typography sx={{ mb: 1.5, textAlign: 'center' }} color="text.secondary">
          {match.score?.roundScore?.home} - {match.score?.roundScore?.away}
        </Typography>
        <TableContainer component={Paper}>
          <Table aria-label="simple table">
            <TableHead>
              <TableRow>
                <TableCell>Home</TableCell>
                <TableCell align="right">Away</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {
                playerRows.map((player) => (
                <TableRow
                  sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
                >
                  <TableCell>{player.homePlayer?.name}</TableCell>
                  <TableCell align="right">{player.awayPlayer?.name}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      </CardContent>
    </Card>
  )
}
