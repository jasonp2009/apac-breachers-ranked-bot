import { useDiscordLogin } from 'react-discord-login';
import {useEffect, useState} from "react";
import { TokenResponse, User } from "react-discord-login/dist/DiscordLoginTypes";
import { useCookies} from "react-cookie";

const cookieKey = 'discordToken';
const expiryBuffer = 15*60;

export function useLogin() {
  const [ cookies, setCookie, deleteCookie ] = useCookies([cookieKey]);
  const [ user, setUser ] = useState<User>();

  useEffect(() => {
    const cookieUser = cookies[cookieKey]?.user;
    cookieUser && setUser(cookieUser);
  }, [cookies]);
  
  const { buildUrl, isLoading } = useDiscordLogin({
    clientId: '1138013824507711498',
    redirectUri: 'http://localhost:3000',
    responseType: 'token', // or 'code'
    scopes: ['identify', 'email'],
    onSuccess: response => {
      const tokenResponse = response as TokenResponse;
      setTokenResponseCookie(tokenResponse);
    }
  });
  
  const setTokenResponseCookie = (tokenResponse: TokenResponse) => {
    setCookie(cookieKey,tokenResponse, {
      maxAge: tokenResponse.expires_in - expiryBuffer
    });
  }

  const login = () => {
    window.location.href = buildUrl();
  }
  
  const logout = () => {
    setUser(undefined);
    deleteCookie(cookieKey);
  }

  return {
    isLoggedIn: !!user,
    user,
    login,
    logout
  };
}
