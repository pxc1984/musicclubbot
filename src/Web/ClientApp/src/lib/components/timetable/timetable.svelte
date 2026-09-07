<script lang="ts">
    import type {TimetableEvent} from "$lib/timetable/types";
    import {SvelteDate} from "svelte/reactivity";

    import TimetableHeader from "./timetable-header.svelte";
    import TimetableTimeColumn from "./timetable-time-column.svelte";
    import TimetableGrid from "./timetable-grid.svelte";

    let {
        date = new Date(),
        events = [],
        startHour = 0,
        endHour = 24,
        hourHeight = 80
    }: {
        date?: Date;
        events?: TimetableEvent[];
        startHour?: number;
        endHour?: number;
        hourHeight?: number;
    } = $props();

    // eslint-disable-next-line svelte/prefer-writable-derived -- локальное изменяемое состояние, синхронизируемое с пропом date
    let selectedDate = $state(new Date());

    $effect(() => {
        selectedDate = new SvelteDate(date);
    });

    function previousDay() {
        const next = new SvelteDate(selectedDate);
        next.setDate(next.getDate() - 1);
        selectedDate = next;
    }

    function nextDay() {
        const next = new SvelteDate(selectedDate);
        next.setDate(next.getDate() + 1);
        selectedDate = next;
    }

    function today() {
        selectedDate = new SvelteDate();
    }
</script>

<div class="flex h-full flex-col overflow-hidden border">
    <TimetableHeader
        date={selectedDate}
        onPrevious={previousDay}
        onNext={nextDay}
        onToday={today}
    />

    <div class="flex flex-1 overflow-y-auto">
        <TimetableTimeColumn
            {startHour}
            {endHour}
            {hourHeight}
        />

        <TimetableGrid
            {events}
            {startHour}
            {endHour}
            {hourHeight}
        />
    </div>
</div>
