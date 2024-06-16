import { useAbrApiClient } from "../../api/useAbrApiClient";
import React, { useEffect, useState } from "react";
import { Autocomplete, Button, LinearProgress, Modal, TextField} from "@mui/material";
import QuestionMarkIcon from '@mui/icons-material/QuestionMark';
import IconButton from '@mui/material/IconButton';
import Tooltip from '@mui/material/Tooltip';
import { BreachersUser } from "../../api/generated/abrApiClient";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";

export function BreachersLink() {
  const [ isLoading, setIsLoading ] = useState<boolean>(true);
  const [ breachersUser, setBreachersUser ] = useState<BreachersUser>();
  const [ searchValue, setSearchValue ] = useState<BreachersUser[]>();
  const [ isModalOpen, setIsModalOpen ] = useState<boolean>(false);
  const [ selectedBreachersUser, setSelectedBreachersUser ] = useState<BreachersUser>();
  const abrApi = useAbrApiClient();

  useEffect(() => {
    abrApi.getLinkedBreachersUser().then(resp => {
      setBreachersUser(resp);
      setIsLoading(false);
    })
  }, []);
  
  const searchForBreachersUser = async (searchString: string) => {
    if (searchString !== undefined && searchString.length !== 0) {
      if (searchString.includes("breacherstracker")) {
        const splitString = searchString.split('/');
        const breacherUserId = splitString[splitString.length - 1];
        const resp = await abrApi.getUser(breacherUserId);
        selectBreachersUser(resp);
      } else {
        const resp = await abrApi.searchUsers(searchString);
        setSearchValue(resp)
      }
    } else {
      setSearchValue(undefined);
    }
  }
  
  const selectBreachersUser = (breachersUser: BreachersUser) => {
    setSelectedBreachersUser(breachersUser);
    setIsModalOpen(true);
  }
  const confirmBreachersUser = async () => {
    if (!selectedBreachersUser?.id) {
      throw new Error("An unexpected error occurred. Please try re-selecting a breachers user.");
    }
    await abrApi.linkDiscordUser(selectedBreachersUser?.id);
    setBreachersUser(selectedBreachersUser);
    setIsModalOpen(false);
  }
  
  return (
    <Box sx={{ width: '100%' }}>
      { isLoading && <LinearProgress /> }
      { !isLoading && (
        breachersUser ? (
          <TextField
            label="Breachers Account"
            value={breachersUser.fullUserName}
            InputProps={{
              readOnly: true,
            }}
            fullWidth
          />
        ) : (
          <Box sx={{display:'flex'}}>
            <Autocomplete
              sx={{flex: 30}}
              id="breachers-user-autocomplete"
              freeSolo
              options={searchValue?? []}
              filterOptions={(x) => x}
              getOptionLabel={(option) => (option as BreachersUser).fullUserName ?? ""}
              onInputChange={async (event, newInputValue) => {
                if (event.type === 'change') {
                  await searchForBreachersUser(newInputValue);
                }
              }}
              onChange={(event, selectedUser) => {
                console.log(event);
                selectBreachersUser(selectedUser as BreachersUser);
              }}
              renderInput={(params) => <TextField {...params} label="Breachers User Name" />}
            />
            <Tooltip
              sx={{flexGrow: 2, flexShrink: 1}}
              title={(
                <Typography>If you can't find your username, you can enter your breacher's tracker link instaed</Typography>
              )}>
              <IconButton>
                <QuestionMarkIcon />
              </IconButton>
            </Tooltip>
          </Box>
        )
      )}
      <Modal
        open={isModalOpen}
        aria-labelledby="modal-modal-title"
        aria-describedby="modal-modal-description"
      >
        <Box sx={{
          position: 'absolute' as 'absolute',
          top: '50%',
          left: '50%',
          transform: 'translate(-50%, -50%)',
          width: {
            xs: '65%',
            sm: 800
          },
          bgcolor: 'background.paper',
          border: '2px solid #000',
          boxShadow: 24,
          p: 4,
        }}>
          <Typography variant="h6" component="h2">
            Confirm Breachers Account
          </Typography>
          <Typography sx={{ mt: 2 }}>
            Clicking the confirm button will link your discord account with the following breachers account
            <br/><br/>
            <b>{selectedBreachersUser?.fullUserName}</b>
            <br/><br/>
            Intentionally linking your discord account with a breachers account that does not belong to you will result
            in a <b>permanent ban</b> on the APAC Breachers Ranked discord bot and you will be unable to queue for matches.
            <br/>
          </Typography>
          <br/>
          <Button
            color='error'
            variant='contained'
            onClick={() => setIsModalOpen(false)}
          >
            Cancel
          </Button>
          <Button
            sx={{
              float: 'right'
            }}
            color='success'
            variant='contained'
            onClick={confirmBreachersUser}
          >
            Confirm
          </Button>
        </Box>
      </Modal>
    </Box>
  )
}
