import {User} from "react-discord-login/dist/DiscordLoginTypes";

export function getProfilePicture(user?: User) {
  return !!user
    ? `https://cdn.discordapp.com/avatars/${user.id}/${user.avatar}.png`
    : '';
}
