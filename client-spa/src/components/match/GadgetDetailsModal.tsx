import * as React from 'react';
import Box from '@mui/material/Box';
import Modal from '@mui/material/Modal';
import Button from '@mui/material/Button';
import { GadgetData } from "../../api/generated/abrApiClient";
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import { Select, SelectChangeEvent } from '@mui/material';
import MenuItem from "@mui/material/MenuItem";
import { gadgetSortFn } from "../../functions";

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
export function GadgetDetailsModal(props: {
  buttonText: string,
  data: GadgetData[]
}) {
  const [open, setOpen] = React.useState(false);
  const [selectedGadget, setSelectedGadget] = React.useState<GadgetData | undefined>(
    props.data.sort(gadgetSortFn)[0]);
  const handleOpen = () => {
    setOpen(true);
  };
  const handleClose = () => {
    setOpen(false);
  };

  const handleChange = (event: SelectChangeEvent) => {
    var gadgetType = event.target.value as string;
    setSelectedGadget(props.data.find(gadget => gadget.type === gadgetType));
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
              value={selectedGadget?.type ?? ''}
              label="Age"
              onChange={handleChange}
            >
              {props.data.sort(gadgetSortFn).map(gadget =>
                gadget.type && (
                  <MenuItem value={gadget.type}>{gadget.type}</MenuItem>
                )
              )};
            </Select>
            { selectedGadget && (
              <Card sx={{ minWidth: 275 }}>
                <CardContent>
                  <p>Kills: {selectedGadget.kills}</p>
                  <p>Damage: {selectedGadget.damage}</p>
                  <p>Friendly damage: {selectedGadget.friendlyDamage}</p>
                  <p>Used: {selectedGadget.used}</p>
                  <p>Triggered: {selectedGadget.triggered}</p>
                  <p>Enemy triggered: {selectedGadget.enemyTriggered}</p>
                  <p>Destroyed: {selectedGadget.destroyed}</p>
                  <p>Healed: {selectedGadget.healed}</p>
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
