import { writable } from "svelte/store";
import log from "./log";
import { WebsocketEvent } from "websocket-ts";

interface MercivMessage {
    event: string;
    args: any[];
    'rpc-id'?: number;
    'response-id'?: number;
}

const messageStore = writable({ data: 'Start', time: new Date() });

let socket:WebSocket = null;
// import {
//   ArrayQueue,
//   ConstantBackoff,
//   Websocket,
//   WebsocketBuilder,
//   WebsocketEvent,
// } from "websocket-ts";

// // Initialize WebSocket with buffering and 1s reconnection delay
// const ws = new WebsocketBuilder(serverUrl)
//   .withBuffer(new ArrayQueue())           // buffer messages when disconnected
//   .withBackoff(new ConstantBackoff(1000)) // retry every 1s
//   .build();

// // Function to output & echo received messages
// const echoOnMessage = (i: Websocket, ev: MessageEvent) => {
//   console.log(`received message: ${ev.data}`);
//   i.send(`echo: ${ev.data}`);
// };

// // Add event listeners
// ws.addEventListener(WebsocketEvent.open, () => console.log("opened!"));
// ws.addEventListener(WebsocketEvent.close, () => console.log("closed!"));
// ws.addEventListener(WebsocketEvent.message, echoOnMessage);


class MercivUnityEditorBridge {
    private rpcId = 0;
    private eventHandlers: { [event: string]: ((...args: any[]) => any)[] } = {};
    private rpcs: { [rpcId: number]: (returnValue: any) => void } = {};
    private window: Window | null = null;

    public started = writable(false);

    public setUnityWindow(window) {
        log.verbose('JS setUnityWindow');
        this.window = window;
        // this.rpcId = 1;
        // this.eventHandlers = {};
        // this.rpcs = {};
        // this.window.addEventListener('merciv-unity-to-js', this.onUnityEditorMessage.bind(this) as EventListener);
        this.started.set(true);

        const serverUrl = 'ws://localhost:3000'
        socket = new window.WebSocket(serverUrl);

        // const addToArray = (event:any) => {
        //   messageStore.update(() => {
        //     return event;
        //   });
        // };

        log.verbose(this)

      socket.addEventListener('open', function (event) {
        log.info('JS open');
        const message = {
          'topic':'register', 
          'detail': 'browser',
        };
        socket.send(JSON.stringify(message))
      })

      socket.addEventListener('close', function (event) {
        log.info('JS setUnityWindcloseow');
        socket = null
      })

      socket.addEventListener('message', function (event) {
        log.info('JS message');
        log.info(event)
        this.onUnityEditorMessage(event.data)
      }.bind(this));
    }

    public async call<T>(event: string, ...args: any[]): Promise<T> {
        log.verbose('JS call', event);
        return new Promise<T>(resolve => {
            const rpcId = this.rpcId++;
            this.rpcs[rpcId] = resolve;
            this.send({ event, args, 'rpc-id': rpcId });
        });
    }

    public raiseEvent(event: string, ...args: any[]) {
        log.verbose('JS raiseEvent', event);
        this.send({ event, args });
    }

    public subscribe(event: string, handler: (...args: any[]) => any) {
        log.verbose('JS subscribe', event);
        if (!this.eventHandlers[event]) {
            this.eventHandlers[event] = [];
        }

        this.eventHandlers[event].push(handler);
    }

    public unsubscribe = (event: string, handler: (...args: any[]) => any) => {
        log.verbose('JS unsubscribe', event);
        this.eventHandlers[event] = this.eventHandlers[event].filter(h => h !== handler);
    }

    public createRpc<T>(event: string, handler: (...args: any[]) => any) {
        this.subscribe(event, handler);
    }

    public removeRpc = (event: string, handler: (...args: any[]) => any) => {
        this.unsubscribe(event, handler);
    }

    private send(message: MercivMessage) {
        log.verbose('JS send', message);
        // const event = new CustomEvent('merciv-js-to-unity', { detail: JSON.stringify(message) });
        // this.window?.dispatchEvent(event);
        const event = {
          'topic':'merciv-js-to-unity', 
          'detail': message,
        };
        socket.send(JSON.stringify(event))
    }

    private onUnityEditorMessage(event) { // Update the type of the event parameter
        log.verbose('JS onUnityMessage',event);

        const message = JSON.parse(event).detail;

        if (message['response-id']) {
            log.verbose('JS onUnityMessage: response', message);
            const responseId = message['response-id'];
            const response = this.rpcs[responseId];
            if (!response) {
                log.verbose('JS onUnityMessage: no response handler for id', responseId);
                return;
            }

            response(message.args[0]);
            delete this.rpcs[responseId];

        } else {
            log.verbose('JS onUnityMessage: event', message);
            if (!this.eventHandlers[message.event]) {
                log.verbose('JS onUnityMessage: no handlers for event', message.event);
                return;
            }

            for (const handler of this.eventHandlers[message.event]) {
                const returnValue = handler(...message.args);
                if (message['rpc-id']) {
                    this.send({ event: message.event, args: [returnValue], 'response-id': message['rpc-id'] });
                }
            }
        }
    }
}

const unityEditor = new MercivUnityEditorBridge();
export default unityEditor;