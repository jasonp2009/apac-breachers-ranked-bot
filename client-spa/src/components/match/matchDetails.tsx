import {
  Box, Button, Card, CardActions,
  CardContent,
  CircularProgress,
  Paper,
  Table, TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow
} from "@mui/material";
import { useMatchData } from "../../hooks";
import React, {useEffect, useState} from "react";
import Typography from "@mui/material/Typography";
import {GameDataEntity, GamePlayerData, MatchDataDto, MatchDto, MatchPlayerDto} from "../../api/generated/abrApiClient";
import {MatchDetailsModal} from "./matchDetailsModal";


class GamePlayerRow {
  homePlayer: GamePlayerData | null | undefined;
  awayPlayer: GamePlayerData | null | undefined;
}

export function MatchDetails(props: {
  sx?: any,
  matchId?: string
}) {
  if (props.matchId == null) {
    throw new Error("MatchId must be provided");
  }
  const { isLoading, error, data } = useMatchData(props.matchId);
  const [ match, setMatch ] = useState<MatchDto>();
  const [ playerRows, setPlayerRows ] = useState<GamePlayerRow[]>()

  const processMatch = (data?: MatchDataDto) => {
    if (data?.match) {
      setMatch(data.match);
    } else {
      setMatch(undefined);
    }
  }
  const processMatchPlayerRow = (data?: MatchDataDto) => {
    let playerRows: GamePlayerRow[] = [];
    if (!data?.gameData || data?.gameData.length == 0) {
      setPlayerRows(undefined);
      return;
    }
    const game = data.gameData[0];
    
    for(let i = 0; i < Math.max(game.homePlayers?.length ?? 0, game.awayPlayers?.length ?? 0); i++) {
      const item = new GamePlayerRow();
      if (game.homePlayers && i < (game.homePlayers?.length ?? 0)) {
        item.homePlayer = game.homePlayers[i];
      }
      if (game.awayPlayers && i < (game.awayPlayers?.length ?? 0)) {
        item.awayPlayer = game.awayPlayers[i];
      }
      playerRows.push(item);
    }
    setPlayerRows(playerRows);
  }
  
  useEffect(() => {
    processMatch(data);
    processMatchPlayerRow(data);
  }, [data]);
  
  
  return (
    <Box>
      { isLoading && <CircularProgress />}
      { !isLoading &&
          <Card sx={props.sx}>
              <CardContent>
                  <Typography sx={{ textAlign: 'center' }} variant="h5" component="div">
                      Match #{match?.matchNumber}
                  </Typography>
                  <Typography sx={{ mb: 1.5, textAlign: 'center' }} color="text.secondary">
                    {match?.score?.roundScore?.home} - {match?.score?.roundScore?.away}
                  </Typography>
                  <TableContainer component={Paper}>
                      <Table aria-label="simple table">
                          <TableHead>
                              <TableRow>
                                  <TableCell>Home</TableCell>
                                  <TableCell align="right">KDA</TableCell>
                                  <TableCell align="right">Damage</TableCell>
                                  <TableCell align="right">Damage</TableCell>
                                  <TableCell align="right">KDA</TableCell>
                                  <TableCell align="right">Away</TableCell>
                              </TableRow>
                          </TableHead>
                          <TableBody>
                            { playerRows &&
                              playerRows.map((player) => (
                                <TableRow
                                  sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
                                >
                                  <TableCell>
                                    {player.homePlayer?.name}
                                    {player.homePlayer &&
                                        <MatchDetailsModal
                                            buttonText="More"
                                            data={player.homePlayer}
                                        />
                                    }
                                  </TableCell>
                                  <TableCell align="right">{player.homePlayer?.kills}-{player.homePlayer?.deaths}-{player.homePlayer?.assists}</TableCell>
                                  <TableCell align="right">{player.homePlayer?.damage}</TableCell>
                                  <TableCell align="right">{player.awayPlayer?.damage}</TableCell>
                                  <TableCell align="right">{player.awayPlayer?.kills}-{player.awayPlayer?.deaths}-{player.awayPlayer?.assists}</TableCell>
                                  <TableCell align="right">
                                    {player.awayPlayer &&
                                        <MatchDetailsModal
                                            buttonText="More"
                                            data={player.awayPlayer}
                                        />
                                    }
                                    {player.awayPlayer?.name}
                                  </TableCell>
                                </TableRow>
                              ))}
                          </TableBody>
                      </Table>
                  </TableContainer>
              </CardContent>
          </Card>
      }
    </Box>
  );
}
