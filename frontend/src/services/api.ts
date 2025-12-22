import { createClient } from "@connectrpc/connect";
import { create } from "@bufbuild/protobuf";

import { transport } from "./config";
import { AuthService, ProfileResponseSchema, TgLoginRequestSchema } from "../proto/auth_pb";
import { SongService, CreateSongRequestSchema, JoinRoleRequestSchema, LeaveRoleRequestSchema, ListSongsRequestSchema, SongIdSchema, UpdateSongRequestSchema } from "../proto/song_pb";
import { EventService, CreateEventRequestSchema, EventIdSchema, ListEventsRequestSchema, SetTracklistRequestSchema, UpdateEventRequestSchema } from "../proto/event_pb";
import type { Timestamp } from "@bufbuild/protobuf/wkt";

export const authClient = createClient(AuthService, transport);
export const songClient = createClient(SongService, transport);
export const eventClient = createClient(EventService, transport);

export function loginWithTelegram(initData: string, tgUserId?: number | string) {
	return authClient.loginWithTelegram(
		create(TgLoginRequestSchema, {
			initData,
			tgUserId: tgUserId ? BigInt(tgUserId) : BigInt(0),
		}),
	);
}

export function getProfile() {
	return authClient.getProfile({});
}

export function listSongs(query = "", pageToken = "", pageSize = 20) {
	return songClient.listSongs({ query, pageToken, pageSize });
}

export function getSong(id: string) {
	return songClient.getSong({ id });
}

export function createSong(payload: {
	title: string;
	artist: string;
	description?: string;
	linkUrl: string;
	linkKind: number;
	roles: string[];
}) {
	return songClient.createSong(
		{
			title: payload.title,
			artist: payload.artist,
			description: payload.description ?? "",
			link: { url: payload.linkUrl, kind: payload.linkKind },
			availableRoles: payload.roles,
		}
	);
}

export function updateSong(payload: {
	id: string;
	title: string;
	artist: string;
	description?: string;
	linkUrl: string;
	linkKind: number;
	roles: string[];
}) {
	return songClient.updateSong(
		{
			id: payload.id,
			title: payload.title,
			artist: payload.artist,
			description: payload.description ?? "",
			link: { url: payload.linkUrl, kind: payload.linkKind },
			availableRoles: payload.roles,
		},
	);
}

export function deleteSong(id: string) {
	return songClient.deleteSong({ id });
}

export function joinSongRole(songId: string, role: string) {
	return songClient.joinRole({ songId, role });
}

export function leaveSongRole(songId: string, role: string) {
	return songClient.leaveRole({ songId, role });
}

export function listEvents(from?: Timestamp, to?: Timestamp, limit = 50) {
	return eventClient.listEvents({ from, to, limit });
}

export function getEvent(id: string) {
	return eventClient.getEvent({ id });
}

export function createEvent(payload: {
	title: string;
	startAt?: Timestamp;
	location?: string;
	notifyDayBefore?: boolean;
	notifyHourBefore?: boolean;
	tracklist?: { order: number; songId: string; customTitle: string; customArtist: string }[];
}) {
	return eventClient.createEvent(
		{
			title: payload.title,
			startAt: payload.startAt,
			location: payload.location ?? "",
			notifyDayBefore: payload.notifyDayBefore ?? false,
			notifyHourBefore: payload.notifyHourBefore ?? false,
			tracklist: payload.tracklist
				? { items: payload.tracklist.map((i) => ({ order: i.order, songId: i.songId, customTitle: i.customTitle, customArtist: i.customArtist })) }
				: undefined,
		},
	);
}

export function updateEvent(payload: {
	id: string;
	title: string;
	startAt?: Timestamp;
	location?: string;
	notifyDayBefore?: boolean;
	notifyHourBefore?: boolean;
}) {
	return eventClient.updateEvent(
		{
			id: payload.id,
			title: payload.title,
			startAt: payload.startAt,
			location: payload.location ?? "",
			notifyDayBefore: payload.notifyDayBefore ?? false,
			notifyHourBefore: payload.notifyHourBefore ?? false,
		},
	);
}

export function deleteEvent(id: string) {
	return eventClient.deleteEvent({ id });
}

export function setTracklist(eventId: string, items: { order: number; songId: string; customTitle: string; customArtist: string }[]) {
	return eventClient.setTracklist(
		{
			eventId,
			tracklist: { items: items.map((i) => ({ order: i.order, songId: i.songId, customTitle: i.customTitle, customArtist: i.customArtist })) },
		},
	);
}
