import { FormControlLabel, Switch } from "@mui/material";
import Box from "@mui/material/Box";
import { useEffect, useState } from "react";
import { useAbrApiClient } from "../../api/useAbrApiClient";
import { MatchDto } from "../../api/generated/abrApiClient";
import ListItem from "@mui/material/ListItem";
import { MatchCard } from '../../components';
import List from "@mui/material/List";
import { useLogin } from "../../hooks";
import {Route, useNavigate} from "react-router-dom";
import { RouteConstants } from "../../constants";

export function MatchHistory() {
  const [ myHistoryToggle, setMyHistoryToggle ] = useState<boolean>(false);
  const [ matches, setMatches ] = useState<MatchDto[]>();
  const { isLoggedIn } = useLogin();
  const navigate = useNavigate();

  const abrApi = useAbrApiClient();

  const loadMatchData = () => {
    setMatches(undefined);
    if (myHistoryToggle) {
      abrApi.userMatchHistory().then(resp => {
        setMatches(resp)
      });
    } else {
      abrApi.matchHistory().then(resp => {
        setMatches(resp)
      });
    }
  }
  
  useEffect(loadMatchData, [myHistoryToggle]);
  
  const viewMatchDetails = (matchId: string) => {
    navigate(`/${RouteConstants.MatchDetails}/${matchId}`);
  }
  
  return (
    <Box maxWidth={600}>
      {isLoggedIn && (
        <FormControlLabel control={
          <Switch
            onChange={(_, checked) => setMyHistoryToggle(checked)}
          />
        } label="Just my history" />
      )}
      <List sx={{display: 'flex', flexWrap: 'wrap'}}>
        {matches && matches.map(match => 
          <ListItem sx={{padding: '10px 0px'}}>
            <MatchCard sx={{flexGrow: 1, flexBasis: 320}} match={match} onClick={viewMatchDetails}/>
          </ListItem>
        )}
      </List>
    </Box>
  );
}
