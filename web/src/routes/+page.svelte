<script lang="ts">
	import unity from "$lib/MercivUnityBridge";
	import { onDestroy } from "svelte";
	import UnityWindow from "../lib/UnityWindow.svelte";
	import log from "$lib/log";

    let timer: any;

    $: started = unity.started;
    $: if ($started) {
        unity.createRpc("add", (n1: number, n2: number, n3: number) => {
            log.info(`JS: Adding ${n1} + ${n2} + ${n3}`);
            return n1 + n2 + n3;
        });

        unity.subscribe("ready", async () => {
            const response = await unity.call("hello", "Hello from Svelte!")
            log.info(response);
        });

        timer = setInterval(() => {
            log.info("JS: Sending clock event");
            unity.raiseEvent("clock", new Date().toLocaleDateString());
        }, 1000);
    }

    onDestroy(() => {
        clearInterval(timer);
    });

</script>

<UnityWindow />

<div class="absolute left-1/2 -translate-x-1/2">
    <h1 class="text-4xl text-center text-white pt-20">Welcome to MercivKit!</h1>
</div>