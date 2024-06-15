import { BreachersLink } from "../../components";
import Box from "@mui/material/Box";
import { useLogin } from "../../hooks";

export function Account() {
  const { isLoggedIn } = useLogin();
  return isLoggedIn ? (
    <Box sx={
      { width: {
        xs: '100%',
        sm: '25rem'
      }}
    }>
      <h2>My Account</h2>
      <BreachersLink />
    </Box>
  ) : (
    <h2>Please login to access this page.</h2>
  );
}
