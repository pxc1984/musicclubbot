<script lang="ts">
    import {User} from "@lucide/svelte";
    import * as Avatar from "$lib/components/ui/avatar";
    import {Badge} from "$lib/components/ui/badge";
    import * as Command from "$lib/components/ui/command";
    import * as DropdownMenu from "$lib/components/ui/dropdown-menu";
    import {getRoleCandidates, joinSongRole, leaveSongRole} from "$lib/api/songs";
    import type {Song, SongRole, SongUser} from "$lib/songs/types";
    import {Permission} from "$lib/permissions/resolve";
    import type {UUID} from "node:crypto";

    let {
        songId,
        role,
        currentUser,
        removableUserIds,
        onupdated,
    }: {
        songId: UUID;
        role: SongRole;
        currentUser: {id: UUID; permissions: string[]} | null;
        removableUserIds: Set<string>;
        onupdated?: (song: Song) => void;
    } = $props();

    let assignOpen = $state(false);
    let query = $state("");
    let candidates = $state<SongUser[]>([]);
    let loadingCandidates = $state(false);
    let acting = $state(false);

    const isVacant = $derived(role.assignment === null);
    const member = $derived(role.assignment?.user ?? null);
    const isYou = $derived(member?.id === currentUser?.id);
    const canAssign = $derived(
        currentUser !== null &&
        ((currentUser.permissions ?? []).includes(Permission.ParticipationEditOwn) ||
            (currentUser.permissions ?? []).includes(Permission.ParticipationEditAny))
    );
    const canRemoveMember = $derived(
        member !== null && currentUser !== null && removableUserIds.has(member.id)
    );

    // При открытии дропдауна и при изменении поиска (с дебаунсом) грузим кандидатов.
    $effect(() => {
        if (!assignOpen || !isVacant || !canAssign) return;

        const handle = setTimeout(() => {
            loadCandidates();
        }, 150);

        return () => clearTimeout(handle);
    });

    async function loadCandidates() {
        loadingCandidates = true;
        try {
            const result = await getRoleCandidates(
                songId,
                role.id,
                query || undefined,
                "assign",
            );
            candidates = result.users;
        } finally {
            loadingCandidates = false;
        }
    }

    async function assign(user: SongUser) {
        if (acting) return;
        acting = true;
        try {
            const updated = await joinSongRole(role.id, {actorUserId: user.id});
            onupdated?.(updated);
            assignOpen = false;
        } finally {
            acting = false;
        }
    }

    async function remove() {
        if (!member || acting) return;
        acting = true;
        try {
            const updated = await leaveSongRole(role.id, {actorUserId: member.id});
            onupdated?.(updated);
        } finally {
            acting = false;
        }
    }

    function getInitials(name: string): string {
        return name
            .split(" ")
            .map((part) => part[0])
            .join("")
            .toUpperCase()
            .slice(0, 2);
    }
</script>

