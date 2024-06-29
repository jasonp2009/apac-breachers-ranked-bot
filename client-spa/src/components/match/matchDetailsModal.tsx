import * as React from 'react';
import Box from '@mui/material/Box';
import Modal from '@mui/material/Modal';
import Button from '@mui/material/Button';
import { GamePlayerData } from "../../api/generated/abrApiClient";
import { WeaponDetailsModal } from "./WeaponDetailsModal";
import { GadgetDetailsModal } from "./GadgetDetailsModal";
import { weaponSortFn, gadgetSortFn } from "../../functions";

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
export function MatchDetailsModal(props: {
  buttonText: string,
  data: GamePlayerData
}) {
  const [open, setOpen] = React.useState(false);
  const handleOpen = () => {
    setOpen(true);
  };
  const handleClose = () => {
    setOpen(false);
  };
  
  const favouriteWeapon = props.data.weapons?.sort(weaponSortFn)[0].type;
  const favouriteGadget = props.data.gadgets?.sort(gadgetSortFn)[0].type;

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
          <h2>{props.data.name}</h2>
          <p>MMR: {props.data.mmr}</p>
          <p>Rank: {props.data.rank}</p>
          <p>Side: {props.data.side}</p>
          <p>KDA: {props.data.kills}-{props.data.deaths}-{props.data.assists}</p>
          <p>Damage: {props.data.damage}</p>
          <p>MVPs: {props.data.roundMvps}</p>
          <p>Aces: {props.data.aces}</p>
          <p>Game time: {props.data.gameTime}</p>
          <p>Favourite Weapon: {favouriteWeapon}</p>
          <p>Favourite Gadget: {favouriteGadget}</p>
          { props.data.weapons &&
            <WeaponDetailsModal
                buttonText="Weapons"
                data={props.data.weapons}
            />
          }
          { props.data.gadgets &&
              <GadgetDetailsModal
                  buttonText="Gadgets"
                  data={props.data.gadgets}
              />
          }
          <Button
            sx={{float: 'right'}}
            onClick={handleClose}>Close</Button>
        </Box>
      </Modal>
    </React.Fragment>
  );
}
