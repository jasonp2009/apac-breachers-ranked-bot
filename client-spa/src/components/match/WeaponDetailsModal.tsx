import * as React from 'react';
import Box from '@mui/material/Box';
import Modal from '@mui/material/Modal';
import Button from '@mui/material/Button';
import { WeaponData} from "../../api/generated/abrApiClient";
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Typography from '@mui/material/Typography';

const style = {
  position: 'absolute' as 'absolute',
  top: '50%',
  left: '50%',
  transform: 'translate(-50%, -50%)',
  width: 400,
  bgcolor: 'background.paper',
  border: '2px solid #000',
  boxShadow: 24,
  pt: 2,
  px: 4,
  pb: 3,
};
export function WeaponDetailsModal(props: {
  buttonText: string,
  data: WeaponData[]
}) {
  const [open, setOpen] = React.useState(false);
  const handleOpen = () => {
    setOpen(true);
  };
  const handleClose = () => {
    setOpen(false);
  };

  return (
    <React.Fragment>
      <Button onClick={handleOpen}>{props.buttonText}</Button>
      <Modal
        open={open}
        onClose={handleClose}
        aria-labelledby="child-modal-title"
        aria-describedby="child-modal-description"
      >
        <Box sx={{...style, width: 300}}>
          <div>
            { props.data.map(weapon => (
                <Card sx={{ minWidth: 275 }}>
                  <CardContent>
                    <Typography sx={{fontSize: 14}} color="text.secondary" gutterBottom>
                      {weapon.name}
                    </Typography>
                    <p>Kills: {weapon.kills}</p>
                    <p>Headshot kills: {weapon.headshotKills}</p>
                    <p>Damage: {weapon.damage}</p>
                    <p>Friendly damage: {weapon.friendlyDamage}</p>
                    <p>Shots fired: {weapon.shotsFired}</p>
                    <p>Hits: {weapon.hits}</p>
                    <p>Headshots: {weapon.headshots}</p>
                  </CardContent>
                </Card>
              )
            )}
          </div>
          <Button onClick={handleClose}>Close</Button>
        </Box>
      </Modal>
    </React.Fragment>
  );
}