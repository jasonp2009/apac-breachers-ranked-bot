import * as React from 'react';
import Box from '@mui/material/Box';
import Modal from '@mui/material/Modal';
import Button from '@mui/material/Button';
import { WeaponData} from "../../api/generated/abrApiClient";
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import {Select, SelectChangeEvent} from '@mui/material';
import MenuItem from "@mui/material/MenuItem";
import { weaponSortFn } from "../../functions";

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
  const [selectedWeapon, setSelectedWeapon] = React.useState<WeaponData | undefined>(
    props.data.sort(weaponSortFn)[0]);
  const handleOpen = () => {
    setOpen(true);
  };
  const handleClose = () => {
    setOpen(false);
  };

  const handleChange = (event: SelectChangeEvent) => {
    var weaponName = event.target.value as string;
    setSelectedWeapon(props.data.find(weapon => weapon.name === weaponName));
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
            <Select
              labelId="demo-simple-select-label"
              id="demo-simple-select"
              value={selectedWeapon?.name ?? ''}
              label="Age"
              onChange={handleChange}
            >
              {props.data.sort(weaponSortFn).map(weapon =>
                weapon.name && (
                  <MenuItem value={weapon.name}>{weapon.name}</MenuItem>
                )
              )};
            </Select>
            { selectedWeapon && (
              <Card sx={{ minWidth: 275 }}>
                <CardContent>
                  <p>Kills: {selectedWeapon.kills}</p>
                  <p>Headshot kills: {selectedWeapon.headshotKills}</p>
                  <p>Damage: {selectedWeapon.damage}</p>
                  <p>Friendly damage: {selectedWeapon.friendlyDamage}</p>
                  <p>Shots fired: {selectedWeapon.shotsFired}</p>
                  <p>Hits: {selectedWeapon.hits}</p>
                  <p>Headshots: {selectedWeapon.headshots}</p>
                </CardContent>
              </Card>
            )}
          </div>
          <Button onClick={handleClose}>Close</Button>
        </Box>
      </Modal>
    </React.Fragment>
  );
}
