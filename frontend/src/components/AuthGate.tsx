import React, { useMemo, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Code, ConnectError } from "@connectrpc/connect";

import { getProfile, loginWithTelegram } from "../services/api";
import { setToken } from "../services/config";
import SongList from "./SongList";
import EventList from "./EventList";
import type { PermissionSet } from "../proto/permissions_pb";
import type { ProfileResponse } from "../proto/auth_pb";
import type { User } from "../proto/user_pb";

const AuthGate: React.FC = () => {
	const queryClient = useQueryClient();
	const [manualUserId, setManualUserId] = useState("");
	const [loginError, setLoginError] = useState<string | null>(null);

	const oauthParams = useMemo(() => new URLSearchParams(window.location.search), []);
	const oauthUserId = oauthParams.get("id");
	const hasOAuthPayload = oauthUserId && oauthParams.get("hash");

	const profileQuery = useQuery({
		queryKey: ["profile"],
		queryFn: () => getProfile(),
	});

	const loginMutation = useMutation({
		mutationFn: async () => {
			setLoginError(null);
			const initData = hasOAuthPayload ? oauthParams.toString() : "";
			const tgId = hasOAuthPayload ? Number(oauthUserId) : manualUserId ? Number(manualUserId) : undefined;

			if (!initData && !tgId) {
				throw new Error("Нет данных авторизации Telegram. Авторизуйтесь через OAuth или укажите TG ID вручную.");
			}

			const session = await loginWithTelegram(initData, tgId);
			setToken(session.accessToken);
			await queryClient.invalidateQueries({ queryKey: ["profile"] });
			return session;
		},
		onError: (err: any) => {
			if (err instanceof ConnectError) {
				setLoginError(err.message);
			} else {
				setLoginError((err as Error).message);
			}
		},
	});

	const isUnauthedCode = profileQuery.isError && (profileQuery.error as ConnectError)?.code === Code.Unauthenticated;
	const profile = profileQuery.data?.profile as User | undefined;
	const permissions = profileQuery.data?.permissions as PermissionSet | undefined;

	if (profileQuery.isLoading) {
		return <div className="card">Загружаем профиль…</div>;
	}

	if (isUnauthedCode) {
		return (
			<div className="card" style={{ maxWidth: 480, margin: "80px auto" }}>
				<div className="card-title" style={{ marginBottom: 12 }}>
					<span role="img" aria-label="bolt">
						⚡
					</span>
					Войти через Telegram
				</div>
				<p style={{ color: "var(--muted)", lineHeight: 1.4 }}>
					Авторизация через Telegram OAuth. Нажмите кнопку ниже или введите TG ID, если хотите форснуть вход.
				</p>
				<div style={{ display: "grid", gap: 12, marginTop: 12 }}>
					<button className="button" onClick={() => (window.location.href = buildTelegramOAuthUrl())}>
						Войти через Telegram OAuth
					</button>
					<input
						className="input"
						placeholder="Опционально: TG user id"
						value={manualUserId}
						onChange={(e) => setManualUserId(e.target.value)}
					/>
					<button className="button" onClick={() => loginMutation.mutate()} disabled={loginMutation.isPending}>
						{loginMutation.isPending ? "Входим…" : "Войти"}
					</button>
					{loginError && <div style={{ color: "var(--danger)" }}>{loginError}</div>}
				</div>
			</div>
		);
	}

	if (profileQuery.isError) {
		return <div className="card">Ошибка загрузки профиля: {(profileQuery.error as Error).message}</div>;
	}

	const hero = (
		<div className="card" style={{ marginBottom: 18 }}>
			<div className="section-header">
				<div className="card-title">
					<span role="img" aria-label="music">
						🎸
					</span>
					Музыкальный клуб
				</div>
				<div className="pill">
					<div
						className="status-dot"
						style={{ background: profileQuery.data?.permissions ? "var(--accent)" : "var(--muted)" }}
					/>
					{profile?.displayName}
				</div>
			</div>
			<p style={{ color: "var(--muted)", marginBottom: 12 }}>
				Собираем сет-листы, треклисты и роли для ближайших мероприятий.
			</p>
			{profileQuery.data && "isChatMember" in profileQuery.data ? null : null}
		</div>
	);

	return (
		<div className="grid">
			{hero}
			<SongList permissions={permissions} profile={profile} />
			<EventList permissions={permissions} />
		</div>
	);
};

export default AuthGate;

function buildTelegramOAuthUrl() {
	const botId = import.meta.env.VITE_TELEGRAM_BOT_ID;
	if (!botId) {
		throw new Error("Не задан VITE_TELEGRAM_BOT_ID");
	}
	const redirectUrl = encodeURIComponent(window.location.origin + window.location.pathname);
	return `https://oauth.telegram.org/auth?bot_id=${botId}&origin=${redirectUrl}&embed=0&request_access=write`;
}
