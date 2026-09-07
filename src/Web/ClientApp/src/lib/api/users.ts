import {api} from "$lib/api/client";

export type PrivacyPreferences = {
    allowAdding: boolean;
    allowRemoving: boolean;
};

export async function getPrivacyPreferences(): Promise<PrivacyPreferences> {
    const response = await api.get<PrivacyPreferences>("/api/v1/users/me/preferences");
    return response.data;
}

export async function updatePrivacyPreferences(
    payload: PrivacyPreferences,
): Promise<PrivacyPreferences> {
    const response = await api.put<PrivacyPreferences>("/api/v1/users/me/preferences", payload);
    return response.data;
}