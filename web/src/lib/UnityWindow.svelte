<script lang="ts">
	import unity from "./MercivUnityBridge";

    let iframe: HTMLIFrameElement;

    $: if (iframe) {
        unity.setUnityWindow(iframe.contentWindow!);
    }
</script>

<div class="absolute inset-0 bg-red-500">
    <iframe bind:this={iframe} id="unity-iframe" src="/WebGL/index.html" title="Unity App"></iframe>
</div>

<style>
    iframe {
        display: block;
        background-color: black;
        width: 100%;
        height: 100%;
        margin: 0px;
        padding: 0px;
        border: none;
        overflow: none;
    }
</style>