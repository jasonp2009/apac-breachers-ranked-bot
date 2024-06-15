import { BreachersLink } from "../../components";
import Box from "@mui/material/Box";

export function Account() {
  
  return (
    <Box sx={
      { width: {
        xs: '100%',
        sm: '25rem'
      }}
    }>
      <h2>My Account</h2>
      <BreachersLink />
    </Box>
  );
}
