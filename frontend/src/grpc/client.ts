import { createPromiseClient, Interceptor } from "@bufbuild/connect-web";
import { createConnectTransport } from "@bufbuild/connect-web";
import {
  AuthService,
  ConcertService,
  ParticipationService,
  SongService,
  UserService
} from "./schema";
import { getToken } from "../lib/auth";

const baseUrl = import.meta.env.VITE_GRPC_HOST ?? "http://localhost:8080";

const authInterceptor: Interceptor = (next) => async (req) => {
  const token = getToken();
  if (token) {
    req.header.set("Authorization", `Bearer ${token}`);
  }
  return next(req);
};

const transport = createConnectTransport({
  baseUrl,
  useBinaryFormat: true,
  interceptors: [authInterceptor]
});

export const songClient = createPromiseClient(SongService, transport);
export const concertClient = createPromiseClient(ConcertService, transport);
export const participationClient = createPromiseClient(
  ParticipationService,
  transport
);
export const authClient = createPromiseClient(AuthService, transport);
export const userClient = createPromiseClient(UserService, transport);
