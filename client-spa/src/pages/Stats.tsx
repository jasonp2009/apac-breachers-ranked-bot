import Iframe from "react-iframe";
import Box from "@mui/material/Box";
import { drawerWidth } from "../components";
import { useLogin } from "../hooks";
import { useAbrApiClient } from "../api/useAbrApiClient";
import { useEffect, useState } from "react";

export function Stats() {
  const [ breachersTrackerUrl, setBreachersTrackerUrl ] = useState<string>();
  const { isLoggedIn } = useLogin();
  const abrApi = useAbrApiClient();

  useEffect(() => {
    isLoggedIn && abrApi.getLinkedBreachersUser().then(resp => {
      resp && setBreachersTrackerUrl(`https://breacherstracker.com/profile/${resp.id}`);
    });
  }, [isLoggedIn]);
  
  return (
    <Box
      sx={{
        
        width: {
          xs: `calc(100vw - 48px)`,
          sm: `calc(100vw - (${drawerWidth}px + 68px))`
        },
        height: {
          xs: "100vh",
          sm: "calc(100vh - 112px)"
        }
      }}
    >
      {isLoggedIn && breachersTrackerUrl
        ? (
          <Iframe url={breachersTrackerUrl}
                  height="100%"
                  width="100%"
          />
        ) : isLoggedIn
        ? (
          <p>Please link your breachers account on the account page to view stats.</p>
        ) : (
          <p>Please log in with your discord account to view stats.</p>
        )
      }
    </Box>
  );
}