{#if isVacant}
    {#if canAssign}
        <DropdownMenu.Root
            bind:open={assignOpen}
            onOpenChange={(open) => {
                if (open) {
                    query = "";
                    candidates = [];
                }
            }}
        >
            <DropdownMenu.Trigger
                class="flex w-full items-center justify-between p-3 rounded-lg bg-muted/50 text-left transition-colors hover:bg-muted cursor-pointer disabled:opacity-60"
                disabled={acting}
                aria-label={`Назначить на роль ${role.title}`}
            >
                <span class="flex items-center gap-3">
                    <Avatar.Root class="size-8">
                        <Avatar.Fallback class="text-xs bg-muted">
                            <User class="size-4"/>
                        </Avatar.Fallback>
                    </Avatar.Root>
                    <div>
                        <p class="text-sm font-medium">{role.title}</p>
                        <p class="text-xs text-muted-foreground">Свободно</p>
                    </div>
                </span>
                <Badge variant="ghost">нажми чтоб зайти</Badge>
            </DropdownMenu.Trigger>

            <DropdownMenu.Content class="w-72 p-1">
                <Command.Root>
                    <Command.Input bind:value={query} placeholder="Поиск участника..."/>
                    <Command.List>
                        {#if loadingCandidates}
                            <Command.Empty>Загрузка...</Command.Empty>
                        {:else if candidates.length === 0}
                            <Command.Empty>Никого нельзя назначить</Command.Empty>
                        {:else}
                            {#each candidates as user (user.id)}
                                <Command.Item
                                    value={user.displayName}
                                    onSelect={() => assign(user)}
                                    disabled={acting}
                                >
                                    <Avatar.Root class="size-6">
                                        <Avatar.Image
                                            src={user.avatarUrl}
                                            alt={user.displayName}
                                        />
                                        <Avatar.Fallback class="text-xs">
                                            {getInitials(user.displayName)}
                                        </Avatar.Fallback>
                                    </Avatar.Root>
                                    <span class="truncate">{user.displayName}</span>
                                    {#if user.id === currentUser?.id}
                                        <Badge
                                            variant="ghost"
                                            class="ml-auto shrink-0"
                                        >
                                            вы
                                        </Badge>
                                    {/if}
                                </Command.Item>
                            {/each}
                        {/if}
                    </Command.List>
                </Command.Root>
            </DropdownMenu.Content>
        </DropdownMenu.Root>
    {:else}
        <div
            class="flex w-full items-center justify-between p-3 rounded-lg bg-muted/50 text-left cursor-default"
        >
            <span class="flex items-center gap-3">
                <Avatar.Root class="size-8">
                    <Avatar.Fallback class="text-xs bg-muted">
                        <User class="size-4"/>
                    </Avatar.Fallback>
                </Avatar.Root>
                <div>
                    <p class="text-sm font-medium">{role.title}</p>
                    <p class="text-xs text-muted-foreground">Свободно</p>
                </div>
            </span>
            <Badge variant="ghost">свободно</Badge>
        </div>
    {/if}
{:else if member}
    {#if canRemoveMember}
        <button
            type="button"
            class="flex w-full items-center justify-between p-3 rounded-lg bg-muted/50 text-left transition-colors hover:bg-muted cursor-pointer disabled:opacity-60"
            onclick={remove}
            disabled={acting}
            title={isYou ? "Нажми чтоб выйти" : "Нажми чтоб снять с роли"}
            aria-label={isYou ? "Выйти из роли" : `Снять ${member.displayName} с роли`}
        >
            <span class="flex items-center gap-3">
                <Avatar.Root class="size-8">
                    <Avatar.Image
                        src={member.avatarUrl}
                        alt={member.displayName}
                    />
                    <Avatar.Fallback class="text-xs">
                        {getInitials(member.displayName)}
                    </Avatar.Fallback>
                </Avatar.Root>
                <div>
                    <p class="text-sm font-medium">{role.title}</p>
                    <p class="text-xs text-muted-foreground">{member.displayName}</p>
                </div>
            </span>
            {#if isYou}
                <Badge variant="ghost">нажми чтоб выйти</Badge>
            {:else}
                <Badge variant="default">занято</Badge>
            {/if}
        </button>
    {:else}
        <div
            class="flex w-full items-center justify-between p-3 rounded-lg bg-muted/50 text-left cursor-default"
        >
            <span class="flex items-center gap-3">
                <Avatar.Root class="size-8">
                    <Avatar.Image
                        src={member.avatarUrl}
                        alt={member.displayName}
                    />
                    <Avatar.Fallback class="text-xs">
                        {getInitials(member.displayName)}
                    </Avatar.Fallback>
                </Avatar.Root>
                <div>
                    <p class="text-sm font-medium">{role.title}</p>
                    <p class="text-xs text-muted-foreground">{member.displayName}</p>
                </div>
            </span>
            <Badge variant="default">занято</Badge>
        </div>
    {/if}
{/if}