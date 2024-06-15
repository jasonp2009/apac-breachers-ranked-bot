import { AbrApiClient } from "./generated/abrApiClient";
import { loginCookieKey } from "../hooks";
import { useCookies } from "react-cookie";

export function useAbrApiClient() {
  const [ cookies ] = useCookies([loginCookieKey]);
  return new AbrApiClient(
    process.env.REACT_APP_ABR_BASE_URL,
    {
      fetch: async (url: RequestInfo, init?: RequestInit): Promise<Response> => {
        const token = cookies[loginCookieKey].access_token;
        if (!!token && !!init) {
          const headers: Headers = new Headers()
          headers.append('Authorization', `Bearer ${token}`)
          init.headers = headers;
        }
        return await fetch(url, init);
      }
    }
  );
}
