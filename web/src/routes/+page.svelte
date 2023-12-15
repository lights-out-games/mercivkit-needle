<script lang="ts">
	import unity from "$lib/MercivUnityBridge";
    import unityEditor from "$lib/MercivUnityEditorBridge";
	import { onDestroy } from "svelte";
	import UnityWindow from "../lib/UnityWindow.svelte";
	import log from "$lib/log";

    import { onMount } from 'svelte';

    let unityBridge = null;
    
    $: startedEditor = unityEditor.started;
    $: bridgeStatus = "local bridge active"
    
    // ensure browser window is available
    // https://www.okupter.com/blog/sveltekit-window-is-not-defined
    onMount(() => {

        console.log(window.innerWidth);

        unityBridge = unityEditor

        unityBridge.createRpc("add", (n1: number, n2: number, n3: number) => {
            log.info(`JS: Adding ${n1} + ${n2} + ${n3}`);
            return n1 + n2 + n3;
        });

        unityBridge.subscribe("ready", async () => {
            const response = await unityBridge.call("hello", "Hello from Svelte!")
            log.info(response);
        });

        unityBridge.setUnityWindow(window)
        log.info("JS>EDITOR: Registering as browser");

        // if ($startedEditor) {
        //     timer = setInterval(() => {
        //         log.info("JS>EDITOR: Sending clock event");
        //         unityEditor.raiseEvent("clock", new Date().toLocaleDateString());
        //     }, 5000);
        // }
    });

    let timer: any;

    $: started = unity.started;
    $: if ($started) {
        unityBridge = unity;
        unityBridge.createRpc("add", (n1: number, n2: number, n3: number) => {
            log.info(`JS: Adding ${n1} + ${n2} + ${n3}`);
            return n1 + n2 + n3;
        });

        unityBridge.subscribe("ready", async () => {
            const response = await unity.call("hello", "Hello from Svelte!")
            log.info(response);
        });

        // timer = setInterval(() => {
        //     log.info("JS>UNITY: Sending clock event");
        //     unity.raiseEvent("clock", new Date().toLocaleDateString());
        // }, 1000);
    }

    onDestroy(() => {
        clearInterval(timer);
    });

    // unityEditor.setUnityWindow()

</script>

<!-- <UnityWindow /> -->

<div class="absolute left-1/2 -translate-x-1/2">
    <h1 class="text-4xl text-center text-white pt-20">Welcome to MercivKit</h1>
    <p>{startedEditor}</p>
    <p>{bridgeStatus}</p>
</div>